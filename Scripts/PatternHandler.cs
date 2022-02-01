using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pattern.Objects;
using Pattern.Configs;



namespace Pattern.Managers
{
    public partial class PatternHandler
    {
        private LinkedList<Slot> m_selectedList;
        public (uint Row, uint Column) Size { get; set; } = (0, 0);
        public uint EvenColumnUp = 1;

        public static PatternHandler Instance => m_instance.Value;
        private static readonly Lazy<PatternHandler> m_instance = new Lazy<PatternHandler>(() => new PatternHandler());
        private PatternHandler() => m_selectedList = new LinkedList<Slot>();

        public void Add(Slot slot)
            => m_selectedList.AddFirst(slot);

        public void Remove()
            => m_selectedList.RemoveFirst();

        public bool IsFloating(uint row)
            => row % 2 == EvenColumnUp;

        public ClockWise[] Shape()
        {
            if (m_selectedList.Count < CONST.MIN_SELECT)
                return null;

            LinkedListNode<Slot> node = m_selectedList.First;
            List<ClockWise> list = new List<ClockWise>();

            for (int i = 0; i < m_selectedList.Count - 1; ++i)
            {
                if (ToDirection(node.Value.Id, node.Next.Value.Id) is ClockWise clockWise)
                    list.Add(clockWise);
                else
                    return null;

                node = node.Next;
            }

            return list.ToArray();
        }
    }


    // converter
    public partial class PatternHandler
    {
        public uint? ToId(ClockWise direction, uint currentId)
        {
            (uint row, uint column) currentOffset = (currentId / Size.Row, currentId % Size.Row);

            switch (direction)
            {
                case ClockWise.up:
                    if (currentOffset.column < Size.Column - 2)
                        return ++currentId;
                    return null;
                case ClockWise.down:
                    if (currentOffset.column > 0)
                        return --currentId;
                    return null;
                case ClockWise.upRight:
                    if (currentOffset.row < Size.Row)
                    {
                        if (IsFloating(currentOffset.row) && currentOffset.column < Size.Column - 2)
                            return currentId + Size.Column + 1;
                        if (!IsFloating(currentOffset.row))
                            return currentId + Size.Column;
                    }
                    return null;
                case ClockWise.downRight:
                    if (currentOffset.row < Size.Row)
                    {
                        if (IsFloating(currentOffset.row))
                            return currentId + Size.Column;
                        if (!IsFloating(currentOffset.row) && currentOffset.row > 0)
                            return currentId + Size.Column - 1;
                    }
                    return null;
                case ClockWise.upLeft:
                    if (currentOffset.row > 0)
                    {
                        if (IsFloating(currentOffset.row) && currentOffset.column < Size.Column - 2)
                            return currentId - Size.Column + 1;
                        if (!IsFloating(currentOffset.row))
                            return currentId - Size.Column;
                    }
                    return null;
                case ClockWise.downLeft:
                    if (currentOffset.row > 0)
                    {
                        if (IsFloating(currentOffset.row))
                            return currentId - Size.Column;
                        if (!IsFloating(currentOffset.row) && currentOffset.row > 0)
                            return currentId - Size.Column - 1;
                    }
                    return null;
                default:
                    return null;
            }
        }

        public ClockWise? ToDirection(uint last, uint current)
        {
            if (m_selectedList.Count == 0)
                return null;

            (uint row, uint column) currentOffset = (current / Size.Row, current % Size.Row);
            (uint row, uint column) lastOffset = (last / Size.Row, last % Size.Row);

            if (currentOffset.row.Equals(lastOffset.row))
            {
                // same row
                if (currentOffset.column.Equals(lastOffset.column + 1))
                    return ClockWise.up;
                if (lastOffset.column.Equals(currentOffset.column + 1))
                    return ClockWise.down;
            }

            if (currentOffset.row.Equals(lastOffset.row + 1))
            {
                // to right
                if (IsFloating(currentOffset.row))
                {
                    if (currentOffset.column.Equals(lastOffset.column))
                        return ClockWise.upRight;
                    if (lastOffset.column.Equals(currentOffset.column + 1))
                        return ClockWise.downRight;
                }
                else
                {
                    if (lastOffset.column.Equals(currentOffset.column))
                        return ClockWise.downRight;
                    if (currentOffset.column.Equals(lastOffset.column + 1))
                        return ClockWise.upRight;
                }
            }

            if (lastOffset.row.Equals(currentOffset.row + 1))
            {
                // to left
                if (IsFloating(currentOffset.row))
                {
                    if (currentOffset.column.Equals(lastOffset.column))
                        return ClockWise.upLeft;
                    if (lastOffset.column.Equals(currentOffset.column + 1))
                        return ClockWise.downLeft;
                }
                else
                {
                    if (lastOffset.column.Equals(currentOffset.column))
                        return ClockWise.downLeft;
                    if (currentOffset.column.Equals(lastOffset.column + 1))
                        return ClockWise.upLeft;
                }
            }

            throw new Exception($"something wrong link, last offset = {lastOffset}, current offset = {currentOffset}");
        }
    }
}
