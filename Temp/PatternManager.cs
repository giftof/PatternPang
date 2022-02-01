/*
 * Create: [PatternManager.cs] on Thu Jan 27 2022 5:11:54 PM
 * Author: [cloudeven@gmail.com]
 *
 * Copyright (c) 2022 [noname]
 */

using System;
using System.Linq;
using System.Collections.Generic;
using Pattern.Objects;
using Pattern.Configs;



namespace Pattern.Managers
{
    public class PatternManager
    {
        public LinkedList<SlotNode> m_selectedList;

        public static PatternManager Instance => m_instance.Value;
        private static readonly Lazy<PatternManager> m_instance = new Lazy<PatternManager>( () => new PatternManager() );
        private PatternManager() => m_selectedList = new LinkedList<SlotNode>();

        public void Add(SlotNode slotNode)
        {
UnityEngine.Debug.Log("ADD");
            if (!IsChain(slotNode))
                return;
            
            else if (IsTryRemove(slotNode))
                Remove();

            else if (IsSameColor(slotNode))
                m_selectedList.AddFirst(slotNode);
        }

        public void Clear()
            => m_selectedList.Clear();

        public int[] Shape()
        {
// UnityEngine.Debug.Log("SHAPE");
// foreach(var item in m_selectedList)
//     UnityEngine.Debug.Log($"id = {item.Id}, color = {item.Color}");

            if (m_selectedList.Count < CONST.MIN_SELECT)
                return null;

            List<int> list = new List<int>();
            LinkedListNode<SlotNode> current = m_selectedList.First;

            while (current.Next != null)
            {
                list.Add(current.Value.FindLinkIndex(current.Next.Value));
                current = current.Next;
            }

            return list.ToArray();
        }

        public SlotNode First
            => m_selectedList.First?.Value;

        /*
         * Privates
         */
        private void Remove()
        {
            if (m_selectedList.Count > 0)
                m_selectedList.RemoveFirst();
        }

        private bool IsChain(SlotNode slotNode) 
            => slotNode != null 
            && (IsFirst() || IsContact(slotNode));
        
        private bool IsSameColor(SlotNode slotNode)
            => IsFirst()
            || m_selectedList.First.Value.Color.Equals(slotNode.Color);

        private bool IsTryRemove(SlotNode slotNode)
            => m_selectedList.Count > 1
            && m_selectedList.First.Next.Value.Equals(slotNode);

        private bool IsFirst()
            => m_selectedList.Count == 0;

        private bool IsContact(SlotNode slotNode)
            => m_selectedList.First.Value.Link.FirstOrDefault(e => slotNode.Equals(e)) != null;
    }
}
