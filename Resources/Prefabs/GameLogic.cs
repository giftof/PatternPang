using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Pattern.Managers;
using Pattern.Configs;

public class GameLogic : MonoBehaviour
{
    [SerializeField] EventSystem eventSystem;

    public static GameLogic Instance;
    public int Score { get; private set; } = 0;
    private int m_matchCount;
    private int m_bombLineCount;

    private void Awake()
    {
        Instance = this;
        PatternHandler.Instance.InputEnd = FinishDrag;
    }

    public void CreateGame()
    {
        Score = 0;
        LineManager.Instance.Clear();
        BoardManager.Instance.Create();
        StartCoroutine(RequestBall(null));
    }

    public void ClearGame()
    {
        LineManager.Instance.Clear();
        BoardManager.Instance.Clear();
        PatternHandler.Instance.Clear();
        SlotPrefab.Activate = false;
    }

    public void ClearBall()
    {
        LineManager.Instance.Clear();
        BallManager.Instance.DropAndClear();
        BoardManager.Instance.ClearChild();
        PatternHandler.Instance.Clear();
        SlotPrefab.Activate = false;
    }

    public void Bomb(SlotPrefab slot)
    {
        if (slot.Generate != null || slot.Child == null)
            return;

        switch (slot.Child.Color)
        {
            case SlotAttribute.bomb1:
                StartCoroutine(DisposeBomb1(slot));
                break;
            case SlotAttribute.bomb2:
                StartCoroutine(DisposeBomb2(slot));
                break;
            case SlotAttribute.bomb3:
                StartCoroutine(DisposeBomb3(slot));
                break;
            default:
                break;
        }
    }

    /*
     * privates... complicated functions...
     */
    private int UnitScore
        => PatternHandler.Instance.Selected().Select((e, index) => (e, index)).Sum(p => p.index) + PatternHandler.Instance.Selected().Count();

    private void FinishDrag()
    {
        int unitScore = UnitScore;
        Vector3[] trace = PatternHandler.Instance.ShapeOffset();
        m_matchCount = 0;

        if (trace != null)
            StartCoroutine(FindMatch(trace, unitScore));
        else
            LineManager.Instance.Clear();
    }

    private void RecursiveMatch(Vector3[] trace, int unitScore)
        => StartCoroutine(FindMatch(trace, unitScore));

    private void DisposeMatchBall(IGrouping<int, SlotPrefab> key)
    {
        SlotPrefab slot = BoardManager.Instance.Data[key.Key];
        int matchCount = key.Count();

        switch (matchCount)
        {
            case 1:
            //case 2:
                BallManager.Instance.Release(slot.Child);
                slot.Child = null;
                break;
            case 2:
            //case 3:
                slot.Child.Color = SlotAttribute.bomb1;
                break;
            case 3:
            //case 4:
                slot.Child.Color = SlotAttribute.bomb2;
                break;
            default:
                slot.Child.Color = SlotAttribute.bomb3;
                break;
        }
    }

    /* complicated functions... can be simple? */
    private bool FillSlot(SlotPrefab slot)
    {
        SlotPrefab upper = Ray.Instance.Shot(slot.transform.position + CONST.DIRECTION_OFFSET[(int)ClockWise.up]);

        if (slot.Child?.IsWorking ?? false)
            return true;

        if (upper == null)
            return slot.Generate?.Invoke(slot) ?? false;
        else
        {
            if (slot.Child == null)
            {
                if (upper.Child != null && !upper.Child.IsWorking)
                {
                    upper.Child.TransferTo(slot);
                    upper.Child = null;
                }
                return FillSlot(upper);
            }
        }
        return FillSlot(upper);
    }

    /* complicated functions... can be simple? */
    IEnumerator RequestBall(Action action)
    {
        eventSystem.enabled = false;
        yield return null;
        int count = 0;

        foreach (var element in BoardManager.Instance.BottomLine)
        {
            if (FillSlot(element))
                count++;
        }

        if (count > 0)
            StartCoroutine(RequestBall(action));
        else
        {
            eventSystem.enabled = true;
            action?.Invoke();
        }
    }

