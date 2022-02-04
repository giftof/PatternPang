using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Pattern.Objects;
using Pattern.Managers;



public delegate void DELEGATE_T<T>(T t);

public class SlotPrefab : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
{
    public Slot Slot { get; set; } = null;
    public SlotPrefab Upper { get; set; } = null;
    public DELEGATE_T<SlotPrefab> Generate;
    public BallPrefab Ball { get; set; } = null;
    private static bool activate = false;

    private void Awake() { }

    public void Initialize(Slot slot)
    {
        name = slot.Id.ToString();
        Slot = slot;
        Generate = null;
        GetComponent<Image>().raycastTarget = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!activate)
        {
            activate = true;
            PatternHandler.Instance.Add(this, Slot.Color);
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
