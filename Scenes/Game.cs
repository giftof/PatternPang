using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pattern.Managers;
using Pattern.Objects;


public class Game : MonoBehaviour
{
    
    void Start()
    {
        Test_Three_X_Three_Board();
        Debug.Log("");
        Test_Five_X_Five_Board();
    }

    private void Test_Five_X_Five_Board()
    {
        Debug.Log("Display 5x5 board");

        SlotManager.Instance.CreateBoard(5, 5);
        SlotNode board;

        board = SlotManager.Instance.Board;
        DisplayClockWise(board);
        board = board.Link[1];
        DisplayClockWise(board);
        board = board.Link[1];
        DisplayClockWise(board);
        board = board.Link[1];
        DisplayClockWise(board);
        board = board.Link[1];
        DisplayClockWise(board);
        board = board.Link[1];
        DisplayClockWise(board);
    }
    private void Test_Three_X_Three_Board()
    {
        Debug.Log("Display 3x3 board");

        SlotManager.Instance.CreateBoard(3, 3);
        SlotNode board;

        board = SlotManager.Instance.Board;
        DisplayClockWise(board);
        board = board.Link[0];
        DisplayClockWise(board);
        board = board.Link[0];
        DisplayClockWise(board);

        board = board.Link[3];
        board = board.Link[2];
        DisplayClockWise(board);
        board = board.Link[0];
        DisplayClockWise(board);
        board = board.Link[0];
        DisplayClockWise(board);

        board = board.Link[3];
        board = board.Link[3];
        board = board.Link[2];
        DisplayClockWise(board);
        board = board.Link[0];
        DisplayClockWise(board);
        board = board.Link[0];
        DisplayClockWise(board);

        board = board.Link[0];
        DisplayClockWise(board);
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
