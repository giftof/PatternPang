using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PatternHandler 
{
    private LinkedList<SlotPrefab> m_selected;
    public PatternHandler() => m_selected = new LinkedList<SlotPrefab>();

    public Action InputEnd { get; set; } = null;

    public AddBall Begin(SlotPrefab slot) 
    {
        if (slot?.IsNotSlot() ?? true || m_selected.Count > 0)
            return AddBall.none;
        
        m_selected.AddFirst(slot);
        return AddBall.add;
    }

    /* -- check sequence --
     *
     * if not near?         (yes : return null),    (no : next)
     * if roll-back?        (yes : return remove),  (no : next)
     * if new && sameColor? (yes : return add),     (no : next)
     * return null
     */
    public AddBall Append(SlotPrefab slot) 
    {
        /* is near? */
        if (slot.Generate != null || IsFar(slot.transform.position))
            return AddBall.none;
        /* is roll-back? */
        if (m_selected.Count > 1 && slot.Equals(Ray.Instance.Shoot(m_selected.First.Next.Value.transform.position))) 
        {
            m_selected.RemoveFirst();
            return AddBall.remove;
        }
        /* is NEW and SAME COLOR */
        if (m_selected.Last.Value.Child.BallColor.Equals(slot.Child.BallColor) && !m_selected.Contains(slot)) 
        {
            m_selected.AddFirst(slot);
            return AddBall.add;
        }
        /* is near but duplicated slot or unmatch color */
        return AddBall.none;
    }

    private bool IsFar(Vector3 other)
        => Vector3.Distance(m_selected.First.Value.transform.position, other) > CONST.MAX_DISTANCE;

    public LinkedList<SlotPrefab> Selected()
        => m_selected;

    public SlotPrefab First()
        => m_selected.First.Value;

    public int Count()
        => m_selected.Count();

    public void Clear()
        => m_selected.Clear();

    public Vector3[] ShapeOffset() 
    {
        if (m_selected.Count < CONST.MIN_SELECT) 
            return null;
        
        (SlotPrefab prev, SlotPrefab curr) = (null, null);
        return (from e in m_selected
                where(prev = curr) == curr && (curr = e) == e && prev != null
                select prev.transform.position - curr.transform.position).ToArray();
    }
}
