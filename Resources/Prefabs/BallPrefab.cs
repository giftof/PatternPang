using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Pattern.Configs;
using Pattern.Managers;



public class BallPrefab : MonoBehaviour
{
    public static uint TweeningCount { get; set; } = 0;

    public void SendTo(SlotPrefab destination)
    {
        transform.DOMoveY(destination.transform.position.y, CONST.MOVE_DURATION).SetEase(Ease.Linear)
            .OnStart(() => {
                destination.Upper.Ball = null;
                TweeningCount += 1;
            })
            .OnComplete(() => {
                destination.Ball = this;
                TweeningCount -= 1;
            });
    }
}
