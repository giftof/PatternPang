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
    private DELEGATE_T<SlotPrefab> bombAction;
    public static bool Activate = false;

    private void Awake() 
    {
        beginAction = LineManager.Instance.Begin;
        addAction = LineManager.Instance.Append;
        removeAction = LineManager.Instance.Remove;
        bombAction = GameLogic.Instance.Bomb;

        GetComponent<Image>().raycastTarget = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Activate) { return; }

        if (PatternHandler.Instance.Begin(this).Equals(AddBall.add))
        {
            beginAction?.Invoke();
            Activate = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Activate) { return; }

        switch (PatternHandler.Instance.Append(this))
        {
            case AddBall.remove:
                //Vibrate.Do(CONST.DURATION_VIBRATE_REMOVE);
                removeAction?.Invoke();
                return;
            case AddBall.add:
                addAction?.Invoke();
                Vibrate.Do(CONST.DURATION_VIBRATE_ADD);
                return;
            default:
                return;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Activate && Input.touchCount <= 1)
        {
            PatternHandler.Instance?.InputEnd?.Invoke();
            Activate = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!Activate)
        {
            bombAction?.Invoke(this);
            Vibrate.Do(CONST.DURATION_VIBRATE_ADD);
        }
    }
}
