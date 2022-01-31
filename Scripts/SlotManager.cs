/*
 * Create: [SlotManager.cs] on Wed Jan 26 2022 오후 4:40:32
 * Author: [cloudeven@gmail.com]
 *
 * Copyright (c) 2022 [noname]
 */

using System;
using System.Collections;
using System.Collections.Generic;
using Pattern.Objects;
using Pattern.Configs;



namespace Pattern.Managers
{
    public class SlotManager : IEnumerable
    {
        public SlotNode[] BottomLineArray { get; private set; } = null;
        public SlotNode Board { get; private set; } = null;
        private int boardSize;

        public static SlotManager Instance => m_instance.Value;
        private static int index;
        private static readonly Lazy<SlotManager> m_instance = new Lazy<SlotManager>( () => new SlotManager() );
        private SlotManager() { }

        public void CreateBoard(int width, int height)
        {
            SlotNode[] nodeArray = new SlotNode[width];

            index = 0;
            boardSize = width * height;
            
            for (int w = 0; w < width; ++w)
                VerticalSuture(height, (int)ClockWise.up, (int)ClockWise.down, out nodeArray[w]);

            for (int w = 0; w < width - 1; ++w)
                ZigzagSuture((w % 2 == 0), nodeArray[w], nodeArray[w + 1], ((int)ClockWise.upRight, (int)ClockWise.downRight), ((int)ClockWise.downLeft, (int)ClockWise.upLeft));

            Board = nodeArray[0];
            BottomLineArray = nodeArray;
        }

        /*
         * Implement IEnumerable
         */
        public IEnumerator GetEnumerator() => new CustomIEnumerator(Board);

        /*
         * Privates
         */
        /** Suture [down -> up] return bottom slot (by out keyword) **/
        private void VerticalSuture(int height, int upIdx, int downIdx, out SlotNode node)
        {
            SlotNode slotNode = new SlotNode(index++);
            
            node = slotNode;
            for (int h = 1; h < height; ++h)
            {
                slotNode.Link[upIdx] = new SlotNode(index++);
                slotNode.Link[upIdx].Link[downIdx] = slotNode;
                slotNode = slotNode.Link[upIdx];
            }

            slotNode.Link[upIdx] = new ColorGenerator(index++);
            slotNode.Link[upIdx].Link[downIdx] = slotNode;
        }
        
        /** Suture [down -> up] nothing return **/
        private void ZigzagSuture(bool zigzag, SlotNode list1, SlotNode list2, (int way1, int way2) dir1, (int way1, int way2) dir2)
        {
            while (true)
            {
                if (zigzag)
                {
                    list1.Link[dir1.way1] = list2;
                    list2.Link[dir2.way1] = list1;

                    if (list1.Link[(int)ClockWise.up] == null)
                        break;
                    list1 = list1.Link[(int)ClockWise.up];
                }
                else
                {
                    list1.Link[dir1.way2] = list2;
                    list2.Link[dir2.way2] = list1;

                    if (list2.Link[(int)ClockWise.up] == null)
                        break;
                    list2 = list2.Link[(int)ClockWise.up];
                }

                zigzag = !zigzag;
            }
        }

        /*
         * Nest Class: IEnumerator
         */
        private class CustomIEnumerator : IEnumerator
        {
            public SlotNode Board;
            private bool m_begin;

            public CustomIEnumerator(SlotNode board)
            {
                m_begin = false;
                Board = board;
            }

            public bool MoveNext()
            {
                if (!m_begin || MoveUp() || MoveNextLineBottom())
                    return m_begin = true;
                return false;
            }

            public void Reset()
            {
                while (MovePreviousLineBottom());
                MoveBottom();
                m_begin = false;
            }

            public object Current 
            {
                get
                {
                    if (Board != null)
                        return Board;
                    return new Exception("[null Exception]");
                }
            }

            private bool MoveUp()
            {
                if (Board.Link[(int)ClockWise.up] != null)
                {
                    Board = Board.Link[(int)ClockWise.up];
                    return true;
                }

                return false;
            }

            private bool MoveNextLineBottom()
            {
                if (Board.Link[(int)ClockWise.downRight] != null)
                    Board = Board.Link[(int)ClockWise.downRight];
                else if (Board.Link[(int)ClockWise.upRight] != null)
                    Board = Board.Link[(int)ClockWise.upRight];
                else
                    return false;

                MoveBottom();

                return true;
            }

            private bool MovePreviousLineBottom()
            {
                if (Board.Link[(int)ClockWise.downLeft] != null)
                    Board = Board.Link[(int)ClockWise.downLeft];
                else if (Board.Link[(int)ClockWise.upLeft] != null)
                    Board = Board.Link[(int)ClockWise.upLeft];
                else
                    return false;
                    
                MoveBottom();

                return true;
            }

            private void MoveBottom()
            {
                while (Board.Link[(int)ClockWise.down] != null)
                    Board = Board.Link[(int)ClockWise.down];
            }
        }
    }





}
