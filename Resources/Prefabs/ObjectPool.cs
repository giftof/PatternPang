using System.Collections.Generic;
using UnityEngine;



public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;
    private Queue<GameObject> queue;

    private void Awake()
        => queue = new Queue<GameObject>();

    public T Request<T>(Transform parent = null, Vector3 position = default) where T : class
    {
        GameObject obj;

        if (queue.Count > 0)
        {
            obj = queue.Dequeue();
            obj.SetActive(true);
        }
        else
            obj = Instantiate(prefab);

        obj.transform.SetParent(parent ?? transform);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = position;

        return obj.TryGetComponent(out T result) ? result : null;
    }

    public void Release(GameObject obj)
    {
        queue.Enqueue(obj);
        obj.transform.SetParent(transform);
        obj.SetActive(false);
    }
}
