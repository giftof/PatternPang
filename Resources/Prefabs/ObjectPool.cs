using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private Queue<GameObject> m_queue;

    public GameObject prefab;

    private void Awake()
        => m_queue = new Queue<GameObject>();

    public T Request<T>() where T : class
    {
        if (m_queue.Count > 0)
            return m_queue.Dequeue().GetComponent<T>();
        return Instantiate(prefab).GetComponent<T>();
    }

    public void Release(GameObject obj)
        => m_queue.Enqueue(obj);
}

