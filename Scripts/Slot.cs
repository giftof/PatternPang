/*
 * Create: [Slot.cs] on Wed Jan 26 2022 오후 4:40:37
 * Author: [cloudeven@gmail.com]
 *
 * Copyright (c) 2022 [noname]
 */

using System.Linq;



namespace Pattern.Objects
{
    public enum PatternColor
    {
        none,
        red,
        green,
        blue,
        yellow,
        purple,
        bomb1,
        bomb2,
        bomb3,
        count,
    }

    /*
     * Component count must be [EVEN (2n)] + [count]
     * As a result, the count is [ODD]
     */
    public enum ClockWise
    {
        up,
        upRight,
        downRight,
        down,
        downLeft,
        upLeft,
        count,
    }

    public class SlotNode
    {
        public int Id { get; set; }
        public SlotNode[] Link { get; set; }
        public PatternColor Color { get; set; }



        public SlotNode(int id)
        {
            Id = id;
            // Color = PatternColor.none;
            Color = (PatternColor)UnityEngine.Random.Range(1, (int)PatternColor.count - 5); // 5 means remove (yellow, purple, bomb 1~3)
            Link = new SlotNode[(int)ClockWise.count];
        }

        public int FindLinkIndex(SlotNode slotNode) => Link.Select((node, index) => new {node, index}).First(tp => slotNode.Equals(tp.node) || tp.index == (int)ClockWise.count).index;
        // public int FindLinkIndex(SlotNode slotNode)
        // {
        //     for (int i = 0; i < (int)ClockWise.count; ++i)
        //         if (slotNode.Equals(Link[i]))
        //             return i;

        //     return 99;
        // }
    }
}
