using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Pattern.Managers;
using Pattern.Configs;
using DG.Tweening;

public class GameLogic : MonoBehaviour
{
    [SerializeField] EventSystem m_eventSystem;
    [SerializeField] BoardManager m_boardHandler;
    [SerializeField] BallManager m_ballHandler;
    [SerializeField] LineManager m_lineHandler;
    [SerializeField] CoverManager m_coverHandler;
    [SerializeField] BombHandler m_bombHandler;
    private PatternHandler m_patternHandler;
    private int m_matchCount;
    private int m_unitScore; // nameing is suck
    private int m_modeScore; // nameing is suck
    private SlotPrefab[] m_bottomArray;
    private SlotPrefab[] m_board;
    private Vector3[] m_shape = null;
    private bool[] m_isWorking;

    public int Score { get; private set; } = 0;
    public bool Finish = false;
    public int BonusTimeSecond;

    private void Awake()
    {
        m_patternHandler = new PatternHandler();
        m_patternHandler.InputEnd = FinishDrag;

        m_lineHandler.SetPatternHandler = m_patternHandler;
        m_boardHandler.SetPatternHandler = m_patternHandler;
        m_boardHandler.SetBombAction = Bomb;
        m_boardHandler.SetBeginAction = m_lineHandler.Begin;
        m_boardHandler.SetAddAction = m_lineHandler.Append;
        m_boardHandler.SetRemoveAction = m_lineHandler.Remove;
        m_coverHandler.transform.localScale = m_boardHandler.transform.localScale;

        m_bombHandler.d_bomb = Bomb;
        m_bombHandler.d_score = BombScore;
        m_bombHandler.d_request = RequestBall;
    }

    public void InitGame()
    {
        uint height = m_boardHandler.Size.Column;
        m_boardHandler.Create();
        m_bottomArray = m_boardHandler.Data
                        .OrderBy(e => e.Value.id)
                        .Where(e => e.Value.id % height == 0)
                        .Select(e => e.Value)
                        .ToArray();
        m_board = m_boardHandler.Data.OrderBy(e => e.Value.id).Select(e => e.Value).ToArray();
        m_isWorking = new bool[m_bottomArray.Length];
    }

    public void CreateGame()
    {
        Score = 0;
        Finish = false;
        BonusTimeSecond = CONST.BONUS_TIMER_BEGIN_VALUE;

        m_lineHandler.Clear();
        RequestBall();
    }

    public void ClearGame()
    {
        m_lineHandler.Clear();
        m_boardHandler.Clear();
        m_patternHandler.Clear();
        m_coverHandler.Clear();
        SlotPrefab.Activate = false;
    }

    public void ClearBall()
    {
        m_lineHandler.Clear();
        m_ballHandler.DropAndClear();
        m_boardHandler.ClearChild();
        m_patternHandler.Clear();
        SlotPrefab.Activate = false;
    }

    /*
     * privates... complicated functions...
     */
    private void Bomb(SlotPrefab slot)
    {
        if (slot.Generate != null || slot.Child == null) { return; }

        switch (slot.Child.BallColor)
        {
            case SlotAttribute.bomb1:
                m_eventSystem.enabled = false;
                StartCoroutine(m_bombHandler.DisposeBomb1(slot));
                break;
            case SlotAttribute.bomb2:
                m_eventSystem.enabled = false;
                StartCoroutine(m_bombHandler.DisposeBomb3(slot));
                break;
            case SlotAttribute.bomb3:
                m_eventSystem.enabled = false;
                StartCoroutine(m_bombHandler.DisposeBomb4(slot));
                break;
            default:
                break;
        }
    }

    private void BombScore()
        => Score += CONST.SCORE_BOMB;

    private int UnitScore
        => m_patternHandler.Selected().Select((e, index) => (e, index)).Sum(p => p.index) + m_patternHandler.Selected().Count();

