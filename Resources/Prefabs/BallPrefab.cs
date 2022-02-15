using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Pattern.Configs;

public class BallPrefab : MonoBehaviour
{
    [SerializeField] Image m_image;
    [SerializeField] Sprite[] m_spriteArray;

    private static Color[] m_palette = new Color[8] {UnityEngine.Color.red, UnityEngine.Color.green, UnityEngine.Color.blue,
        UnityEngine.Color.yellow, UnityEngine.Color.cyan,
        UnityEngine.Color.white, UnityEngine.Color.white, UnityEngine.Color.white};
    private SlotAttribute m_color { get; set; }

    public bool IsWorking { get; private set; }

    public SlotAttribute Color
    {
        get => m_color;
        set
        {
            m_color = value;
            m_image.color = ConvertToColor();
            m_image.sprite = ConvertImage();
        }
    }

    public void TransferTo(IParent<BallPrefab> destination)
    {
        if (IsWorking) { return; }

        IsWorking = true;

        if (destination is MonoBehaviour monoObj)
        {
            destination.Child = this;

            transform
                .DOMoveY(monoObj.transform.position.y, CONST.DURATION_MOVE)
                .SetEase(Ease.Linear)
                .OnComplete(() => {
                    //transform.DOShakeScale(CONST.DURATION_MOVE, CONST.DURATION_JELLY_ELASTICITY, 2);
                    IsWorking = false;
                });
        }
    }

    public void Drop<T>(DELEGATE_T<T> finishAction, T obj)
    {
        IsWorking = true;
        transform.DOMoveY(transform.position.y - CONST.TEMP_DROP_DISTANCE, CONST.DURATION_MOVE * CONST.TEMP_DROP_DISTANCE)
            .OnComplete(() => {
                finishAction?.Invoke(obj);
                IsWorking = false;
            });
    }

    public bool IsBomb()
        => Color > SlotAttribute.color_count;

    private Color ConvertToColor()
    {
        if (m_color < 0) { return UnityEngine.Color.grey; }
        return m_palette[(int)m_color];
    }

    private Sprite ConvertImage()
    {
        if (m_color < 0) { return m_spriteArray[0]; }
        if (m_color > SlotAttribute.color_count) { return m_spriteArray[(int)m_color - 1]; }
        return m_spriteArray[(int)m_color];
    }
}
