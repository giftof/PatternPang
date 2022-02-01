using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Pattern.Objects;
using Pattern.Business;
using Pattern.Configs;



public class SlotPrefab : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
{
    public Slot Slot { get; set; } = null;
    public SlotPrefab Upper { get; set; } = null;
    public BallPrefab Ball { get; set; } = null;
    private static bool activate = false;

    private void Awake() { }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!activate)
        {
            activate = true;
            //GameLogic.Instance.AddTrace(SlotNode);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //if (activate)
        //    GameLogic.Instance.AddTrace(SlotNode);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (activate && Input.touchCount == 0)
        {
            activate = false;
            //GameLogic.Instance.Dispose();
        }
    }
}
