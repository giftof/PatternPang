using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pattern.Configs;
using Pattern.Managers;



public class Ray : MonoBehaviour
{
    private void Awake()
    {
        PatternHandler.Instance.ray = this;
    }

    public SlotPrefab Shot(Vector3 position)
    {
        RaycastHit2D hit;

        hit = Physics2D.Raycast(position, transform.forward);

        if (hit && hit.transform.TryGetComponent(out SlotPrefab slotPrefab))
            return slotPrefab;
        return null;
    }
}
