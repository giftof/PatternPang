/*
 * Create: [GameLogic.cs] on Fri Jan 28 2022 12:17:11 PM
 * Author: [cloudeven@gmail.com]
 *
 * Copyright (c) 2022 [noname]
 */

using System;
using System.Linq;
using System.Collections.Generic;
using Pattern.Managers;
using Pattern.Objects;
using Pattern.Configs;



namespace Pattern.Business
{
    public class GameLogic
    {
        public int BoardWidth { get; set; }
        public int BoardHeight { get; set; }
        public static GameLogic Instance => m_instance.Value;
        private static readonly Lazy<GameLogic> m_instance = new Lazy<GameLogic>( () => new GameLogic() );
        private GameLogic() {}



        public void AddTrace(SlotNode slotNode)
            => PatternManager.Instance.Add(slotNode);

        public int[] Shape
        {
            get
            {
                int[] shape = PatternManager.Instance.Shape();

                if (shape == null)
                    PatternManager.Instance.Clear();

                return shape;
            }
        }

        public void Generate()
        {
            Dispose();
            SlotManager.Instance.CreateBoard(BoardWidth, BoardHeight);
        }

        public void Dispose()
        {
            while (true)
            {
                MarkingSelected();
                RemoveMarked();

                if (!FillColors(out int count))
                    break;
            }

            PatternManager.Instance.Clear();
        }

        public SlotNode Board
            => SlotManager.Instance.Board;
            
        /*
         * Privates
         */
        public void MarkingSelected()
        {
            int[] shape = Shape;
            int verify = shape?.Sum() ?? 0;

            if (verify.Equals(0))
                return;

            foreach (SlotNode node in SlotManager.Instance)
            {
                SlotNode temp = node;

                int zeroQuery = shape?
                    .Where(e
                        => temp != null 
                        && (temp = temp.NextSameColorNode(e)) != null)
                    .Sum(e => e)
                    .CompareTo(verify) ?? 1;
                
                if (zeroQuery.Equals(0))
                {
                    temp = node;
                    temp.Increment();

                    shape
                        .Where(e
                            => temp.Link[e].Increment() > 0
                            && (temp = temp.Link[e]) != null)
                        .Select(e => e)
                        .ToArray();
                }
            }
        }

        private void RemoveMarked()
        {
            if (SlotManager.Instance.Board == null)
                return;

            foreach (SlotNode node in SlotManager.Instance)
                if (node.MatchCount > 0)
                    node.Dispose();
        }

        private bool FillColors(out int count)
        {
            SlotNode[] bottomLine = SlotManager.Instance.BottomLineArray;
            int fillCount = 0;

            while (bottomLine != null && bottomLine.Where(e => RequestUpper(e) == true && ++fillCount > 0).Select(e => e).Count() != 0);

            return (count = fillCount) > 0;
        }

        private bool RequestUpper(SlotNode slotNode)
        {
            bool result = false;

            while (slotNode.Link[(int)ClockWise.up] != null)
            {
                if (slotNode.Color == PatternColor.none)
                {
                    result = true;
                    slotNode.Color = slotNode.Link[(int)ClockWise.up].Request;
                }

                slotNode = slotNode.Link[(int)ClockWise.up];
            }

            return result;
        }
    }
}



                // PatternColor beginColor = node.Color;

                // var zero = shape
                //     .Where(e1 => temp != null && (temp = temp.NextSameColorNode(e1)) != null)
                //     .Select(e1 => temp.Color.Equals(beginColor))
                //     .GroupBy(
                //         e => beginColor,
                //         e => temp,
                //         (key, value) => new {
                //             Color = key,
                //             Count = value.Count(),
                //             Begin = node,
                //             Value = value
                //         }
                //     );

                // UnityEngine.Debug.Log($"{zero.Count()}");
                // foreach (var item in zero)
                // {
                //     UnityEngine.Debug.Log($"{item}");
                //     UnityEngine.Debug.Log($"???");
                //     UnityEngine.Debug.Log($"color = {item.Color}, bc = {item.Begin.Color}, vcnt = {item.Value.Count()}");
                // }

                // UnityEngine.Debug.Log($"----------------------------------");
