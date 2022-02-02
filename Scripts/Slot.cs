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

        public Slot(uint id)
        {
            Id = id;
        }
    }
}
