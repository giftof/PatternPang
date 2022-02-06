/*
 * Create: [Config.cs] on Mon Jan 31 2022 6:03:17 PM
 * Author: [cloudeven@gmail.com]
 *
 * Copyright (c) 2022 [noname]
 */



namespace Pattern.Configs
{
    public static class CONST
    {
        public static readonly uint MIN_SELECT = 3;
        public static readonly uint BOMB = 3;
        public static readonly uint GENERATOR_LINE = 1;
        public static readonly uint EVEN_COLUMN_UP = 1;
        public static float MAX_DISTANCE;
        public static readonly float MOVE_DURATION = .1f;
        public static readonly uint LEVEL1 = 6;
        public static readonly uint LEVEL2 = 4;
        public static readonly (uint, uint) SIZE0 = (3, 3 + GENERATOR_LINE);
        public static readonly (uint, uint) SIZE1 = (7, 8 + GENERATOR_LINE);
        public static readonly (uint, uint) SIZE2 = (9, 11 + GENERATOR_LINE);
        public static readonly uint TEMP_DROP_DISTANCE = 9;
    }

    //public static class PATH
    //{
    //    public static readonly string SLOT = "Prefabs/SlotPrefab";
    //    public static readonly string BALL = "Prefabs/BallPrefab";
    //}

    public enum AddBall
    {
        remove = -1,
        none,
        add,
    }

    public enum SlotAttribute
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
        generator,
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

}
