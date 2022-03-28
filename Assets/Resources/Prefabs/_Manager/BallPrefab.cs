using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BallPrefab: MonoBehaviour {
    [SerializeField] Image m_image;
    [SerializeField] Sprite[] m_spriteArray;

    private static Color[] m_palette = new Color[8]{
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
        Color.cyan,
        Color.white,
        Color.white,
        Color.white
    };
    private SlotAttribute m_color {
        get;
        set;
    }

    public SlotAttribute BallColor {
        get => m_color;
        set {
            m_color = value;
            m_image.color = ConvertToColor();
            m_image.sprite = ConvertImage();
        }
    }

    public Tweener MoveTo(IParent<BallPrefab> destination) {
        DOTween.Kill(GetInstanceID());
        MonoBehaviour dest = destination as MonoBehaviour;
        float duration = (transform.position.y - dest.transform.position.y) / CONST.OFFSET * CONST.DURATION_MOVE;
        destination.Child = this;

        return transform
            .DOMoveY(dest.transform.position.y, duration)
            .SetEase(Ease.Linear);
    }

    public void Drop<T>(DELEGATE_T<T> finishAction, T obj) {
        transform
            .DOMoveY(
                transform.position.y - CONST.TEMP_DROP_DISTANCE,
                CONST.DURATION_MOVE * CONST.TEMP_DROP_DISTANCE
            )
            .SetUpdate(true)
            .OnComplete(() => {
                finishAction
                    ?.Invoke(obj);
            });
    }

    // public bool IsBomb() => BallColor > SlotAttribute.color_count;

    private Color ConvertToColor() {
        if (m_color < 0) {
            return Color.grey;
        }
        // if (m_color > SlotAttribute.color_count) {
        //     return m_palette[(int)m_color - 1];
        // }
        return m_palette[(int)m_color];
    }

    private Sprite ConvertImage() {
        if (m_color < 0) {
            return m_spriteArray[0];
        }
        // if (m_color > SlotAttribute.color_count) {
        //     return m_spriteArray[(int)m_color - 1];
        // }
        return m_spriteArray[(int)m_color];
    }
}
