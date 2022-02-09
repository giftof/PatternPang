using Pattern.Configs;

namespace Pattern.Objects
{
    public class Slot
    {
        public SlotAttribute Color { get; set; } = SlotAttribute.none;
        public uint Id { get; set; } = 0; /* remove */

        public Slot(uint id) /* remove */
            => Id = id; /* remove */
    }

    //public sealed class BottomSlot
    //{
    //    public bool IsWorking { get; set; }
    //}
}
