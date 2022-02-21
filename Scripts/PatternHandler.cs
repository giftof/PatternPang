using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Pattern.Configs;

namespace Pattern.Managers
{
    public class PatternHandler
    {
        private LinkedList<SlotPrefab> m_selected;
        public PatternHandler()
            => m_selected = new LinkedList<SlotPrefab>();

        public Action InputEnd { get; set; } = null;

        public AddBall Begin(SlotPrefab slot)
        {
            if (slot == null || slot.Child == null || slot.Generate != null || m_selected.Count > 0 || slot.Child.BallColor > SlotAttribute.color_count)
                return AddBall.none;

            m_selected.AddFirst(slot);
            return AddBall.add;
        }

        /* -- check sequence --
         * 
         * if exception?        (yes : return begin),   (no : next)
         * if not near?         (yes : return null),    (no : next)
         * if roll-back?        (yes : return remove),  (no : next)
         * if new && sameColor? (yes : return add),     (no : next)
         * return null
         */
        public AddBall Append(SlotPrefab target)
        {
            /* exception check */
            if (m_selected.Count == 0)
                return Begin(target);

            /* is near? */
            if (target.Generate != null
                || Vector3.Distance(m_selected.First.Value.transform.position, target.transform.position) > CONST.MAX_DISTANCE)
                return AddBall.none;

            /* is roll-back? */
            if (m_selected.Count > 1
                && target.Equals(Ray.Instance.Shot(m_selected.First.Next.Value.transform.position)))
            {
                m_selected.RemoveFirst();
                return AddBall.remove;
            }

            /* is NEW and SAME COLOR */
            if (m_selected.Last.Value.Child.BallColor.Equals(target.Child.BallColor)
                && !m_selected.Contains(target))
            {
                m_selected.AddFirst(target);
                return AddBall.add;
            }

            /* is near but duplicated slot or unmatch color */
            return AddBall.none;
        }

        public LinkedList<SlotPrefab> Selected()
            => m_selected;

        public SlotPrefab First()
            => m_selected.First.Value;

        public void Clear()
            => m_selected.Clear();

        public Vector3[] ShapeOffset()
        {
            Vector3[] offset = null;

            if (m_selected.Count >= CONST.MIN_SELECT)
            {
                (SlotPrefab prev, SlotPrefab curr) = (null, null);

                offset = m_selected
                    .Where(e => {
                        prev = curr;
                        curr = e;
                        return prev != null;
                    })
                    .Select(e => prev.transform.position - curr.transform.position)
                    .ToArray();
            }

            Clear();
            return offset;
        }
    }
}
