using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Pattern.Objects;
using Pattern.Business;



public class SlotPrefab : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
{
    public SlotNode SlotNode {get; set;}
    public BallPrefab Ball {get; set;}
    private static bool activate = false;
    public Action FillAction {get; set;} = null;
    


    public void OnPointerDown(PointerEventData eventData)
    {
        if (!activate)
        {
            activate = true;
            GameLogic.Instance.AddTrace(SlotNode);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (activate)
            GameLogic.Instance.AddTrace(SlotNode);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (activate && Input.touchCount == 0)
        {
            activate = false;
            GameLogic.Instance.Dispose();
            FillAction?.Invoke();
        }
    }
}
