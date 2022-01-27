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
        public SlotNode Board { get; set; }

        public static SlotManager Instance => m_instance.Value;
        private static readonly Lazy<SlotManager> m_instance = new Lazy<SlotManager>( () => new SlotManager() );
        private SlotManager() { }

        public void CreateBoard(int width, int height)
        {
            SlotNode[] nodeArray = new SlotNode[width];

            for (int w = 0; w < width; ++w)
                nodeArray[w] = VerticalSuture(height, (int)ClockWise.up, (int)ClockWise.down);

            for (int w = 0; w < width - 1; ++w)
            {
                if (w % 2 == 0)
                    ZigzagSuture(nodeArray[w], nodeArray[w + 1], ((int)ClockWise.upRight, (int)ClockWise.downRight), ((int)ClockWise.downLeft, (int)ClockWise.upLeft));
                else
                    ZigzagSuture(nodeArray[w], nodeArray[w + 1], ((int)ClockWise.downRight, (int)ClockWise.upRight), ((int)ClockWise.upLeft, (int)ClockWise.downLeft));
            }

            Board = nodeArray[0];
        }

        /*
         *
         */
        private SlotNode VerticalSuture(int height, int upIdx, int downIdx)
        {
            SlotNode slotNode = null;

            for (int h = 0; h < height; ++h)
            {
                if (slotNode == null)
                    slotNode = new SlotNode();
                else
                {
                    slotNode.Link[upIdx] = new SlotNode();
                    slotNode.Link[upIdx].Link[downIdx] = slotNode;
                    slotNode = slotNode.Link[upIdx];
                }
            }

            return slotNode;
        }

        /* Suture top -> down */
        private void ZigzagSuture(SlotNode list1, SlotNode list2, (int way1, int way2) dir1, (int way1, int way2) dir2)
        {
            bool side = false;

            while (true)
            {
                if (side)
                {
                    list1.Link[dir1.way1] = list2;
                    list2.Link[dir2.way1] = list1;

                    if (list1.Link[(int)ClockWise.down] == null)
                        break;

                    list1 = list1.Link[(int)ClockWise.down];
                }
                else
                {
                    list1.Link[dir1.way2] = list2;
                    list2.Link[dir2.way2] = list1;

                    if (list2.Link[(int)ClockWise.down] == null)
                        break;

                    list2 = list2.Link[(int)ClockWise.down];
                }

                side = !side;
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
