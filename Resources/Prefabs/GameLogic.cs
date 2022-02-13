using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Pattern.Managers;

public class GameLogic : MonoBehaviour
{
    [SerializeField] EventSystem eventSystem;

    public static GameLogic Instance;
    public int Score { get; private set; } = 0;
    private int m_matchCount;

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
    }

    public void ClearBall()
    {
        LineManager.Instance.Clear();
        BallManager.Instance.DropAndClear();
        BoardManager.Instance.ClearChild();
    }

    IEnumerator RequestBall(Action action)
    {
        eventSystem.enabled = false;
        yield return null;
        int count = 0;

        foreach (var element in BoardManager.Instance.BottomLine)
        {
            if (element.Dispose())
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
            yield return new WaitForSecondsRealtime(1);

            LineManager.Instance.Clear();
            foreach (var key in group)
                DisposeMatchBall(key.Key);

            yield return new WaitForSecondsRealtime(.1f);
            StartCoroutine(RequestBall(() => { TEST(trace, unitScore); }));
        }
    }

    private void TEST(Vector3[] trace, int unitScore)
    {
        StartCoroutine(FindMatch(trace, unitScore));
    }

    private void DisposeMatchBall(int objectId)
    {
        SlotPrefab slot = BoardManager.Instance.Data[objectId];
        BallManager.Instance.Release(slot.Child);
        slot.Child = null;
    }
}
