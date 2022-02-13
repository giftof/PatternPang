using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Pattern.Managers;
using Pattern.Configs;

public class SlotPrefab : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler, IPointerClickHandler, IParent<BallPrefab>/*, IAttributedParent<BallPrefab>*/
{
    public BallPrefab Child { get; set; } = null;
    public T_DELEGATE_T<bool, SlotPrefab> Generate;

    private Action beginAction;
    private Action addAction;
    private Action removeAction;
    private static bool activate = false;

    private void Awake() 
    {
        beginAction = LineManager.Instance.Begin;
        addAction = LineManager.Instance.Append;
        removeAction = LineManager.Instance.Remove;

        GetComponent<Image>().raycastTarget = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (activate)
            return;

        if (PatternHandler.Instance.Begin(this).Equals(AddBall.add))
        {
            beginAction?.Invoke();
            activate = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!activate)
            return;

        switch (PatternHandler.Instance.Append(this))
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

    public void OnPointerUp(PointerEventData eventData)
    {
        if (activate && Input.touchCount <= 1)
        {
            PatternHandler.Instance?.InputEnd?.Invoke();
            activate = false;
        }
    }

    /* make simple */
    public bool Dispose()
    {
        SlotPrefab upper = Ray.Instance.Shot(transform.position + CONST.DIRECTION_OFFSET[(int)ClockWise.up]);

        if (Child?.IsWorking ?? false)
            return true;

        if (upper == null)
            return Generate?.Invoke(this) ?? false;
        else
        {
            if (Child == null)
            {
                if (upper.Child != null && !upper.Child.IsWorking)
                {
                    upper.Child.TransferTo(this);
                    upper.Child = null;
                }
                return upper.Dispose();
            }
        }
        return upper.Dispose();
    }

    /* not yet */
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("not implement yet");
    }
}
