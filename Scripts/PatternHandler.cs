using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pattern.Objects;
using Pattern.Configs;



namespace Pattern.Managers
{
    public class PatternHandler
    {
        private LinkedList<Vector3> m_selectedPosition;
        private SlotAttribute? m_fixedAttribute = null;

        public RayPrefab rayPrefab;
        public static PatternHandler Instance => m_instance.Value;
        private static readonly Lazy<PatternHandler> m_instance = new Lazy<PatternHandler>(() => new PatternHandler());
        private PatternHandler() => m_selectedPosition = new LinkedList<Vector3>();

        public void Add(Vector3 position, SlotAttribute attribute)
        {
            if (m_fixedAttribute == null)
            {
                m_selectedPosition.AddFirst(position);
                m_fixedAttribute = attribute;
            }
            else if (m_fixedAttribute.Equals(attribute))
                m_selectedPosition.AddFirst(position);
        }

        public void AddChecker(SlotPrefab target)
        {
            if (m_selectedPosition.Count > 1)
            {
                if (rayPrefab.Shot(m_selectedPosition.First.Next.Value).Equals(target))
                {
                    Remove();
                    return;
                }
            }

            if (m_fixedAttribute.Equals(target.Slot.Color)
                && Vector3.Distance(m_selectedPosition.First.Value, target.transform.localPosition) < CONST.MAX_DISTANCE)
                m_selectedPosition.AddFirst(target.transform.localPosition);
        }

        public void Remove()
            => m_selectedPosition.RemoveFirst();

        public void Clear()
        {
            m_selectedPosition.Clear();
            m_fixedAttribute = null;
        }

        public Vector3[] ShapeOffset()
        {
            if (m_selectedPosition.Count < CONST.MIN_SELECT)
                return null;

            LinkedListNode<Vector3> node = m_selectedPosition.First;
            List<Vector3> list = new List<Vector3>();

            for (int i = 0; i < m_selectedPosition.Count - 1; ++i)
            {
                list.Add(node.Value - node.Next.Value);
                node = node.Next;
            }

            return list.ToArray();
        }
    }
}
