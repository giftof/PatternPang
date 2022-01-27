/*
 * Create: [Game.cs] on Thu Jan 27 2022 4:58:52 PM
 * Author: [cloudeven@gmail.com]
 *
 * Copyright (c) 2022 [noname]
 */

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Pattern.Managers;
using Pattern.Objects;



public class Game : MonoBehaviour
{
    
    void Start()
    {
        Test_N_X_M_Board(3, 3);
        Debug.Log("");
        Test_N_X_M_Board(5, 5);

        SlotManager.Instance.CreateBoard(2, 2);

        int[] test = new int[2] {0, 1};

        IEnumerable<int> test2 = test.Select(e => e);
    }

    private void Test_N_X_M_Board(int width, int height)
    {
        Debug.Log($"Create [{width} x {height}] sized Board and display");

        SlotManager.Instance.CreateBoard(width, height);
        foreach (SlotNode node in SlotManager.Instance)
            DisplayClockWise(node);
    }

    private void DisplayClockWise(SlotNode node)
    {
        StringBuilder stringBuilder = new StringBuilder("Node condition = {");

        if (node == null)
            stringBuilder.Append("[null object!]");
        else
        {
            stringBuilder.Append($"nodeId[{node.Id}] == ");
            foreach ( int index in Enum.GetValues( typeof( ClockWise ) ) )
                if (index < (int)ClockWise.count)
                    stringBuilder.Append($"{index}: [{(node.Link[index] != null ? node.Link[index].Id.ToString() : "N")}], ");
        }
        
        stringBuilder.Append("}");
        Debug.LogWarning(stringBuilder);
    }
}
