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

    public void Generate()
    {
        Vector3 beginPosition = Vector3.left * (Size.Row - 1) * widthUnit * .5f;
        Vector3 currentPosition = beginPosition;
        SlotPrefab lower;

        Clear();
        BaseLine = new SlotPrefab[Size.Row];
        slotArray = new SlotPrefab[Size.Row * Size.Column];

        for (uint w = 0; w < Size.Row; ++w)
        {
            lower = null;
            if (IsFloating(w))
                currentPosition += Vector3.up * slotSize.y * .5f;
            for (uint h = 0; h < Size.Column; ++h)
            {
                SlotPrefab slot = slotPool.Request(transform, currentPosition).GetComponent<SlotPrefab>();
                slot.Slot = new Slot();
                slot.Slot.Id = w * Size.Column + h;
                slotArray[slot.Slot.Id] = slot;
/* debug code */slot.name = slot.Slot.Id.ToString();
                currentPosition += Vector3.up * slotSize.y;

                Sewing(slot, ref lower);
                BaseLine[w] = BaseLine[w] ?? lower;
            }
            currentPosition = beginPosition + Vector3.right * (w + 1) * widthUnit;
        }
        SetDistance();
    }

    public SlotPrefab[] BallGenerator()
    {
        return BaseLine.Select(e => {
            while (e.Upper != null) e = e.Upper;
            e.GetComponent<Image>().raycastTarget = false;
            return e;
        }).ToArray();
    }

    public void Clear()
    {
        if (slotArray != null)
            foreach (var item in slotArray)
                slotPool.Release(item.gameObject);

        slotArray = null;
        BaseLine = null;
    }

    private void SetDistance()
    {
        float[] distance = new float[5];
        distance[0] = Vector3.Distance(slotArray[0].transform.localPosition, slotArray[Size.Column * 2 + 1].transform.localPosition);
        distance[1] = Vector3.Distance(slotArray[0].transform.localPosition, slotArray[2].transform.localPosition);
        distance[2] = Vector3.Distance(slotArray[0].transform.localPosition, slotArray[Size.Column + 1].transform.localPosition);
        distance[3] = Vector3.Distance(slotArray[0].transform.localPosition, slotArray[Size.Column].transform.localPosition);
        distance[4] = Vector3.Distance(slotArray[0].transform.localPosition, slotArray[1].transform.localPosition);
        float min = Mathf.Min(distance[0], distance[1], distance[2]);
        float max = Mathf.Max(distance[3], distance[4]);
        CONST.MAX_DISTANCE = (min + max) * .5f;

        //Debug.Log($"CONST.MAX_DISTANCE = {CONST.MAX_DISTANCE}");
    }

    private bool IsFloating(uint row)
        => row % 2 == CONST.EVEN_COLUMN_UP;

    private void Sewing(SlotPrefab upper, ref SlotPrefab lower)
    {
        if (lower != null)
            lower.Upper = upper;
        lower = upper;
    }

    private void Dispose()
    {
        Vector3[] shape = PatternHandler.Instance.ShapeOffset();

        if (shape == null)
            return;

        foreach (SlotPrefab item in slotArray)
        {
            SlotAttribute attribute = item.Slot.Color;
            Vector3 position = item.transform.localPosition;

            var group = shape.GroupBy(
                    e => PatternHandler.Instance.rayPrefab.Shot(position += e).Slot.Color.Equals(attribute),
                    e => PatternHandler.Instance.rayPrefab.Shot(position),
                    (key, value) => new {
                        Key = key,
                        Count = value.Count(),
                    }
                );

            Debug.Log($">> group.Count = {group.Count()}");
            foreach (var g in group)
                Debug.Log($"g.Key = {g.Key}, g.Count = {g.Count}");
        }
    }
}
