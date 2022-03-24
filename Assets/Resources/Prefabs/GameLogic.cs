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
    [SerializeField] BulletManager m_bulletHandler;
    [SerializeField] CharactorManager m_charactorHandler;
    [SerializeField] ComboManager m_comboHandler;

    private PatternHandler m_patternHandler;
    private int m_matchCount;
    private int m_unitScore; // nameing is suck
    private int m_modeScore; // nameing is suck
    private SlotPrefab[] m_board;
    private SlotPrefab m_first = null;
    private Vector3[] m_shape = null;

    public int Score { get; private set; } = 0;
    public bool Finish { get; set; } = true;
    public int BonusTimeSecond;

    private void Start()
    {
        m_patternHandler = new PatternHandler();
        m_patternHandler.InputEnd = FinishDrag;

        m_lineHandler.SetPatternHandler = m_patternHandler;

        m_boardHandler.SetPatternHandler = m_patternHandler;
        m_boardHandler.SetBeginAction = m_lineHandler.Begin;
        m_boardHandler.SetAddAction = m_lineHandler.Append;
        m_boardHandler.SetRemoveAction = m_lineHandler.Remove;

        m_coverHandler.transform.localScale = m_boardHandler.transform.localScale;

        m_charactorHandler.Request(m_charactorHandler.transform);

        InitGame();
    }

    public void CreateGame()
    {
        Score = 0;
        Finish = false;
        BonusTimeSecond = CONST.BONUS_TIMER_BEGIN_VALUE;

        m_lineHandler.Clear();
        RequestBall();
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
    private int UnitScore
        => m_patternHandler.Selected().Select((e, index) => (e, index)).Sum(p => p.index) + m_patternHandler.Selected().Count();

    private int ModeScore
        => m_patternHandler.Selected().Count - (int)CONST.MIN_SELECT;

    private void UpdateScore()
        => Score += m_unitScore * Multi(++m_matchCount, m_modeScore);

    private void FinishDrag()
    {
        m_unitScore = UnitScore;
        m_modeScore = ModeScore;
        m_first = m_patternHandler.First();
        m_shape = m_patternHandler.ShapeOffset();
        m_patternHandler.Clear();
        m_matchCount = 0;

        if (m_shape == null)
            m_lineHandler.Clear();
        else
        {
            m_eventSystem.enabled = false;
            UpdateScore();
            m_comboHandler.Display(m_first.transform, m_matchCount);
            StartCoroutine(DisposeSequentialPattern());
        }
    }

    private void ReleaseEventsystem()
    {
        m_eventSystem.enabled = true;
        m_first = null;
        m_shape = null;
    }

    private void ShootBullet(IGrouping<int, SlotPrefab> group)
    {
        BulletPrefab bullet = m_bulletHandler.Request();
        bullet.transform.position = group.First().transform.position;
        m_bulletHandler.Move(bullet, m_charactorHandler.First());
    }

    private void DisposePatternedBall(IGrouping<int, SlotPrefab> group)
    {
        SlotPrefab slot = group.First();

        m_ballHandler.Release(slot.Child);
        slot.Child = null;
    }

    /* complicated functions... can be simple? */
    private void InitGame()
    {
        int height = m_boardHandler.Size.Column;
        m_boardHandler.Create();
        m_board = m_boardHandler.Data.OrderBy(e => e.Value.id).Select(e => e.Value).ToArray();
    }

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
                StartCoroutine(DisposeSequentialPattern());
        }).Play();
    }

    IEnumerator DisposeSequentialPattern()
    {
        if (m_shape == null)
        {
            ReleaseEventsystem();
            yield break;
        }

        List<List<SlotPrefab>> m = Pattern();
        if (m.Count > 0)
        {
            foreach (var e in m)
            {
                if (e.First().id.Equals(m_first?.id))
                    m_first = null;
                else
                {
                    yield return new WaitForSecondsRealtime(CONST.DURATION_WAIT_MATCH_BALL);
                    UpdateScore();
                    m_comboHandler.Display(e.First().transform, m_matchCount);
                    m_lineHandler.Clear();
                    m_lineHandler.ToLine(e);
                    m_ballHandler.ToPunch(e);
                }
            }
            yield return new WaitForSecondsRealtime(CONST.DURATION_WAIT_MATCH_BALL);
            m_lineHandler.Clear();
            ShootAndDispose(m);
            RequestBall();
        }
        else
            ReleaseEventsystem();
    }

    private void ShootAndDispose(List<List<SlotPrefab>> l)
    {
        foreach (var e in l.SelectMany(e1 => e1.Select(e2 => e2)).GroupBy(e => e.id))
        {
            ShootBullet(e);
            DisposePatternedBall(e);
        }
    }

    private List<List<SlotPrefab>> Pattern()
        => (from e in m_boardHandler.Data
            let line = DrawPatternedElement(e.Value)
            where line != null
            select line).ToList();

    private List<SlotPrefab> DrawPatternedElement(SlotPrefab origin)
    {
        Vector3 position = origin.transform.position;

        if (origin.Generate != null)
            return null;

        var list = (from offset in m_shape
                    let hit = Ray.Instance.Shot(position -= offset)
                    where hit != null && hit.Generate == null && hit.Child.BallColor.Equals(origin.Child.BallColor)
                    select hit)
                    .ToList();

        if (list.Count.Equals(m_shape.Length))
        {
            list.Insert(0, origin);
            return list;
        }

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
