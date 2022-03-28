using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class SlotPrefab: MonoBehaviour,
IPointerDownHandler,
IPointerEnterHandler,
IPointerUpHandler,
IParent<BallPrefab> {
    private Action m_beginAction;
    private Action m_addAction;
    private Action m_removeAction;
    private DELEGATE_T<SlotPrefab> m_bombAction;
    private PatternHandler m_pattern;

    public int id;
    public BallPrefab Child { get; set; } = null;
    public T_DELEGATE_T<bool, SlotPrefab> Generate;
    public static bool Activate = false;

    private void Awake() {
        GetComponent<Image>().raycastTarget = true;
    }

    public void PunchScale()
        => Child.transform.DOPunchScale(Vector3.one * .2f, CONST.DURATION_WAIT_MATCH_BALL, 1, 1);

    public PatternHandler SetPatternHandler {
        set => m_pattern = value;
    }

    public Action SetRemoveAction {
        set => m_removeAction = value;
    }

    public Action SetAddAction {
        set => m_addAction = value;
    }

    public Action SetBeginAction {
        set => m_beginAction = value;
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (Activate)
            return;

        if (m_pattern.Begin(this).Equals(AddBall.add)) {
            PunchScale();
            m_beginAction?.Invoke();
            Activate = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (!Activate)
            return;
        
        switch (m_pattern.Append(this)) {
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

    public void OnPointerUp(PointerEventData eventData) {
        if (Activate && Input.touchCount <= 1) {
            m_pattern.InputEnd();
            Activate = false;
        }
    }
}
