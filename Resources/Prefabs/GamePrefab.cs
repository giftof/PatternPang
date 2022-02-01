using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pattern.Configs;
using Pattern.Objects;
using Pattern.Managers;



public class GamePrefab : MonoBehaviour
{
    public ObjectPool slotPool;
    public SlotPrefab[] BaseLine { get; set; } = null;
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
        Vector3 beginPosition = Vector3.left * (PatternHandler.Instance.Size.Row - 1) * widthUnit * .5f;
        Vector3 currentPosition = beginPosition;
        SlotPrefab lower;

        Clear();
        BaseLine = new SlotPrefab[PatternHandler.Instance.Size.Row];
        slotArray = new SlotPrefab[PatternHandler.Instance.Size.Row * PatternHandler.Instance.Size.Column];

        for (uint w = 0; w < PatternHandler.Instance.Size.Row; ++w)
        {
            lower = null;
            if (PatternHandler.Instance.IsFloating(w))
                currentPosition += Vector3.up * slotSize.y * .5f;
            for (uint h = 0; h < PatternHandler.Instance.Size.Column; ++h)
            {
                SlotPrefab slot = slotPool.Request(transform, currentPosition).GetComponent<SlotPrefab>();
                slot.Slot = new Slot();
                slot.Slot.Id = w * PatternHandler.Instance.Size.Column + h;
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
            return e;
        }).ToArray();
    }

    private void SetDistance()
    {
        float[] distance = new float[5];
        distance[0] = Vector3.Distance(slotArray[0].transform.localPosition, slotArray[PatternHandler.Instance.Size.Column * 2 + 1].transform.localPosition);
        distance[1] = Vector3.Distance(slotArray[0].transform.localPosition, slotArray[2].transform.localPosition);
        distance[2] = Vector3.Distance(slotArray[0].transform.localPosition, slotArray[PatternHandler.Instance.Size.Column + 1].transform.localPosition);
        distance[3] = Vector3.Distance(slotArray[0].transform.localPosition, slotArray[PatternHandler.Instance.Size.Column].transform.localPosition);
        distance[4] = Vector3.Distance(slotArray[0].transform.localPosition, slotArray[1].transform.localPosition);
        float min = Mathf.Min(distance[0], distance[1], distance[2]);
        float max = Mathf.Max(distance[3], distance[4]);
        CONST.MAX_DISTANCE = (min + max) * .5f;

        Debug.Log($"CONST.MAX_DISTANCE = {CONST.MAX_DISTANCE}");
    }

    private void Sewing(SlotPrefab upper, ref SlotPrefab lower)
    {
        if (lower != null)
            lower.Upper = upper;
        lower = upper;
    }

    private void Clear()
    {
        if (slotArray != null)
            foreach (var item in slotArray)
                slotPool.Release(item.gameObject);

        slotArray = null;
        BaseLine = null;
    }
}
