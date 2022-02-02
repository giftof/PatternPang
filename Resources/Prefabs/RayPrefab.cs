using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pattern.Configs;
using Pattern.Managers;



public class RayPrefab : MonoBehaviour
{
    RaycastHit m_hit;
    float m_maxDistance = 1200;

    private void Awake()
    {
        PatternHandler.Instance.rayPrefab = this;
    }

    public SlotPrefab Shot(Vector3 position)
    {
        Debug.LogError("will shot debugray");
        Debug.DrawRay(position, Vector3.forward * m_maxDistance, Color.red, 1000);




        if (Physics.Raycast(position, transform.forward, out m_hit, m_maxDistance))
            if (m_hit.transform.TryGetComponent(out SlotPrefab slotPrefab))
                return slotPrefab;
        return null;
    }
}
