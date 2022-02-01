/*
 * Create: [Config.cs] on Mon Jan 31 2022 6:03:17 PM
 * Author: [cloudeven@gmail.com]
 *
 * Copyright (c) 2022 [noname]
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Pattern.Configs
{
    public static class CONST
    {
        public static readonly int MIN_SELECT = 3;
        public static readonly int BOMB = 3;
    }

    // public enum Const
    // {
    //     bomb = 3;
    // }

    public enum PatternColor
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
