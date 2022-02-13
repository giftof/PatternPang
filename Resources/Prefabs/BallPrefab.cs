using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Pattern.Configs;

public class BallPrefab : MonoBehaviour
{
    [SerializeField] Image image;

    private static Color[] m_palette = new Color[8] {UnityEngine.Color.red, UnityEngine.Color.green, UnityEngine.Color.blue,
        UnityEngine.Color.yellow, UnityEngine.Color.cyan,
        UnityEngine.Color.black, UnityEngine.Color.black, UnityEngine.Color.black};

    private SlotAttribute m_color { get; set; }
    public bool IsWorking { get; private set; }

    public SlotAttribute Color
    {
        get => m_color;
        set
        {
            m_color = value;
            image.color = ConvertToColor();
        }
    }

    public void TransferTo(IParent<BallPrefab> destination)
    {
        if (IsWorking)
            return;

        IsWorking = true;

        if (destination is MonoBehaviour monoObj)
        {
            destination.Child = this;

            transform
                .DOMoveY(monoObj.transform.position.y, CONST.MOVE_DURATION)
                .SetEase(Ease.Linear)
                .OnComplete(() => {
                    IsWorking = false;
                });
        }
    }

    public void Drop<T>(DELEGATE_T<T> finishAction, T obj)
    {
        IsWorking = true;
        transform.DOMoveY(transform.position.y - CONST.TEMP_DROP_DISTANCE, CONST.MOVE_DURATION * CONST.TEMP_DROP_DISTANCE)
            .OnComplete(() => {
                finishAction?.Invoke(obj);
                IsWorking = false;
            });
    }

    private Color ConvertToColor()
    {
        if (m_color < 0)
            return UnityEngine.Color.grey;
        return m_palette[(int)m_color];
    }
}
