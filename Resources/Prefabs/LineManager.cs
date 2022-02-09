using System.Collections.Generic;
using UnityEngine;
using Pattern.Managers;


public class LineManager : MonoBehaviour
{
    [SerializeField] ObjectPool linePool;

    public static LineManager Instance = null;
    private LinkedList<LinePrefab> lineArray;
    LinePrefab currentLine;
    int jointCount;

    private void Awake()
    {
        lineArray = new LinkedList<LinePrefab>();
        Instance = this;
    }

    public void Begin()
    {
        Clear();

        SlotPrefab slot = PatternHandler.Instance.First();
        currentLine = linePool.Request<LinePrefab>();
        currentLine.transform.SetParent(transform);
        currentLine.transform.localScale = Vector3.one;
        currentLine.transform.localPosition = default;
        jointCount = 0;

        currentLine.line.positionCount = ++jointCount;
        currentLine.line.SetPosition(jointCount - 1, slot.transform.position);

        lineArray.AddFirst(currentLine);
    }

    public void Append()
    {
        currentLine.line.positionCount = ++jointCount;
        currentLine.line.SetPosition(jointCount - 1, PatternHandler.Instance.First().transform.position);
    }

    public void Remove()
    {
        currentLine.line.positionCount = --jointCount;
    }

    public void Clear()
    {
        if (lineArray == null)
            return;

        foreach (var item in lineArray)
            linePool.Release(item.gameObject);
        lineArray.Clear();
    }

    public void ToLine(List<SlotPrefab> list)
    {
        int joint = list.Count;
        LinePrefab unit = linePool.Request<LinePrefab>();
        unit.transform.SetParent(transform);
        unit.transform.localScale = Vector3.one;
        unit.transform.localPosition = default;

        unit.line.positionCount = joint;
        unit.line.SetPosition(0, list[0].transform.position);

        for (int i = 0; i < list.Count; ++i)
            unit.line.SetPosition(i, list[i].transform.position);

        lineArray.AddFirst(unit);
    }
}
