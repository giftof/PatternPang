using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pattern.Configs;



public class RayPrefab : MonoBehaviour
{
    RaycastHit m_hit;
    float m_maxDistance = 1000;

    public SlotPrefab Shot(Vector3 position)
    {
        if (Physics.Raycast(position, transform.forward, out m_hit, m_maxDistance))
            if (m_hit.transform.TryGetComponent(out SlotPrefab slotPrefab))
                return slotPrefab;
        return null;
    }
}