    private int ModeScore
        => m_patternHandler.Selected().Count - (int)CONST.MIN_SELECT;

    private void FinishDrag()
    {
        m_unitScore = UnitScore;
        m_modeScore = ModeScore;
        m_shape = m_patternHandler.ShapeOffset();
        m_matchCount = 0;
        m_lineHandler.Clear();
        
        StartCoroutine(DisposeMatch());
    }

    private void DisposeMatchBall(IGrouping<int, SlotPrefab> key)
    {
        SlotPrefab slot = m_boardHandler.Data[key.Key];
        int matchCount = key.Count();

        switch (matchCount)
        {
            case 1:
                m_ballHandler.Release(slot.Child);
                slot.Child = null;
                break;
            case 2:
                slot.Child.BallColor = SlotAttribute.bomb1;
                break;
            case 3:
                slot.Child.BallColor = SlotAttribute.bomb2;
                break;
            default:
                slot.Child.BallColor = SlotAttribute.bomb3;
                break;
        }
    }

    /* complicated functions... can be simple? */
    private void RequestBall()
    {
        if (Finish)
            return;
        
        m_eventSystem.enabled = false;
        int count = 0;
        Sequence sequence = DOTween.Sequence();

        foreach (var e in m_board)
        {
            SlotPrefab upper = Ray.Instance.Shot(e.transform.position + CONST.DIRECTION_OFFSET[(int)ClockWise.up]);
            if (upper == null)
            {
                if (e.Generate(e))
                    ++count;
            }
            else
            {
                if (e.Child == null && upper.Child != null)
                {
                    sequence.Join(upper.Child.MoveTo(e));
                    upper.Child = null;
                }
            }
        }

        sequence.OnComplete(() => {
            if (count > 0)
                RequestBall();
            else
                StartCoroutine(DisposeMatch());
        }).Play();
    }

    /* complicated functions... can be simple? */
    IEnumerator DisposeMatch()
    {
        if (m_shape == null)
        {
            m_eventSystem.enabled = true;
            yield break;
        }

        m_eventSystem.enabled = false;

        List<SlotPrefab> matchedList = Matched();
        var group = matchedList.GroupBy(e => e.GetInstanceID());

        if (group.Count().Equals(0))
        {
            if (!Finish)
            {
                m_shape = null;
                m_eventSystem.enabled = true;
            }
        }
        else
        {
            yield return new WaitForSecondsRealtime(CONST.DURATION_WAIT_REMOVE);

            m_lineHandler.Clear();
            foreach (var key in group)
                DisposeMatchBall(key);

            yield return new WaitForSecondsRealtime(CONST.DURATION_WAIT_FILL_BALL);
            RequestBall();
        }
    }

    private List<SlotPrefab> Matched()
    {
        return m_boardHandler.Data
            .Select(e => DrawMatchedElement(e.Value, m_shape))
            .Where(list => list != null)
            .SelectMany(list => {
                Score += m_unitScore * Multi(++m_matchCount, m_modeScore);
                m_lineHandler.ToLine(list);
                return list.Select(e => e);
            })
            .ToList();
    }
    
    private List<SlotPrefab> DrawMatchedElement(SlotPrefab origin, Vector3[] offsetArray)
    {
        Vector3 position = origin.transform.position;

        if (origin.Generate != null)
            return null;

        var list = (from offset in offsetArray
                    let hit = Ray.Instance.Shot(position -= offset)
                    where hit != null && hit.Generate == null && !hit.Child.IsBomb() && hit.Child.BallColor.Equals(origin.Child.BallColor)
                    select hit)
                    .Reverse().ToList();
        list.Add(origin);                        

        if (list.Count > offsetArray.Length)
            return list;
        return null;
    }

    private int Multi(int count, int mode)
    {
        int rst = count * (int)Math.Pow(count * 2, mode);
        
        if (1 < count)
            BonusTimeSecond += mode;
        return rst;
    }
}
