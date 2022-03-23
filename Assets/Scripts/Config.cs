using UnityEngine;

namespace Pattern.Configs
{
    public static class CONST
    {
        public static float OFFSET = 0;
        public static readonly int MIN_SELECT = 3;
        public static readonly int BOMB = 3;
        public static readonly int GENERATOR_LINE = 1;
        public static readonly int EVEN_COLUMN_UP = 1;
        public static readonly int TEMP_DROP_DISTANCE = 9;
        public static readonly float LINE_WIDTH = .1f;
        public static readonly float HEXAGON_WIDTH_RATIO = .75f;

        public static readonly float DURATION_MOVE = .18f;
        public static readonly float DURATION_PLAY_TIME = 30;
        public static readonly float DURATION_JELLY_ELASTICITY = .6f;
        public static readonly long DURATION_VIBRATE_ADD = 8; // ms: vibe
        //public static readonly long[] DURATION_VIBRATE_REMOVE = new long[4] { 0, 1, 100, 1 }; // ms: rest, vibe, rest, vibe
        public static readonly long DURATION_VIBRATE_REMOVE = 1;
        public static readonly float DURATION_WAIT_REMOVE = 1;
        public static readonly float DURATION_WAIT_FILL_BALL = .1f;
        public static readonly float DURATION_WAIT_MATCH_BALL = .4f;
        public static readonly float DURATION_BOMB_STEP = .1f;
        public static readonly float DURATION_COMBO = 3f;

        public static readonly float COMBOBOX_FLOATING_DISTANCE = 30f;
        public static readonly int SCORE_BOMB = 5;
        public static readonly int BONUS_TIMER_BEGIN_VALUE = 0;

        public static float MAX_DISTANCE;
        public static Vector3[] DIRECTION_OFFSET = new Vector3[6];

        public static readonly int LEVEL1 = 1;
        public static readonly int LEVEL2 = 2;
        public static readonly int LEVEL3 = 3;
        public static readonly int LEVEL4 = 4;
        public static readonly int LEVEL5 = 5;
        public static readonly (int, int) SIZE33 = (3, 3 + GENERATOR_LINE);
        public static readonly (int, int) SIZE75 = (7, 5 + GENERATOR_LINE);
        public static readonly (int, int) SIZE78 = (7, 8 + GENERATOR_LINE);
        public static readonly (int, int) SIZE911 = (9, 11 + GENERATOR_LINE);
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
