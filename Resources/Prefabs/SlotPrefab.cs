using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Pattern.Objects;
using Pattern.Configs;
using Pattern.Managers;
using DG.Tweening;



public delegate void DELEGATE_T<T>(T t);

public class SlotPrefab : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
{
    public Slot Slot { get; set; } = null;
    public SlotPrefab Upper { get; set; } = null;
    public DELEGATE_T<SlotPrefab> Generate;
    private static bool activate = false;
    private BallPrefab m_ball { get; set; } = null;

    private void Awake() { }

    public BallPrefab Ball
    {
        get { return m_ball; }
        set
        {
            Slot.Color = (SlotAttribute)UnityEngine.Random.Range(1, (int)SlotAttribute.count - CONST.LEVEL1);
            m_ball = value;
            m_ball.GetComponent<Image>().color = ConvertToColor();
        }
    }

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
            Dispose();
            //Disposer?.Invoke(this);
        }
    }

    public void Dispose()
    {
        if (Upper == null)
        {
            Generate?.Invoke(this);
            return;
        }

        if (Ball == null && Upper.Ball != null)
            Upper.Ball.SendTo(this);

        Upper?.Dispose();
    }

    private Color ConvertToColor()
    {
        return Slot.Color switch
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
}

//public static class SlotPrefabExtension
//{
//    public static void Dispose(this SlotPrefab me)
//    {

//    }
//}