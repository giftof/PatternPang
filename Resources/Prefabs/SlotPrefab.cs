using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Pattern.Objects;
using Pattern.Managers;
using Pattern.Configs;



public delegate void DELEGATE_T<T>(T t);

public class SlotPrefab : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler, IPointerClickHandler, IParent<BallPrefab>
{
    public Slot Slot { get; set; } = null;
    public DELEGATE_T<SlotPrefab> Generate;
    public Action FinishAction { get; set; } = null;

    private Action beginAction;
    private Action addAction;
    private Action removeAction;

    public BallPrefab Child { get; set; } = null;
    
    private static bool activate = false;



    private void Awake() 
    {
        beginAction = LineManager.Instance.Begin;
        addAction = LineManager.Instance.Append;
        removeAction = LineManager.Instance.Remove;
    }

    public void Initialize(Slot slot)
    {
        //name = slot.Id.ToString();

        Slot = slot;
        Generate = null;
        GetComponent<Image>().raycastTarget = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!activate)
        {
            activate = true;
            PatternHandler.Instance.Begin(this);
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
        if (activate && Input.touchCount <= 1)
        {
            PatternHandler.Instance?.InputEnd?.Invoke();
            FinishAction?.Invoke();
            activate = false;
        }
    }

    public void Dispose()
    {
        SlotPrefab upper = Ray.Instance.Shot(transform.position + CONST.DIRECTION_OFFSET[(int)ClockWise.up]);

        if (upper == null)
            Generate?.Invoke(this);

        if (upper != null ? upper.Child : null != null)
        {
            upper.Child.SendTo(this);
            upper.Child = null;
        }
    }

    /* not yet */
    public void OnPointerClick(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }
}
