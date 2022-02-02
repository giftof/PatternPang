using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pattern.Configs;
using DG.Tweening;



public class PlayPrefab : MonoBehaviour
{
    [SerializeField] ObjectPool ballPool;
    public SlotPrefab[] BaseLine { get; set; } = null;
    public SlotPrefab[] Generator { get; set; } = null;
    private BallPrefab[] m_ballArray = null;

    public void Request()
    {
        foreach (var item in BaseLine)
        {
            RecursiceRequest(item);
            //SlotPrefab current = item;
            //while (current.Slot.Color != SlotAttribute.none)
            //    RecursiceRequest(current);
        }
    }



    private void RequestBall(SlotPrefab slotPrefab)
    {
        if (slotPrefab.Upper != null)
        {
            if (slotPrefab.Slot.Color == SlotAttribute.none
                && slotPrefab.Upper.Slot.Color != SlotAttribute.none)
                slotPrefab.Upper.Ball.SendTo(slotPrefab);
                
            //RequestBall(slotPrefab.Upper);
        }
    }

    private void RecursiceRequest(SlotPrefab slotPrefab)
    {
        while (slotPrefab.Upper != null
            && slotPrefab.Slot.Color != SlotAttribute.none)
            slotPrefab = slotPrefab.Upper;

        //while (slotPrefab.Upper != null)


    }
}
