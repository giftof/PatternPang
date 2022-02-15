using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Pattern.Managers;
using Pattern.Configs;

public class SlotPrefab : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler, IPointerClickHandler, IParent<BallPrefab>/*, IAttributedParent<BallPrefab>*/
{
    private Action m_beginAction;
    private Action m_addAction;
    private Action m_removeAction;
    private DELEGATE_T<SlotPrefab> m_bombAction;
    private Vibrate m_vibrate = Vibrate.Instance;
    private PatternHandler m_pattern = PatternHandler.Instance;

    public BallPrefab Child { get; set; } = null;
    public T_DELEGATE_T<bool, SlotPrefab> Generate;
    public static bool Activate = false;

    private void Awake() 
    {
        m_beginAction = LineManager.Instance.Begin;
        m_addAction = LineManager.Instance.Append;
        m_removeAction = LineManager.Instance.Remove;
        m_bombAction = GameLogic.Instance.Bomb;

        GetComponent<Image>().raycastTarget = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Activate) { return; }

        if (m_pattern.Begin(this).Equals(AddBall.add))
        {
            m_beginAction?.Invoke();
            Activate = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Activate) { return; }

        switch (m_pattern.Append(this))
        {
            case AddBall.remove:
                //Vibrate.Do(CONST.DURATION_VIBRATE_REMOVE);
                m_removeAction?.Invoke();
                return;
            case AddBall.add:
                m_addAction?.Invoke();
                m_vibrate.Do(CONST.DURATION_VIBRATE_ADD);
                return;
            default:
                return;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Activate && Input.touchCount <= 1)
        {
            m_pattern?.InputEnd?.Invoke();
            Activate = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!Activate)
        {
            m_bombAction?.Invoke(this);
            m_vibrate.Do(CONST.DURATION_VIBRATE_ADD);
        }
    }
}
