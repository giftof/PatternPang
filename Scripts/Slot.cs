/*
 * Create: [Slot.cs] on Wed Jan 26 2022 오후 4:40:37
 * Author: [cloudeven@gmail.com]
 *
 * Copyright (c) 2022 [noname]
 */

using UnityEngine;
using System;
using System.Linq;
using Pattern.Configs;



namespace Pattern.Objects
{
    public class Slot
    {
        public SlotAttribute Color { get; set; } = SlotAttribute.none;
        public uint MatchCount { get; set; } = 0;
        public uint Id { get; set; } = 0;
    }



    public class SlotNode
    {
        public int Id { get; set; } = 0;
        public SlotNode[] Link { get; set; } = new SlotNode[(int)ClockWise.count];
        public SlotAttribute Color { get; set; } = SlotAttribute.none;
        public int MatchCount { get; set; } = 0;


        public SlotNode(int id)
        {
            Id = id;
            Dispose();
        }

        public void Dispose()
        {
            switch (MatchCount)
            {
                case 0:
                    break;
                case 1:
                case 2:
                    Color = SlotAttribute.none;
                    break;
                case 3:
                    Color = SlotAttribute.bomb1;
                    Color = SlotAttribute.none;
                    break;
                case 4:
                    Color = SlotAttribute.bomb2;
                    Color = SlotAttribute.none;
                    break;
                default:
                    Color = SlotAttribute.bomb3;
                    Color = SlotAttribute.none;
                    break;
            }

            MatchCount = 0;
        }

        public virtual SlotAttribute Request
        {
            get 
            {
                SlotAttribute color = Color;
                Color = SlotAttribute.none;
                return color;
            }
        }

        public int Increment()
        {
            ++MatchCount;
            return MatchCount;
        }

        public SlotNode NextSameColorNode(int dir)
            => Color != SlotAttribute.generator
            && Link[dir] != null
            && Link[dir].Color.Equals(Color)
            ? Link[dir] : null;
//        public SlotNode NextSameColorNode(int dir)
//        {
//Debug.Log($"ENTER NEXTSAMECOLORNODE >> ID = {Id}, dir = {(ClockWise)dir}, Link[dir] = {Link[dir]}");
//if (Color == PatternColor.generator)
//    Debug.LogError("Color == PatternColor.generator");
//else if (Link[dir] == null)
//    Debug.LogError($"Link[dir] == null, dir = {(ClockWise)dir}, really? = {Link[dir]}");
//else if (!Link[dir].Color.Equals(Color))
//    Debug.LogError("!Link[dir].Color.Equals(Color)");

//            return Color != PatternColor.generator 
//            && Link[dir] != null 
//            && Link[dir].Color.Equals(Color) 
//            ? Link[dir] : null;
//        }

        public int FindLinkIndex(SlotNode slotNode) => Link.Select((node, index) => new {node, index}).First(tp => slotNode.Equals(tp.node) || tp.index == (int)ClockWise.count).index;
    }



    public class ColorGenerator : SlotNode
    {
        public ColorGenerator(int id) : base(id) 
        { 
            Color = SlotAttribute.generator;
        }

        public override SlotAttribute Request
        {
            get { return (SlotAttribute)UnityEngine.Random.Range(1, (int)SlotAttribute.count - 5); /* 5 means remove (yellow, purple, bomb 1~3)*/ }
        }
    }
}
