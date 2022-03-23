using System.Linq;
using System.Collections.Generic;
using Pattern.Managers;

public class LineManager : ManagedPool<LinePrefab>
{
    private LinePrefab m_currentLine;
    private int m_jointCount;
    private PatternHandler m_patternHandler;

    protected override void Awake()
        => base.Awake();

    public PatternHandler SetPatternHandler
    {
        set => m_patternHandler = value;
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
        m_currentLine.line.SetPosition(m_jointCount - 1, m_patternHandler.First().transform.position);
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

        list.Select((e, index) => (e, index)).Where(pair => {
            unit.line.SetPosition(pair.index, pair.e.transform.position);
            return false;
        }).ToArray();
    }
}
