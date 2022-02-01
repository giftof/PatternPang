/*
 * Create: [GameLogic.cs] on Fri Jan 28 2022 12:17:11 PM
 * Author: [cloudeven@gmail.com]
 *
 * Copyright (c) 2022 [noname]
 */

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

                if (FillColors() == 0)
                    break;
            }

            PatternManager.Instance.Clear();
        }

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
                // if (node.Color == PatternColor.generator)
                //     continue;

                SlotNode temp = node;
                int zeroQuery = shape?
                    .Where(e
                        => temp != null 
                        && (temp = temp.NextSameColorNode(e)) != null)
                    .Sum(e
                        => e)
                    .CompareTo(verify) ?? 1;
                
                if (zeroQuery.Equals(0))
                {
                    temp = node;
                    temp.Increment();

                    shape
                        .Where(e
                            => temp.Link[e].Increment() > 0
                            && (temp = temp.Link[e]) != null)
                        .Select(e
                            => e)
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

        private int FillColors()
        {
            SlotNode[] bottomLine = SlotManager.Instance.BottomLineArray;
            int fillCount = 0;

            while (bottomLine != null && bottomLine.Where(e => RequestUpper(e) == true && ++fillCount > 0).Select(e => e).Count() != 0);

            return fillCount;
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
