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

    private Vector2 slotSize;
    private float widthUnit;
    private SlotPrefab[] slotArray;
    private List<SlotPrefab> matched;



    private void Awake()
    {
        slotSize = slotPool.prefab.GetComponent<RectTransform>().sizeDelta;
        widthUnit = slotSize.x * 0.75f;
    }

    public void Initialize()
        => StartCoroutine(Init());

    IEnumerator Init()
    {
        eventSystem.enabled = false;
        matched = new List<SlotPrefab>();
        LineManager.Instance.Clear();

        ClearBoard();
        CreateBoard();
        PresetOffset();

        yield return null;

        SetBallGenerator();
        StartCoroutine(FillBoard(() => { eventSystem.enabled = true; }));
    }

    public void ClearBall()
    {
        eventSystem.enabled = false;
        LineManager.Instance.Clear();

        if (slotArray != null)
            foreach (var item in slotArray)
            {
                if (item.Ball != null)
                    item.Ball.Drop(ballPool.Release, item.Ball.gameObject);
                item.Ball = null;
            }

        StartCoroutine(WaitDrop(() => { eventSystem.enabled = true; }));
    }

    public void ClearBoard()
    {
        LineManager.Instance.Clear();

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
        Vector3 beginPosition = Vector3.left * (Size.Row - 1) * widthUnit * .5f;
        Vector3 currentPosition = beginPosition;

        BaseLine = new SlotPrefab[Size.Row];
        slotArray = new SlotPrefab[Size.Row * Size.Column];

        for (uint w = 0; w < Size.Row; ++w)
        {
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
                }
                else
                    throw new Exception("slotPool make Exception");
            }
            currentPosition = beginPosition + Vector3.right * (w + 1) * widthUnit;
        }
    }

    private bool IsFloating(uint row)
        => row % 2 == CONST.EVEN_COLUMN_UP;

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

        matched.Clear();

        if (shape != null)
            SearchSamePattern(shape);

        PatternHandler.Instance.Clear();
        /*LineManager.Instance.Clear();*/
    }

    private void SearchSamePattern(Vector3[] shape)
    {
        List<SlotPrefab> list = new List<SlotPrefab>();

        slotArray
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
                Vector3 position = head.transform.position;

                list.Add(head);
                list.AddRange(shape.Select(offset => ray.Shot(position -= offset)));
                return list;
            }).ToArray();

        var group = list.GroupBy(e => e.Slot.Id);

        foreach (var g in group)
            Debug.Log($"key = {g.Key}, cnt = {g.Count()}");
        

        /*        Debug.Log($"selected count = {selected.Count()}");
                foreach (var item in selected)
                    foreach (var it in item)
                        *//*Debug.Log($"item = {item}");*//*
                        Debug.Log($"it = {it}");
        */

        //var array = slotArray
        //    .Where(slot => !slot.Slot.Color.Equals(SlotAttribute.generator))
        //    .SelectMany()
        //    .SelectMany(slot => shape.Where(offset => {

        //    }));
        //{
        //    if (!slot.Slot.Color.Equals(SlotAttribute.generator))
        //        return false;

        //    Vector3 position = slot.transform.position;
        //    var arr2 = shape.Where(offset => {
        //        position -= offset;
        //        return slot.Slot.Color.Equals(ray.Shot(position)?.Slot.Color);
        //    }).Select(offset => ray.Shot(position));

        //    foreach (var it in arr2)
        //    {
        //        Debug.Log($"it = {it}");
        //    }

        //    return arr2.Count() == shape.Length;
        //})
        //.Select(slot =>
        //{

        //})
        //.Select(slot => {
        //Vector3 position = slot.transform.position;

        //var arr2 = shape.Where(offset =>
        //{
        //    position -= offset;
        //    return slot.Slot.Color.Equals(ray.Shot(position)?.Slot.Color);
        //}).Select(offset => ray.Shot(position));



        //    foreach (var it in arr2)
        //    {
        //        Debug.Log($"it = {it}");
        //    }

        //    return arr2.Count() == shape.Length;
        //    //if (list.Count == shape.Length + 1)
        //    //    return list;
        //    //return false;
        //});

        //foreach (var item in array)
        //{
        //    Debug.Log($"item = {item}");
        //}






        //List<SlotPrefab> same = new List<SlotPrefab>();

        //foreach (var item in slotArray)
        //{
        //    if (item.Slot.Color.Equals(SlotAttribute.generator))
        //        continue;

        //    if (same.Count == shape.Count() + 1)
        //    {
        //        matched.AddRange(same);
        //        LineManager.Instance.ToLine(same);
        //    }
        //    same.Clear();

        //    Vector3 position = item.transform.position;
        //    same.Add(item);

        //    foreach (var offset in shape)
        //    {
        //        position -= offset;
        //        SlotPrefab hitSlot = ray.Shot(position);

        //        if (!item.Slot.Color.Equals(hitSlot?.Slot.Color))
        //            break;
        //        else
        //            same.Add(hitSlot);
        //    }
        //}
    }

    /* at least 3 x 3 brard */
    private void PresetOffset()
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
        CONST.DIRECTION_OFFSET[0] = slotArray[1].transform.position - slotArray[0].transform.position;
        CONST.DIRECTION_OFFSET[3] = CONST.DIRECTION_OFFSET[0] * -1;
        CONST.DIRECTION_OFFSET[1] = slotArray[Size.Column].transform.position - slotArray[0].transform.position;
        CONST.DIRECTION_OFFSET[4] = CONST.DIRECTION_OFFSET[1] * -1;
        CONST.DIRECTION_OFFSET[2] = CONST.DIRECTION_OFFSET[1] + Vector3.down * CONST.DIRECTION_OFFSET[1].y * 2;
        CONST.DIRECTION_OFFSET[5] = CONST.DIRECTION_OFFSET[2] * -1;
    }
}
