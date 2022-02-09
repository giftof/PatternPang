using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Pattern.Configs;



public class BallPrefab : MonoBehaviour
{
    public SlotAttribute m_color { get; set; }
    public static uint TweeningCount { get; set; } = 0; /* remove */
    [SerializeField] Image image;

    public SlotAttribute Color
    {
        get { return m_color; }
        set
        {
            m_color = value;
            image.color = ConvertToColor();
        }
    }

    public void SendTo(SlotPrefab destination)
    {
        //TweeningCount += 1;
        transform.DOMoveY(destination.transform.position.y, CONST.MOVE_DURATION).SetEase(Ease.Linear)
            .OnComplete(() => {
                destination.Child = this;
                destination.Slot.Color = Color;
                //TweeningCount -= 1;
            });
    }

    public void Drop<T>(DELEGATE_T<T> finishAction, T obj)
    {
        //TweeningCount += 1;
        transform.DOMoveY(transform.position.y - CONST.TEMP_DROP_DISTANCE, CONST.MOVE_DURATION * CONST.TEMP_DROP_DISTANCE).SetEase(Ease.Linear)
            .OnComplete(() => {
                finishAction?.Invoke(obj);
                //TweeningCount -= 1;
            });
    }

    private Color ConvertToColor()
    {
        return m_color switch
        {
            SlotAttribute.red => UnityEngine.Color.red,
            SlotAttribute.green => UnityEngine.Color.green,
            SlotAttribute.blue => UnityEngine.Color.blue,
            SlotAttribute.yellow => UnityEngine.Color.yellow,
            SlotAttribute.purple => UnityEngine.Color.cyan,
            SlotAttribute.bomb1 => UnityEngine.Color.black,
            SlotAttribute.bomb2 => UnityEngine.Color.black,
            SlotAttribute.bomb3 => UnityEngine.Color.black,
            _ => UnityEngine.Color.grey,
        };
    }
}
