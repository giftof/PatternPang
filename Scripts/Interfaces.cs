using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pattern.Configs;

public interface IParent<T> where T : MonoBehaviour
{
    public T Child { get; set; }
}
