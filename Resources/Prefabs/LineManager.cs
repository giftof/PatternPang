using System.Collections.Generic;
using UnityEngine;
using Pattern.Managers;


public class LineManager : MonoBehaviour
{
    [SerializeField] ObjectPool linePool;

    public static LineManager Instance;
    private LinkedList<LinePrefab> lineArray;

    private void Awake() => Instance = this;

    public void Begin()
        => lineArray = new LinkedList<LinePrefab>();

    public void Append()
    {
        (SlotPrefab begin, SlotPrefab end) = PatternHandler.Instance.LastLine();
        LinePrefab unit = linePool.Request<LinePrefab>(transform);
        unit.Position((begin.transform.position, end.transform.position));
        lineArray.AddFirst(unit);
    }

    public void Remove()
    {
        linePool.Release(lineArray.First.Value.gameObject);
        lineArray.RemoveFirst();
    }

    public void Clear()
    {
        foreach (var item in lineArray)
            linePool.Release(item.gameObject);
        lineArray.Clear();
    }
}
