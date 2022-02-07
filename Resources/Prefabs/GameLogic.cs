using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Pattern.Configs;
using Pattern.Objects;
using Pattern.Managers;



public class GameLogic : MonoBehaviour
{
    [SerializeField] ObjectPool slotPool;
    [SerializeField] ObjectPool ballPool;
    [SerializeField] EventSystem eventSystem;
    [SerializeField] Ray ray;

    public SlotPrefab[] BaseLine { get; set; } = null;
    public (uint Row, uint Column) Size { get; set; } = (0, 0);
    public uint BallVar { get; set; } = 0;

    public int Score { get; private set; } = 0;
    private Vector2 m_slotSize;
    private float m_widthUnit;
    private SlotPrefab[] m_slotArray;
    private int m_matchCount;
    private int m_unitScore;



    private void Awake()
    {
        m_slotSize = slotPool.prefab.GetComponent<RectTransform>().sizeDelta;
        m_widthUnit = m_slotSize.x * 0.75f;
    }

    public void Initialize()
        => StartCoroutine(Init());

    IEnumerator Init()
    {
        Score = 0;
        m_matchCount = 0;
        eventSystem.enabled = false;
        LineManager.Instance.Clear();

        ClearBoard();
        CreateBoard();
        PresetOffset();

        yield return null;
        SetBallGenerator();

        yield return null;
        StartCoroutine(FillBoard(() => { Finish(); }));
    }

    public void ClearBall()
    {
        eventSystem.enabled = false;
        LineManager.Instance.Clear();

        if (m_slotArray != null)
            foreach (var item in m_slotArray)
            {
                if (item.Ball != null)
                    item.Ball.Drop(ballPool.Release, item.Ball.gameObject);
                item.Ball = null;
            }

        StartCoroutine(WaitDrop(() => { Finish(); }));
    }

    public void ClearBoard()
    {
        LineManager.Instance.Clear();

        if (m_slotArray != null)
            foreach (var item in m_slotArray)
            {
                if (item.Ball != null)
                    ballPool.Release(item.Ball.gameObject);
                item.Ball = null;
                slotPool.Release(item.gameObject);
            }

        m_slotArray = null;
        BaseLine = null;
    }

    private void Finish()
    {
        m_matchCount = 0;
        eventSystem.enabled = true;
        LineManager.Instance.Clear();
        PatternHandler.Instance.Clear();
    }

    private void SetBallGenerator()
    {
        BaseLine.Select(e => {
            while (true)
            {
                SlotPrefab hit = Ray.Instance.Shot(e.transform.position + CONST.DIRECTION_OFFSET[(int)ClockWise.up]);

                if (hit != null)
                    e = hit;
                else
                    break;
            }
            e.GetComponent<Image>().raycastTarget = false;
            e.Generate = GenerateBall;
            e.Slot.Color = SlotAttribute.generator;
            return e;
        }).ToArray();
    }

    private void GenerateBall(SlotPrefab destination)
    {
        if (destination.Ball != null)
            return;

        StartCoroutine(CreateBall(destination));
    }

    private void CreateBoard()
    {
        Vector3 beginPosition = Vector3.left * (Size.Row - 1) * m_widthUnit * .5f;
        Vector3 currentPosition = beginPosition;

        BaseLine = new SlotPrefab[Size.Row];
        m_slotArray = new SlotPrefab[Size.Row * Size.Column];

        for (uint w = 0; w < Size.Row; ++w)
        {
            if (IsFloating(w))
                currentPosition += Vector3.up * m_slotSize.y * .5f;
            for (uint h = 0; h < Size.Column; ++h)
            {
                if (slotPool.Request<SlotPrefab>(transform, currentPosition) is SlotPrefab slot)
                {
                    slot.Initialize(new Slot(w * Size.Column + h), AfterDraw);

                    m_slotArray[slot.Slot.Id] = slot;
                    currentPosition += Vector3.up * m_slotSize.y;

                    BaseLine[w] = BaseLine[w] ?? slot;
                }
                else
                    throw new Exception("slotPool make Exception");
            }
            currentPosition = beginPosition + Vector3.right * (w + 1) * m_widthUnit;
        }
    }

    private bool IsFloating(uint row)
        => row % 2 == CONST.EVEN_COLUMN_UP;

    IEnumerator FillBoard(Action action)
    {
        while (BallPrefab.TweeningCount > 0)
            yield return null;

        int nullCount = 0;

        foreach (var item in m_slotArray)
        {
            if (item.Ball == null)
            {
                ++nullCount;
                item.Dispose();
            }
        }

        if (nullCount > 0)
            StartCoroutine(FillBoard(action));
        else
            action?.Invoke();
    }

    IEnumerator CreateBall(SlotPrefab destination)
    {
        while (BallPrefab.TweeningCount > 0)
            yield return null;

        if (ballPool.Request<BallPrefab>(destination.transform.parent, destination.transform.localPosition) is BallPrefab ballPrefab)
        {
            ballPrefab.Color = (SlotAttribute)UnityEngine.Random.Range(1, (int)SlotAttribute.count - BallVar);
            destination.Ball = ballPrefab;
        }
        else
            throw new Exception("ballPool make Exception");
    }

    IEnumerator WaitDrop(Action action)
    {
        while (BallPrefab.TweeningCount > 0)
            yield return null;

        action?.Invoke();
    }

