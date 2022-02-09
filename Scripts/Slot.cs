/*
 * Create: [Slot.cs] on Wed Jan 26 2022 오후 4:40:37
 * Author: [cloudeven@gmail.com]
 *
 * Copyright (c) 2022 [noname]
 */

using Pattern.Configs;



namespace Pattern.Objects
{
    public class Slot
    {
        public SlotAttribute Color { get; set; } = SlotAttribute.none;
        public uint Id { get; set; } = 0;

        public Slot(uint id)
            => Id = id;
    }

    public sealed class BottomSlot
    {
        public bool IsWorking { get; set; }
    }
}
