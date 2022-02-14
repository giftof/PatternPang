using UnityEngine;

namespace Pattern.Configs
{
    public static class CONST
    {
        public static readonly uint LEVEL1 = 3;
        public static readonly uint LEVEL2 = 4;
        public static readonly uint LEVEL3 = 5;
        public static readonly (uint, uint) SIZE0 = (3, 3 + GENERATOR_LINE);
        public static readonly (uint, uint) SIZE1 = (7, 8 + GENERATOR_LINE);
        public static readonly (uint, uint) SIZE2 = (9, 11 + GENERATOR_LINE);

        public static readonly uint MIN_SELECT = 3;
        public static readonly uint BOMB = 3;
        public static readonly uint GENERATOR_LINE = 1;
        public static readonly uint EVEN_COLUMN_UP = 1;
        public static readonly uint TEMP_DROP_DISTANCE = 9;
        public static readonly float LINE_WIDTH = .1f;
        public static readonly float HEXAGON_WIDTH_RATIO = .75f;

        public static readonly float DURATION_MOVE = .1f;
        public static readonly float DURATION_PLAY_TIME = 30;
        public static readonly float DURATION_JELLY_ELASTICITY = .6f;
        public static readonly long DURATION_VIBRATE_ADD = 8; // ms: vibe
        //public static readonly long[] DURATION_VIBRATE_REMOVE = new long[4] { 0, 1, 100, 1 }; // ms: rest, vibe, rest, vibe
        public static readonly long DURATION_VIBRATE_REMOVE = 1;
        public static readonly float DURATION_WAIT_REMOVE = 1;
        public static readonly float DURATION_WAIT_FILL_BALL = .1f;
        public static readonly float DURATION_BOMB_STEP = .001f;

        public static readonly int SCORE_BOMB = 5;

        public static float MAX_DISTANCE;
        public static Vector3[] DIRECTION_OFFSET = new Vector3[6];
    }

    public enum AddBall
    {
        remove = -1,
        none,
        add,
    }

    public enum SlotAttribute
    {
        none = -1,
        red,
        green,
        blue,
        yellow,
        purple,
        color_count,
        bomb1,
        bomb2,
        bomb3,
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
