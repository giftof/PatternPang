using System;
using System.Collections.Generic;
using UnityEngine;
using Pattern.Configs;



namespace Pattern.Managers
{
    public class PatternHandler
    {
        public Action InputEnd { get; set; } = null;

        private LinkedList<SlotPrefab> m_selected;
        private SlotAttribute m_fixedAttribute = SlotAttribute.none;

        public static PatternHandler Instance => m_instance.Value;

        private static readonly Lazy<PatternHandler> m_instance = new Lazy<PatternHandler>(() => new PatternHandler());
        private PatternHandler() => m_selected = new LinkedList<SlotPrefab>();



        public void Begin(SlotPrefab slot, SlotAttribute attribute)
        {
            if (m_fixedAttribute.Equals(SlotAttribute.none))
            {
                m_selected.AddFirst(slot);
                m_fixedAttribute = attribute;
            }
            else if (m_fixedAttribute.Equals(attribute))
                m_selected.AddFirst(slot);
        }

        public AddBall Append(SlotPrefab target)
        {
            if (m_selected.Count > 1)
            {
                if (target.Equals(Ray.Instance.Shot(m_selected.First.Next.Value.transform.position)))
                {
                    m_selected.RemoveFirst();
                    return AddBall.remove;
                }
            }

            if (m_fixedAttribute.Equals(target.Slot.Color)
                && Vector3.Distance(m_selected.First.Value.transform.position, target.transform.position) < CONST.MAX_DISTANCE
                && !m_selected.Contains(target))
            {
                m_selected.AddFirst(target);
                return AddBall.add;
            }

            return AddBall.none;
        }

        public SlotPrefab First()
            => m_selected.First.Value;

        public void Clear()
        {
            m_selected.Clear();
            m_fixedAttribute = SlotAttribute.none;
        }

        public Vector3[] ShapeOffset()
        {
            if (m_selected.Count < CONST.MIN_SELECT)
                return null;

            LinkedListNode<SlotPrefab> node = m_selected.First;
            List<Vector3> list = new List<Vector3>();

            for (int i = 0; i < m_selected.Count - 1; ++i)
            {
                list.Add(node.Value.transform.position - node.Next.Value.transform.position);
                node = node.Next;
            }

            return list.ToArray();
        }
    }
}
