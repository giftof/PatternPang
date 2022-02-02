using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pattern.Configs;
using Pattern.Objects;
using Pattern.Managers;



public class GamePrefab : MonoBehaviour
{
    [SerializeField] ObjectPool slotPool;
    [SerializeField] ObjectPool ballPool;
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

    public void Initialize((uint row, uint column) size)
    {
        Size = size;

        Clear();
        CreateBoard();
        SetDistance();
        SetBallGenerator();
        StartCoroutine(FillBoard());
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
                    slot.Initialize(new Slot(w * Size.Column + h));

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

    public void Clear()
    {
        if (slotArray != null)
            foreach (var item in slotArray)
                slotPool.Release(item.gameObject);

        slotArray = null;
        BaseLine = null;
    }

    /* at least 3 x 3 brard */
    private void SetDistance()
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

    private Color ConvertToColor(SlotAttribute attribute)
    {
        return attribute switch
        {
            SlotAttribute.red => Color.red,
            SlotAttribute.green => Color.green,
            SlotAttribute.blue => Color.blue,
            SlotAttribute.yellow => Color.yellow,
            SlotAttribute.purple => Color.cyan,
            SlotAttribute.bomb1 => Color.black,
            SlotAttribute.bomb2 => Color.black,
            SlotAttribute.bomb3 => Color.black,
            _ => Color.white,
        };
    }

    IEnumerator FillBoard()
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
            StartCoroutine(FillBoard());

        /* test code begin */
        else
        {
            foreach (var item in slotArray)
            {
                Debug.Log($"id = {item.Slot.Id}, color = {item.Slot.Color}");
            }
        }
        /* test code end */
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
}