    private void AfterDraw()
    {
        eventSystem.enabled = false;
        Vector3[] shape = PatternHandler.Instance.ShapeOffset();
        /*m_unitScore = shape.Select((e, index) => (e, index)).Select(t => t.index).Sum();*/

Debug.LogWarning($"m_unitScore = {m_unitScore}");
        if (shape != null)
        {
            m_unitScore = 0;
            for (int i = 0; i < shape.Length;)
                m_unitScore += ++i;

            SearchSamePattern(shape);
        }
        else
            Finish();
    }

    private void SearchSamePattern(Vector3[] shape)
    {
        List<SlotPrefab> list = new List<SlotPrefab>();

        m_slotArray
            .Where(slot =>
            {
                if (slot.Slot.Color.Equals(SlotAttribute.generator))
                    return false;

                Vector3 position = slot.transform.position;
                return shape.FirstOrDefault(offset => !slot.Slot.Color.Equals(ray.Shot(position -= offset)?.Slot.Color)) == default;
            })
            .Select(slot => slot)
            .Select(head =>
            {
                List<SlotPrefab> local = new List<SlotPrefab>();
                Vector3 position = head.transform.position;

                ++m_matchCount;
                Score += m_matchCount * m_unitScore;
Debug.Log($"inc score = {m_matchCount} * {m_unitScore} = {m_matchCount * m_unitScore}");

                local.Add(head);
                local.AddRange(shape.Select(offset => ray.Shot(position -= offset)));
                list.AddRange(local);
                LineManager.Instance.ToLine(local);
                return list;
            }).ToArray();

        IEnumerable<IGrouping<uint, SlotPrefab>> group = list.GroupBy(e => e.Slot.Id).ToArray();

        if (group.Count() > 0)
            StartCoroutine(RemoveMatchedBall(group, shape.Length));
        else
            Finish();
    }

    IEnumerator RemoveMatchedBall(IEnumerable<IGrouping<uint, SlotPrefab>> group, int mod)
    {
        Debug.LogWarning("call [RemoveMatchedBall]");

        yield return new WaitForSecondsRealtime(1.0f);

        LineManager.Instance.Clear();

        yield return new WaitForSecondsRealtime(0.5f);

        foreach (var g in group)
        {
            /*Debug.Log($"key = {g.Key}, cnt = {g.Count()}");*/
            switch (g.Count())
            {
                case 1:
                    /*Debug.Log("case 1 normal remove");*/
                    ballPool.Release(m_slotArray[g.Key].Ball.gameObject);
                    m_slotArray[g.Key].Ball = null;
                    m_slotArray[g.Key].Slot.Color = SlotAttribute.none;
                    break;
                case 2:
                    /*Debug.Log("case 2 normal remove");*/
                    ballPool.Release(m_slotArray[g.Key].Ball.gameObject);
                    m_slotArray[g.Key].Ball = null;
                    m_slotArray[g.Key].Slot.Color = SlotAttribute.none;
                    break;
                case 3:     // bomb level 1
                    /*Debug.Log("case 3 bomb level 1");*/
                    ballPool.Release(m_slotArray[g.Key].Ball.gameObject);
                    m_slotArray[g.Key].Ball = null;
                    m_slotArray[g.Key].Slot.Color = SlotAttribute.none;
                    break;
                case 4:     // bomb level 2
                    /*Debug.Log("case 4 bomb level 2");*/
                    ballPool.Release(m_slotArray[g.Key].Ball.gameObject);
                    m_slotArray[g.Key].Ball = null;
                    m_slotArray[g.Key].Slot.Color = SlotAttribute.none;
                    break;
                default:    // bomb level 3
                    /*Debug.Log("case 5~ bomb level 3");*/
                    ballPool.Release(m_slotArray[g.Key].Ball.gameObject);
                    m_slotArray[g.Key].Ball = null;
                    m_slotArray[g.Key].Slot.Color = SlotAttribute.none;
                    break;
            }
        }

        StartCoroutine(FillBoard(() => {
            SearchSamePattern(PatternHandler.Instance.ShapeOffset());
        }));
    }

    /* at least 3 x 3 brard */
    private void PresetOffset()
    {
        float[] distance = new float[5];
        distance[0] = Vector3.Distance(m_slotArray[0].transform.position, m_slotArray[Size.Column * 2 + 1].transform.position);
        distance[1] = Vector3.Distance(m_slotArray[0].transform.position, m_slotArray[2].transform.position);
        distance[2] = Vector3.Distance(m_slotArray[0].transform.position, m_slotArray[Size.Column + 1].transform.position);
        distance[3] = Vector3.Distance(m_slotArray[0].transform.position, m_slotArray[Size.Column].transform.position);
        distance[4] = Vector3.Distance(m_slotArray[0].transform.position, m_slotArray[1].transform.position);
        float min = Mathf.Min(distance[0], distance[1], distance[2]);
        float max = Mathf.Max(distance[3], distance[4]);

        CONST.MAX_DISTANCE = (min + max) * .5f;
        CONST.DIRECTION_OFFSET[0] = m_slotArray[1].transform.position - m_slotArray[0].transform.position;
        CONST.DIRECTION_OFFSET[3] = CONST.DIRECTION_OFFSET[0] * -1;
        CONST.DIRECTION_OFFSET[1] = m_slotArray[Size.Column].transform.position - m_slotArray[0].transform.position;
        CONST.DIRECTION_OFFSET[4] = CONST.DIRECTION_OFFSET[1] * -1;
        CONST.DIRECTION_OFFSET[2] = CONST.DIRECTION_OFFSET[1] + Vector3.down * CONST.DIRECTION_OFFSET[1].y * 2;
        CONST.DIRECTION_OFFSET[5] = CONST.DIRECTION_OFFSET[2] * -1;
    }
}
