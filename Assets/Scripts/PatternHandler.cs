using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PatternHandler {
    private LinkedList<SlotPrefab> m_selected;
    public PatternHandler() => m_selected = new LinkedList<SlotPrefab>();

    public Action InputEnd { get; set; } = null;

    public AddBall Begin(SlotPrefab slot) {
        if (slot == null || slot.Child == null || slot.Generate != null || m_selected.Count > 0 || slot.Child.BallColor > SlotAttribute.color_count) 
            return AddBall.none;
        
        m_selected.AddFirst(slot);
        return AddBall.add;
    }

    /* -- check sequence --
         *
        //  * if exception?        (yes : return begin),   (no : next)
         * if not near?         (yes : return null),    (no : next)
         * if roll-back?        (yes : return remove),  (no : next)
         * if new && sameColor? (yes : return add),     (no : next)
         * return null
         */
    public AddBall Append(SlotPrefab slot) {
        // /* exception check */
        // if (m_selected.Count == 0) 
        //     return Begin(slot);
        
        /* is near? */
        if (slot.Generate != null || Vector3.Distance(m_selected.First.Value.transform.position, slot.transform.position) > CONST.MAX_DISTANCE) 
            return AddBall.none;
        
        /* is roll-back? */
        if (m_selected.Count > 1 && slot.Equals(Ray.Instance.Shoot(m_selected.First.Next.Value.transform.position))) {
            m_selected.RemoveFirst();
            return AddBall.remove;
        }

        /* is NEW and SAME COLOR */
        if (m_selected.Last.Value.Child.BallColor.Equals(slot.Child.BallColor) && !m_selected.Contains(slot)) {
            m_selected.AddFirst(slot);
            return AddBall.add;
        }

        /* is near but duplicated slot or unmatch color */
        return AddBall.none;
    }

    public LinkedList<SlotPrefab> Selected() => m_selected;

    public SlotPrefab First() => m_selected.First.Value;

    public int Count() => m_selected.Count();

    public void Clear() => m_selected.Clear();

    public Vector3[] ShapeOffset() {
        if (m_selected.Count < CONST.MIN_SELECT) 
            return null;
        
        (SlotPrefab prev, SlotPrefab curr) = (null, null);
        return (
            from e in m_selected where(prev = curr) == curr && (curr = e) == e && prev != null select prev.transform.position - curr.transform.position
        ).ToArray();
    }
}
