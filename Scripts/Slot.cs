/*
 * Create: [Slot.cs] on Wed Jan 26 2022 오후 4:40:37
 * Author: [cloudeven@gmail.com]
 *
 * Copyright (c) 2022 [noname]
 */

using System.Linq;
using Pattern.Configs;



namespace Pattern.Objects
{
    public class SlotNode
    {
        public int Id { get; set; } = 0;
        public SlotNode[] Link { get; set; } = new SlotNode[(int)ClockWise.count];
        public PatternColor Color { get; set; } = PatternColor.none;
        public int MatchCount { get; set; } = 0;



        public SlotNode(int id)
        {
            Id = id;
            Dispose();
            // Color = PatternColor.none;
            // Color = (PatternColor)UnityEngine.Random.Range(1, (int)PatternColor.count - 5); // 5 means remove (yellow, purple, bomb 1~3)
            // Link = new SlotNode[(int)ClockWise.count];
        }

        public void Dispose()
        {
            switch (MatchCount)
            {
                case 0:
                    break;
                case 1:
                case 2:
                    Color = PatternColor.none;
                    break;
                case 3:
                    Color = PatternColor.bomb1;
                    break;
                case 4:
                    Color = PatternColor.bomb2;
                    break;
                default:
                    Color = PatternColor.bomb3;
                    break;
            }
            
            MatchCount = 0;
        }

        public virtual PatternColor Request
        {
            get 
            {
                PatternColor color = Color;
                Color = PatternColor.none;
                return color;
            }
        }

        public int Increment()
        {
            ++MatchCount;
            return MatchCount;
        }

        public SlotNode NextSameColorNode(int dir)
            => (Color != PatternColor.generator && Link[dir] != null && Link[dir].Color.Equals(Color)) ? Link[dir] : null;

        public int FindLinkIndex(SlotNode slotNode) => Link.Select((node, index) => new {node, index}).First(tp => slotNode.Equals(tp.node) || tp.index == (int)ClockWise.count).index;
    }



    public class ColorGenerator : SlotNode
    {
        public ColorGenerator(int id) : base(id) 
        { 
            Color = PatternColor.generator;
        }

        public override PatternColor Request
        {
            get { return (PatternColor)UnityEngine.Random.Range(1, (int)PatternColor.count - 5); /* 5 means remove (yellow, purple, bomb 1~3)*/ }
        }
    }
}
