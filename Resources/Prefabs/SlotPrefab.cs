using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Pattern.Objects;
using Pattern.Managers;
using Pattern.Configs;



public delegate void DELEGATE_T<T>(T t);

public class SlotPrefab : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
{
    public Slot Slot { get; set; } = null;
    public DELEGATE_T<SlotPrefab> Generate;
    public Action finishAction;

    private Action beginAction;
    private Action addAction;
    private Action removeAction;

    public BallPrefab Ball { get; set; } = null;
    
    private static bool activate = false;



    private void Awake() 
    {
        beginAction = LineManager.Instance.Begin;
        addAction = LineManager.Instance.Append;
        removeAction = LineManager.Instance.Remove;
    }

    public void Initialize(Slot slot, Action finishAction)
    {
        name = slot.Id.ToString();

        Slot = slot;
        Generate = null;
        GetComponent<Image>().raycastTarget = true;

        this.finishAction = finishAction;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!activate)
        {
            activate = true;
            PatternHandler.Instance.Begin(this, Slot.Color);
            beginAction?.Invoke();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (activate)
        {
            AddBall addBall = PatternHandler.Instance.Append(this);
            
            switch (addBall)
            {
                case AddBall.remove:
                    removeAction?.Invoke();
                    return;
                case AddBall.add:
                    addAction?.Invoke();
                    return;
                default:
                    return;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (activate && Input.touchCount == 0)
        {
            finishAction?.Invoke();
            activate = false;
        }
    }

    public void Dispose()
    {
        SlotPrefab upper = Ray.Instance.Shot(transform.position + CONST.DIRECTION_OFFSET[(int)ClockWise.up]);

        if (upper == null)
            Generate?.Invoke(this);

        if (upper != null ? upper.Ball : null != null)
        {
            upper.Ball.SendTo(this);
            upper.Ball = null;
        }
    }
}
