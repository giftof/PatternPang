using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class GameLogic: MonoBehaviour {
    private EventSystem m_eventSystem;
    private BoardManager m_boardHandler;
    private BallManager m_ballHandler;
    private LineManager m_lineHandler;
    private CoverManager m_coverHandler;
    private BulletManager m_bulletHandler;
    private CharactorManager m_charactorHandler;
    private ComboManager m_comboHandler;

    private PatternHandler m_patternHandler;
    private int m_matchCount;
    private int m_unitScore; // nameing is suck
    private int m_modeScore; // nameing is suck
    private SlotPrefab m_first = null;
    private Vector3[] m_shape = null;

    public SlotPrefab[] Board;
    public int Score { get; private set; } = 0;
    public bool Finish { get; set; } = true;
    public int BonusTimeSecond;
    public BoardManager BoardManager => m_boardHandler;

    private void Awake() {
        m_ballHandler = Instantiate(Resources.Load<BallManager>("Prefabs/_Manager/BallManager"), transform.parent);
        m_boardHandler = Instantiate(Resources.Load<BoardManager>("Prefabs/_Manager/BoardManager"), transform.parent);
        m_lineHandler = Instantiate(Resources.Load<LineManager>("Prefabs/_Manager/LineManager"), transform.parent);
        m_coverHandler = Instantiate(Resources.Load<CoverManager>("Prefabs/_Manager/CoverManager"), transform.parent);
        m_bulletHandler = Instantiate(Resources.Load<BulletManager>("Prefabs/_Bullet/BulletManager"), transform.parent);
        m_charactorHandler = Instantiate(Resources.Load<CharactorManager>("Prefabs/_Charactor/CharactorManager"), transform.parent);
        m_comboHandler = Instantiate(Resources.Load<ComboManager>("Prefabs/_Manager/ComboManager"), transform.parent);
        
        m_boardHandler.ballManager = m_ballHandler;
        m_boardHandler.coverManager = m_coverHandler;
    }

    private void Start() {
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

    private void InitGame() {
        int height = m_boardHandler.Size.Column;
        m_boardHandler.Create();
        Board = m_boardHandler.SlotArray();
    }

    public EventSystem EventSystem {
        set => m_eventSystem = value;
    }

    public (int Row, int Column)Size {
        set => m_boardHandler.Size = value;
    }

    public int BallVariation {
        set => m_ballHandler.BallVariation = value;
    }

    public void CreateGame() {
        Score = 0;
        Finish = false;
        BonusTimeSecond = CONST.BONUS_TIMER_BEGIN_VALUE;

        m_lineHandler.Clear();
        RequestBall();
    }

    public void ClearBall() {
        m_lineHandler.Clear();
        m_ballHandler.DropAndClear();
        m_boardHandler.ClearChild();
        m_patternHandler.Clear();
        SlotPrefab.Activate = false;
    }

    private int UnitScore 
        => m_patternHandler.Selected().Select((e, index) => (e, index)).Sum(p => p.index)
         + m_patternHandler.Selected().Count();

    private int ModeScore 
        => m_patternHandler.Selected().Count
         - (int)CONST.MIN_SELECT;

    private void UpdateScore() 
        => Score += m_unitScore * Multi(++m_matchCount, m_modeScore);

    private void ReleaseEventsystem() {
        m_eventSystem.enabled = true;
        m_lineHandler.Clear();
        m_first = null;
        m_shape = null;
    }

    private void FinishDrag() {
        m_unitScore = UnitScore;
        m_modeScore = ModeScore;
        m_first = m_patternHandler.First();
        m_shape = m_patternHandler.ShapeOffset();
        m_patternHandler.Clear();
        m_matchCount = 0;

        if (m_shape == null)
            ReleaseEventsystem();
        else {
            m_eventSystem.enabled = false;
            UpdateScore();
            m_comboHandler.Display(m_first.transform, m_matchCount);
            StartCoroutine(DisposePatternSequentially());
        }
    }

    private void FireBullet(IGrouping<int, SlotPrefab> group)
        => m_bulletHandler.FireTo(
                group.First().transform.position,
                m_charactorHandler.First().transform.position,
                () => m_charactorHandler.First().Scaling()
            );

    IEnumerator DisposePatternSequentially() {
        if (m_shape == null) {
            ReleaseEventsystem();
            yield break;
        }

        List<List<SlotPrefab>> m = m_boardHandler.Pattern(MatchedList);
        if (m.Count > 0) {
            foreach (var e in m) {
                if (e.First().id.Equals(m_first?.id)) 
                    m_first = null;
                else {
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
            DisposeMatchedAction(m);
            RequestBall();
        } else 
            ReleaseEventsystem();
    }
    
    private void DisposeMatchedAction(List<List<SlotPrefab>> matchedList) {
        foreach (var e in matchedList.SelectMany(e1 => e1.Select(e2 => e2)).GroupBy(e => e.id)) {
            FireBullet(e);
            RemoveBall(e);
        }
    }

    private void RemoveBall(IGrouping<int, SlotPrefab> group) {
        SlotPrefab slot = group.First();

        m_ballHandler.Release(slot.Child);
        slot.Child = null;
    }

    private List<SlotPrefab> MatchedList(SlotPrefab origin) {
        Vector3 position = origin.transform.position;

        if (origin.Generate != null) 
            return null;
        
        var list = (from offset in m_shape 
                    let hit = Ray.Instance.Shoot(position -= offset)
                    where hit != null && hit.Generate == null && hit.Child.BallColor.Equals(origin.Child.BallColor)
                    select hit).ToList();

        if (list.Count.Equals(m_shape.Length)) {
            list.Insert(0, origin);
            return list;
        }

        return null;
    }

    private void RequestBall() {
        bool flag = false;
        m_eventSystem.enabled = false;
        Sequence sequence = DOTween.Sequence();

        foreach (var e in Board) {
            SlotPrefab upper = Ray.Instance.Shoot(e.transform.position + CONST.DIRECTION_OFFSET[(int)ClockWise.up]);

            switch (upper) {
                case null:
                    if (e.Generate(e))
                        flag = true;
                    break;
                default:
                    if (e.Child == null && upper.Child != null) {
                        sequence.Join(upper.Child.MoveTo(e));
                        upper.Child = null;
                    }
                    break;
            }
        }

        sequence.OnComplete(() => {
            switch (flag) {
                case true:
                    RequestBall();
                    break;
                default:
                    StartCoroutine(DisposePatternSequentially());
                    break;
            }
        }).Play();
    }

    private int Multi(int count, int mode) {
        int rst = count * (int)Math.Pow(count * 2, mode);

        if (1 < count)
            BonusTimeSecond += mode;
        return rst;
    }
}
