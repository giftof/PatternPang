using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Pattern.Configs;

public class BallPrefab : MonoBehaviour
{
    [SerializeField] Image m_image;
    [SerializeField] Sprite[] m_spriteArray;

    private static Color[] m_palette = new Color[8] {Color.red, Color.green, Color.blue,
                                                    Color.yellow, Color.cyan,
                                                    Color.white, Color.white, Color.white};
    private SlotAttribute m_color { get; set; }

    public bool IsWorking { get; private set; }

    public SlotAttribute BallColor
    {
        get => m_color;
        set
        {
            m_color = value;
            m_image.color = ConvertToColor();
            m_image.sprite = ConvertImage();
        }
    }

    public Tweener TEST_Trans(IParent<BallPrefab> destination)
    {
        return DOTween.To(() => 0, x => _ = x, 0, 0);
    }

    public void TransferTo(IParent<BallPrefab> destination, Action callBack = null)
    {
        if (IsWorking) { return; }

        IsWorking = true;

        if (destination is MonoBehaviour monoObj)
        {
            destination.Child = this;

            transform
                .DOMoveY(monoObj.transform.position.y, CONST.DURATION_MOVE)
                .SetEase(Ease.Linear)
                .SetUpdate(true)
                .OnComplete(() => {
                    IsWorking = false;
                    callBack?.Invoke();
                });
        }
    }

    public void Drop<T>(DELEGATE_T<T> finishAction, T obj)
    {
        IsWorking = true;
        transform.DOMoveY(transform.position.y - CONST.TEMP_DROP_DISTANCE, CONST.DURATION_MOVE * CONST.TEMP_DROP_DISTANCE)
            .SetUpdate(true)
            .OnComplete(() => {
                finishAction?.Invoke(obj);
                IsWorking = false;
            });
    }

    public bool IsBomb()
        => BallColor > SlotAttribute.color_count;

    private Color ConvertToColor()
    {
        if (m_color < 0) { return Color.grey; }
        if (m_color > SlotAttribute.color_count) { return m_palette[(int)m_color - 1]; }
        return m_palette[(int)m_color];
    }

    private Sprite ConvertImage()
    {
        if (m_color < 0) { return m_spriteArray[0]; }
        if (m_color > SlotAttribute.color_count) { return m_spriteArray[(int)m_color - 1]; }
        return m_spriteArray[(int)m_color];
    }
}