    /* complicated functions... can be simple? */
    IEnumerator FindMatch(Vector3[] trace, int unitScore)
    {
        eventSystem.enabled = false;

        yield return null;
        List<SlotPrefab> list = new List<SlotPrefab>();

        if (trace == null)
            yield break;

        BoardManager.Instance.Data
            .Select(e =>
            {
                Vector3 position = e.Value.transform.position;

                if (e.Value.Generate != null)
                    return false;

                List<SlotPrefab> shape = trace
                    .Where(offset => {
                        SlotPrefab slot = Ray.Instance.Shot(position -= offset);
                        if (slot == null
                            || slot.Generate != null
                            || !slot.Child.Color.Equals(e.Value.Child.Color))
                            return false;
                        return true;
                    })
                    .Select(offset => Ray.Instance.Shot(position))
                    .ToList();
                shape.Insert(0, e.Value);

                if (shape.Select(obj => e.Value.Child.Color.Equals(obj?.Child.Color)).Count().Equals(trace.Count() + 1))
                {
                    LineManager.Instance.ToLine(shape);
                    ++m_matchCount;
                    list.AddRange(shape);
                    Score += unitScore * m_matchCount;
                    Debug.LogWarning($"inc score = {unitScore * m_matchCount} = {unitScore} * {m_matchCount}");
                    return true;
                }
                return false;
            }).Count();

        var group = list.GroupBy(e => e.GetInstanceID());

        if (group.Count().Equals(0))
            eventSystem.enabled = true;
        else
        {
            yield return new WaitForSecondsRealtime(CONST.DURATION_WAIT_REMOVE);

            LineManager.Instance.Clear();
            foreach (var key in group)
                DisposeMatchBall(key);

            yield return new WaitForSecondsRealtime(CONST.DURATION_WAIT_FILL_BALL);
            StartCoroutine(RequestBall(() => { RecursiveMatch(trace, unitScore); }));
        }
    }

    IEnumerator DisposeBomb1(SlotPrefab slot)
    {
        eventSystem.enabled = false;

        ReleaseBombed(slot);
        m_bombLineCount += 1;
        foreach (var offset in CONST.DIRECTION_OFFSET)
        {
            SlotPrefab target = Ray.Instance.Shot(slot.transform.position + offset);
            if (target != null && target.Generate == null)
            {
                yield return new WaitForSecondsRealtime(CONST.DURATION_BOMB_STEP);
                ReleaseBombed(target, true);
            }
        }
        eventSystem.enabled = true;

        if (--m_bombLineCount == 0)
            StartCoroutine(RequestBall(null));
    }

    IEnumerator DisposeBomb2(SlotPrefab slot)
    {
        eventSystem.enabled = false;

        ReleaseBombed(slot);
        yield return new WaitForSecondsRealtime(CONST.DURATION_BOMB_STEP);

        eventSystem.enabled = true;
        m_bombLineCount += 1;
        StartCoroutine(RemoveLine(slot, ClockWise.up, ClockWise.down));
    }

    IEnumerator DisposeBomb3(SlotPrefab slot)
    {
        eventSystem.enabled = false;

        ReleaseBombed(slot);
        yield return new WaitForSecondsRealtime(CONST.DURATION_BOMB_STEP);

        eventSystem.enabled = true;
        m_bombLineCount += 3;
        StartCoroutine(RemoveLine(slot, ClockWise.up, ClockWise.down));
        StartCoroutine(RemoveLine(slot, ClockWise.upLeft, ClockWise.downRight));
        StartCoroutine(RemoveLine(slot, ClockWise.upRight, ClockWise.downLeft));
    }

    IEnumerator RemoveLine(SlotPrefab slot, ClockWise dir1, ClockWise dir2)
    {
        eventSystem.enabled = false;
        Vector3 pos1 = slot.transform.position;
        Vector3 pos2 = slot.transform.position;

        while(true)
        {
            int failCount = 0;
            SlotPrefab target1 = Ray.Instance.Shot(pos1 += CONST.DIRECTION_OFFSET[(int)dir1]);
            SlotPrefab target2 = Ray.Instance.Shot(pos2 += CONST.DIRECTION_OFFSET[(int)dir2]);

            if (target1 == null || target1.Generate != null)
                ++failCount;
            else
                ReleaseBombed(target1, true);

            if (target2 == null || target2.Generate != null)
                ++failCount;
            else
                ReleaseBombed(target2, true);

            if (failCount > 1)
                break;
            yield return new WaitForSecondsRealtime(CONST.DURATION_BOMB_STEP);
        }
        eventSystem.enabled = true;

        if (--m_bombLineCount == 0)
            StartCoroutine(RequestBall(null));
    }

    private void ReleaseBombed(SlotPrefab slot, bool recursive = false)
    {
        if (recursive)
            Bomb(slot);

        if (slot.Child)
        {
            BallManager.Instance.Release(slot.Child);
            slot.Child = null;
            Score += CONST.SCORE_BOMB;
        }
    }
}

