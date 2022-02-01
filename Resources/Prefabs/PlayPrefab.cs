using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pattern.Configs;



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
            SlotPrefab current = item;
            while (current.Slot.Color != SlotAttribute.none)
                RecursiceRequest(current);
        }
    }



    private void RecursiceRequest(SlotPrefab slotPrefab)
    {

    }
}
