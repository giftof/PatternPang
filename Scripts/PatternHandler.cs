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
        private LinkedList<SlotPrefab> m_selected;
        private SlotAttribute m_fixedAttribute = SlotAttribute.none;

        public RayPrefab rayPrefab;
        public static PatternHandler Instance => m_instance.Value;
        private static readonly Lazy<PatternHandler> m_instance = new Lazy<PatternHandler>(() => new PatternHandler());
        private PatternHandler() => m_selected = new LinkedList<SlotPrefab>();

        public void Add(SlotPrefab slot, SlotAttribute attribute)
        {
            if (m_fixedAttribute.Equals(SlotAttribute.none))
            {
                m_selected.AddFirst(slot);
                m_fixedAttribute = attribute;
            }
            else if (m_fixedAttribute.Equals(attribute))
                m_selected.AddFirst(slot);
        }

        public void AddChecker(SlotPrefab target)
        {
            if (m_selected.Count > 1)
            {
Debug.Log($"ray result = {rayPrefab.Shot(m_selected.First.Next.Value.transform.position)}");
                if (target.Equals(rayPrefab.Shot(m_selected.First.Next.Value.transform.position)))
                {
                    Remove();
                    return;
                }
            }

            if (m_fixedAttribute.Equals(target.Slot.Color)
                && Vector3.Distance(m_selected.First.Value.transform.position, target.transform.position) < CONST.MAX_DISTANCE)
                m_selected.AddFirst(target);


            /* test code */
            Debug.Log($"selected count = {m_selected.Count}");
        }

        public void Remove()
            => m_selected.RemoveFirst();


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
