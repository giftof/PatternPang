using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;
    private Queue<GameObject> queue;

    private void Awake()
        => queue = new Queue<GameObject>();

    public T Request<T>() where T : class
    {
        if (queue.Count > 0)
            return queue.Dequeue().GetComponent<T>();
        return Object.Instantiate(prefab).GetComponent<T>();
    }

    public void Release(GameObject obj)
        => queue.Enqueue(obj);
}

