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
    public SlotPrefab Upper { get; set; } = null;
    public DELEGATE_T<SlotPrefab> Generate;
    public Action finishAction;
    public Action beginAction;
    public Action addAction;
    public Action removeAction;
    public BallPrefab Ball { get; set; } = null;
    private static bool activate = false;

    private void Awake() { }

    public void Initialize(Slot slot, Action beginAction, Action addAction, Action removeAction, Action finishAction)
    {
        name = slot.Id.ToString();

        this.beginAction = beginAction;
        this.addAction = addAction;
        this.removeAction = removeAction;
        this.finishAction = finishAction;
        Slot = slot;
        Generate = null;
        GetComponent<Image>().raycastTarget = true;
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
            /* dispose selected and find same pattern */
            finishAction?.Invoke();
            activate = false;
        }
    }

    public void Dispose()
    {
        if (Upper == null)
            Generate?.Invoke(this);

        if (Upper != null ? Upper.Ball : null != null)
        {
            Upper.Ball.SendTo(this);
            Upper.Ball = null;
        }
    }
}
