using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class LineManager: ManagedPool<LinePrefab> {
    private LinePrefab m_currentLine;
    private PatternHandler m_patternHandler;

    protected override void Awake() {
        base.Awake();
        pool.prefab = Resources.Load<GameObject>("Prefabs/_Manager/LinePrefab");
    }

    public PatternHandler SetPatternHandler {
        set => m_patternHandler = value;
    }

    public void Begin() {
        Clear();
        m_currentLine = Request(transform, default);
        Append();
    }

    public void Append() {
        m_currentLine.line.positionCount = m_patternHandler.Count();
        m_currentLine
            .line
            .SetPosition(
                m_patternHandler.Count() - 1,
                m_patternHandler.First().transform.position
            );
    }

    public void Remove() => m_currentLine.line.positionCount = m_patternHandler.Count();

    public void ToLine(List<SlotPrefab> list) {
        LinePrefab unit = Request(transform, default);

        unit.line.positionCount = list.Count;
        list
            .Select((e, index) => (e, index))
            .Where(pair => {
                unit
                    .line
                    .SetPosition(pair.index, pair.e.transform.position);
                return false;
            })
            .ToArray();
    }
}
