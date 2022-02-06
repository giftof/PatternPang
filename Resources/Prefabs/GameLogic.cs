using System;
using System.Linq;
using System.Collections;
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

    public SlotPrefab[] BaseLine { get; set; } = null;
    public (uint Row, uint Column) Size { get; set; } = (0, 0);

    private Vector2 slotSize;
    private float widthUnit;
    private SlotPrefab[] slotArray = null;



    private void Awake()
    {
        slotSize = slotPool.prefab.GetComponent<RectTransform>().sizeDelta;
        widthUnit = slotSize.x * 0.75f;
    }

    public void Initialize()
    {
        eventSystem.enabled = false;

        ClearBoard();
        CreateBoard();
        PresetDistance();
        SetBallGenerator();
        StartCoroutine(FillBoard(() => {
            eventSystem.enabled = true;
        }));
    }

    public void ClearBall()
    {
        eventSystem.enabled = false;

        if (slotArray != null)
            foreach (var item in slotArray)
            {
                if (item.Ball != null)
                    item.Ball.Drop(ballPool.Release, item.Ball.gameObject);
                item.Ball = null;
            }

        StartCoroutine(WaitDrop(() => {
            eventSystem.enabled = true;
        }));
    }

    public void ClearBoard()
    {
        if (slotArray != null)
            foreach (var item in slotArray)
            {
                if (item.Ball != null)
                    ballPool.Release(item.Ball.gameObject);
                item.Ball = null;
                slotPool.Release(item.gameObject);
            }

        slotArray = null;
        BaseLine = null;
    }

    private void SetBallGenerator()
    {
        BaseLine.Select(e => {
            while (e.Upper != null) e = e.Upper;
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
        Vector3 beginPosition = Vector3.left * (Size.Row - 1) * widthUnit * .5f;
        Vector3 currentPosition = beginPosition;
        SlotPrefab lower;

        BaseLine = new SlotPrefab[Size.Row];
        slotArray = new SlotPrefab[Size.Row * Size.Column];

        for (uint w = 0; w < Size.Row; ++w)
        {
            lower = null;
            if (IsFloating(w))
                currentPosition += Vector3.up * slotSize.y * .5f;
            for (uint h = 0; h < Size.Column; ++h)
            {
                if (slotPool.Request<SlotPrefab>(transform, currentPosition) is SlotPrefab slot)
                {
                    slot.Initialize(new Slot(w * Size.Column + h), AfterDraw);

                    slotArray[slot.Slot.Id] = slot;
                    currentPosition += Vector3.up * slotSize.y;

                    BaseLine[w] = BaseLine[w] ?? slot;
                    Sewing(slot, ref lower);
                }
                else
                    throw new Exception("slotPool make Exception");
            }
            currentPosition = beginPosition + Vector3.right * (w + 1) * widthUnit;
        }
    }

    /* at least 3 x 3 brard */
    private void PresetDistance()
    {
        float[] distance = new float[5];
        distance[0] = Vector3.Distance(slotArray[0].transform.position, slotArray[Size.Column * 2 + 1].transform.position);
        distance[1] = Vector3.Distance(slotArray[0].transform.position, slotArray[2].transform.position);
        distance[2] = Vector3.Distance(slotArray[0].transform.position, slotArray[Size.Column + 1].transform.position);
        distance[3] = Vector3.Distance(slotArray[0].transform.position, slotArray[Size.Column].transform.position);
        distance[4] = Vector3.Distance(slotArray[0].transform.position, slotArray[1].transform.position);
        float min = Mathf.Min(distance[0], distance[1], distance[2]);
        float max = Mathf.Max(distance[3], distance[4]);
        CONST.MAX_DISTANCE = (min + max) * .5f;
    }

    private bool IsFloating(uint row)
        => row % 2 == CONST.EVEN_COLUMN_UP;

    private void Sewing(SlotPrefab upper, ref SlotPrefab lower)
    {
        if (lower != null)
            lower.Upper = upper;
        lower = upper;
    }

    IEnumerator FillBoard(Action action)
    {
        while (BallPrefab.TweeningCount > 0)
            yield return null;

        int nullCount = 0;

        foreach (var item in slotArray)
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
            ballPrefab.Color = (SlotAttribute)UnityEngine.Random.Range(1, (int)SlotAttribute.count - CONST.LEVEL1);
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
        Vector3[] shape = PatternHandler.Instance.ShapeOffset();
        PatternHandler.Instance.Clear();
        LineManager.Instance.Clear();
    }
}
