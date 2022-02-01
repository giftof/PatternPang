using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Pattern.Objects;
using Pattern.Configs;
using Pattern.Managers;



public class SlotPrefab : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
{
    public Slot Slot { get; set; } = null;
    public SlotPrefab Upper { get; set; } = null;
    public BallPrefab Ball { get; set; } = null;
    public Action Disposer { get; set; } = null;
    private static bool activate = false;

    private void Awake() { }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!activate)
        {
            activate = true;
            PatternHandler.Instance.Add(transform.localPosition, Slot.Color);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (activate)
            PatternHandler.Instance.AddChecker(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (activate && Input.touchCount == 0)
        {
            activate = false;
            Disposer?.Invoke();
        }
    }
}
