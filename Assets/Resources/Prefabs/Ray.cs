using UnityEngine;

public class Ray : MonoBehaviour
{
    public static Ray Instance;

    private void Awake()
        => Instance = this;

    public SlotPrefab Shot(Vector3 position)
    {
        RaycastHit2D hit;

        hit = Physics2D.Raycast(position, transform.forward);

        if (hit && hit.transform.TryGetComponent(out SlotPrefab slotPrefab))
            return slotPrefab;
        return null;
    }
}
