using UnityEngine;



public class Ray : MonoBehaviour
{
    public static Ray Instance;

    private void Awake()
        => Instance = this;

    public SlotPrefab Shot(Vector3 position)
    {
        RaycastHit2D hit;

Debug.Log($"position in ray = {position}");

        hit = Physics2D.Raycast(position, transform.forward);

        if (hit && hit.transform.TryGetComponent(out SlotPrefab slotPrefab))
        {
Debug.LogWarning($"hit in ray = {slotPrefab}");
            return slotPrefab;
        }
Debug.Log("hit is NULL!");
        return null;
    }
}
