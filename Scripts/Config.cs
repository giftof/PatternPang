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
        public static float MAX_DISTANCE;
    }

    public static class PATH
    {
        public static readonly string SLOT = "Prefabs/SlotPrefab";
        public static readonly string BALL = "Prefabs/BallPrefab";
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
        test,
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
