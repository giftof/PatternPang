/*
 * Create: [SlotManager.cs] on Wed Jan 26 2022 오후 4:40:32
 * Author: [cloudeven@gmail.com]
 *
 * Copyright (c) 2022 [noname]
 */


using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pattern.Objects;



namespace Pattern.Managers
{
    public class SlotManager
    {
        private SlotNode m_board;

        public static SlotManager Instance => m_instance.Value;
        private static readonly Lazy<SlotManager> m_instance = new Lazy<SlotManager>( () => new SlotManager() );
        private SlotManager() 
        {

        }

        public void CreateBoard(int width, int height)
        {
            SlotNode[] nodeArray = new SlotNode[width];

            nodeArray.Select( (node, index) => new {node, index} ).First(e => e.node == null)
            // m_board = new SlotNode();
            
            for (int w = 0; w < width; ++w)
            {
                for (int h = 0; h < height; ++h)
                {
                    if (m_board == null)
                        m_board = new SlotNode();
                    else
                    {
                        m_board.Link[0] = new SlotNode();
                        m_board.Link[0].Link[(int)ClockWise.count / 2] = m_board;
                        m_board = m_board.Link[0];
                    }
                    // if (w == 0 || w == width - 1)
                    // {
                    //     m_board.Link[0] = new SlotNode();
                    // }
                    // else if (w == width - 1)
                    // {
                    //     m_board.Link[0] = new SlotNode();
                    // }
                    // else
                    // {

                    // }
                }
            }
        }


    }



    public class PatternManager
    {
        private LinkedList<SlotNode> m_selectedList;
        private static SlotNode s_lastSlotNode;

        public static PatternManager Instance => m_instance.Value;
        private static readonly Lazy<PatternManager> m_instance = new Lazy<PatternManager>( () => new PatternManager() );
        private PatternManager() 
        {
            m_selectedList = new LinkedList<SlotNode>();
        }

        public void Add(SlotNode slotNode)
        {
            if (m_selectedList.Count.Equals(0) 
            || (CompareColor(slotNode) && IsNear(slotNode)))
                m_selectedList.AddFirst(slotNode);
            
            if (IsCancel(slotNode))
                Remove();

            s_lastSlotNode = slotNode;
        }

        public void Remove()
        {
            if (m_selectedList.Count > 0)
                m_selectedList.RemoveFirst();
        }

        public void Clear() => m_selectedList.Clear();

        public int[] Shape()
        {
            List<int> list = new List<int>();
            LinkedListNode<SlotNode> current = m_selectedList.First;

            while (current.Next != null)
            {
                list.Add(current.Value.FindLinkIndex(current.Next.Value));
                current = current.Next;
            }

            return list.ToArray();
        }

        /*
         * Privates
         */
        private bool IsCancel(SlotNode slotNode) => m_selectedList.Count > 1 && m_selectedList.First.Value.Equals(s_lastSlotNode);
        private bool CompareColor(SlotNode slotNode) => m_selectedList.First.Value.Color.Equals(slotNode.Color);
        private bool IsNear(SlotNode slotNode) => m_selectedList.First.Value.Link.FirstOrDefault(e => e.Equals(slotNode)) != null;
    }
}
