/*
 * Create: [SlotManager.cs] on Wed Jan 26 2022 오후 4:40:32
 * Author: [cloudeven@gmail.com]
 *
 * Copyright (c) 2022 [noname]
 */



using System;
using Pattern.Objects;



namespace Pattern.Managers
{
    public class SlotManager
    {
        public SlotNode Board { get; set; }

        public static SlotManager Instance => m_instance.Value;
        public static int index;
        private static readonly Lazy<SlotManager> m_instance = new Lazy<SlotManager>( () => new SlotManager() );
        private SlotManager() { }

        public void CreateBoard(int width, int height)
        {
            SlotNode[] nodeArray = new SlotNode[width];

            index = 0;
            for (int w = 0; w < width; ++w)
                VerticalSuture(height, (int)ClockWise.up, (int)ClockWise.down, out nodeArray[w]);

            for (int w = 0; w < width - 1; ++w)
                ZigzagSuture((w % 2 == 0), nodeArray[w], nodeArray[w + 1], ((int)ClockWise.upRight, (int)ClockWise.downRight), ((int)ClockWise.downLeft, (int)ClockWise.upLeft));

            Board = nodeArray[0];
        }

        /* 
         * Suture down -> up
         * return bottom slot (by out keyword)
         */
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
        }
        
        /* 
         * Suture down -> up
         * nothing return
         */
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
    }
}
