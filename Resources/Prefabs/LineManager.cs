using System.Collections.Generic;
using Pattern.Managers;

public class LineManager : ManagedPool<LinePrefab>
{
    private LinePrefab m_currentLine;
    private int m_jointCount;

    public static LineManager Instance = null;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    public void Begin()
    {
        Clear();
        m_currentLine = Request(transform, default);
        Append();
    }

    public void Append()
    {
        m_currentLine.line.positionCount = ++m_jointCount;
        m_currentLine.line.SetPosition(m_jointCount - 1, PatternHandler.Instance.First().transform.position);
    }

    public void Remove()
        => m_currentLine.line.positionCount = --m_jointCount;

    public override void Clear()
    {
        base.Clear();
        m_jointCount = 0;
    }

    public void ToLine(List<SlotPrefab> list)
    {
        int joint = list.Count;
        LinePrefab unit = Request(transform, default);

        unit.line.positionCount = joint;
        unit.line.SetPosition(0, list[0].transform.position);

        for (int i = 0; i < list.Count; ++i)
            unit.line.SetPosition(i, list[i].transform.position);
    }
}
