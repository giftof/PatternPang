using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class SlotPrefab: MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler, IParent<BallPrefab> 
{
    private static Action m_beginAction;
    private static Action m_addAction;
    private static Action m_removeAction;
    private static PatternHandler m_patternHandler;

    public int id;
    public BallPrefab Child { get; set; } = null;
    public T_DELEGATE_T<bool, SlotPrefab> Generate;

    private void Awake() 
    {
        GetComponent<Image>().raycastTarget = true;
    }

    public void PunchScale()
        => Child.transform.DOPunchScale(Vector3.one * .2f, CONST.DURATION_WAIT_MATCH_BALL, 1, 1);

    public static PatternHandler SetPatternHandler 
    {
        set => m_patternHandler = value;
    }

    public static Action SetRemoveAction 
    {
        set => m_removeAction = value;
    }

    public static Action SetAddAction 
    {
        set => m_addAction = value;
    }

    public static Action SetBeginAction 
    {
        set => m_beginAction = value;
    }

    public bool IsNotSlot()
        => Child == null && Generate != null && Child.BallColor > SlotAttribute.color_count;

    public void OnPointerDown(PointerEventData eventData) 
    {
        if (m_patternHandler.Count() > 0)
            return;

        if (m_patternHandler.Begin(this).Equals(AddBall.add)) 
        {
            PunchScale();
            m_beginAction?.Invoke();
        }
    }

    public void OnPointerEnter(PointerEventData eventData) 
    {
        if (m_patternHandler.Count().Equals(0))
            return;
        
        switch (m_patternHandler.Append(this)) 
        {
            case AddBall.remove:
                m_removeAction?.Invoke();
                return;
            case AddBall.add: PunchScale();
                m_addAction?.Invoke();
                Vibrate.Do(CONST.DURATION_VIBRATE_ADD);
                return;
            default: return;
        }
    }

    public void OnPointerUp(PointerEventData eventData) 
    {
        if (m_patternHandler.Count() > 0 && Input.touchCount <= 1) 
        {
            m_patternHandler.InputEnd();
        }
    }
}
