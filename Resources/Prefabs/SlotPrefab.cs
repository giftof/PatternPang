using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Pattern.Objects;
using Pattern.Business;



public class SlotPrefab : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
{
    public SlotNode SlotNode {get; set;}

    public void OnPointerDown(PointerEventData eventData)
        => GameLogic.Instance.AddTrace(SlotNode);

    public void OnPointerEnter(PointerEventData eventData)
        => GameLogic.Instance.AddTrace(SlotNode);

    public void OnPointerUp(PointerEventData eventData)
        => GameLogic.Instance.Dispose();
}
