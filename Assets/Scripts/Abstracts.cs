using System.Collections.Generic;
using UnityEngine;

public delegate void DELEGATE_T<T>(T t);
public delegate T1 T_DELEGATE_T<T1, T2>(T2 t2);

public abstract class ManagedPool<T> : MonoBehaviour where T : MonoBehaviour {
    public ObjectPool pool;
    protected Dictionary<int, T> dictionary;

    protected virtual void Awake() {
        dictionary = new Dictionary<int, T>();
        pool = (new GameObject("objectPool")).AddComponent<ObjectPool>();
        pool.transform.SetParent(transform);
    }

    public virtual void Release(T target) {
        dictionary.Remove(target.GetInstanceID());
        pool.Release(target.gameObject);
        target
            .gameObject
            .SetActive(false);
    }

    public virtual T Request() {
        T obj = pool.Request<T>();
        obj
            .gameObject
            .SetActive(true);
        dictionary.Add(obj.GetInstanceID(), obj);
        return obj;
    }

    public T Request(Transform parent, Vector3 localPosition = default) {
        T obj = Request();

        obj
            .transform
            .SetParent(parent);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = localPosition;

        return obj;
    }

    public virtual void Clear() {
        foreach(var pair in dictionary) {
            pool.Release(pair.Value.gameObject);
            pair
                .Value
                .gameObject
                .SetActive(false);
        }
        dictionary.Clear();
    }
}
