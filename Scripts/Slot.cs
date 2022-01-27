/*
 * Create: [Slot.cs] on Wed Jan 26 2022 오후 4:40:37
 * Author: [cloudeven@gmail.com]
 *
 * Copyright (c) 2022 [noname]
 */

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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
            Color = PatternColor.none;
            Link = new SlotNode[(int)ClockWise.count];
        }

        public int FindLinkIndex(SlotNode slotNode) => Link.Select((node, index) => new {node, index}).First(e => e.node.Equals(slotNode) || e.index == (int)ClockWise.count).index;
        // public int MirrorSideLink(ClockWise clockWise) => ((int)clockWise + ((int)ClockWise.count/2)) % (int)ClockWise.count;
        // public int MirrorSideLinkIndex(SlotNode slotNode) => FindLinkIndex(slotNode) + ( ( (int)ClockWise.count/2 ) % (int)ClockWise.count );
    }
}
