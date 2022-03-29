using UnityEngine;

public interface IParent<T > where T :MonoBehaviour {
    public T Child { get; set; }
}
