using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pattern.Objects;
using Pattern.Managers;
using Pattern.Business;
using Pattern.Configs;



public class BoardPrefab : MonoBehaviour /*, IEnumerable*/
{



    //[SerializeField] int Width {get; set;}
    //[SerializeField] int Height {get; set;}
    //[SerializeField] SlotPrefab slot;
    ////[SerializeField] BallPrefab ball;
    ////[SerializeField] List<SlotPrefab> list = new List<SlotPrefab>();
    ////[SerializeField] List<SlotPrefab> generator = new List<SlotPrefab>();
    //public SlotPrefab[] BottomLineArray { get; private set; } = null;
    //public SlotPrefab Board { get; private set; } = null;
    ////int index = 0;

    ///*
    // * Implement IEnumerable
    // */
    //public IEnumerator GetEnumerator() => new CustomIEnumerator(Board);



    ////public void RemoveBall()
    ////{
    ////    foreach (var item in list)
    ////    {
    ////        if (item.SlotNode == null)
    ////            Destroy(item.Ball.gameObject);
    ////    }
    ////}

    //private void Clear()
    //{
    //    foreach (var item in Board)
    //    {
    //        var temp = item;
            
    //    }
    //    //foreach (var item in list)
    //    //    Destroy(item.gameObject);
        
    //    //foreach (var item in generator)
    //    //    Destroy(item.gameObject);

    //    //list.Clear();
    //    //generator.Clear();
    //}

    //public void FillColor()
    //{
    //    //foreach(SlotPrefab prefab in list)
    //    //{
    //    //    if (prefab.Ball == null)
    //    //    {
    //    //        BallPrefab pball = Instantiate(ball);
    //    //        pball.GetComponent<Image>().color = SetColor(prefab.SlotNode.Color);
    //    //        pball.transform.SetParent(prefab.transform);
    //    //        pball.transform.localScale = Vector2.one;
    //    //        pball.transform.localPosition = Vector3.zero;
    //    //        prefab.Ball = pball;
    //    //    }
    //    //    else
    //    //    {
    //    //        Debug.Log("hello!");
    //    //        prefab.Ball.GetComponent<Image>().color = SetColor(prefab.SlotNode.Color);
    //    //    }
    //    //}
    //}

    //private Color SetColor(PatternColor color)
    //{
    //    switch (color)
    //    {
    //        case PatternColor.red:
    //            return Color.red;
    //        case PatternColor.green:
    //            return Color.green;
    //        case PatternColor.blue:
    //            return Color.blue;
    //        case PatternColor.yellow:
    //            return Color.yellow;
    //        case PatternColor.purple:
    //            return Color.cyan;
    //        default:
    //            return Color.black;
    //    }
    //}
    
    ////public void CreateBoard2(int width, int height)
    ////{
    ////    Clear();

    ////    Width = width;
    ////    Height = height + 1;

    ////    float heightInterval = slot.GetComponent<RectTransform>().sizeDelta.x;
    ////    float widthInterval = slot.GetComponent<RectTransform>().sizeDelta.y * 0.75f;
    ////    float leftBottom = width % 2 == 0 ? -widthInterval * (width / 2) + (widthInterval / 2) : -widthInterval * (width / 2);
    ////    Vector2 position = new Vector2(leftBottom, 0);
    ////    int index = 0;
        
    ////    foreach (SlotNode node in SlotManager.Instance)
    ////    {
    ////        SlotPrefab prefab = Instantiate(slot);

    ////        if (index % Height == 0)
    ////        {
    ////            position = new Vector2(leftBottom + (index / Height) * widthInterval, 0);
    ////            if ((index / Height) % 2 == 1)
    ////                position += Vector2.up * heightInterval * 0.5f;
    ////        }

    ////        position += Vector2.up * heightInterval;
    ////        prefab.transform.SetParent(transform);
    ////        prefab.transform.localScale = Vector2.one;
    ////        prefab.transform.localPosition = position;
    ////        prefab.FillAction = FillColor;

    ////        if (index % Height == height)
    ////        {
    ////            prefab.SlotNode = node as ColorGenerator;
    ////            generator.Add(prefab);
    ////        }
    ////        else
    ////        {
    ////            prefab.SlotNode = node;
    ////            list.Add(prefab);
    ////        }

    ////        ++index;
    ////    }
    ////}

    //public void CreateBoard(int width, int height)
    //{
    //    SlotPrefab[] nodeArray = new SlotPrefab[width];

    //    index = 0;

    //    for (int w = 0; w < width; ++w)
    //        VerticalSewing(height, (int)ClockWise.up, (int)ClockWise.down, out nodeArray[w]);

    //    for (int w = 0; w < width - 1; ++w)
    //        ZigzagSewing(w % 2 == 0, nodeArray[w], nodeArray[w + 1], ((int)ClockWise.upRight, (int)ClockWise.downRight), ((int)ClockWise.downLeft, (int)ClockWise.upLeft));

    //    Board = nodeArray[0];
    //    BottomLineArray = nodeArray;
    //}

    ///*
    // * Privates
    // */
    ///** Sewing [down -> up] return bottom slot (by out keyword) **/
    //private void VerticalSewing(int height, int upIdx, int downIdx, out SlotPrefab node)
    //{
    //    SlotPrefab slotNode = Instantiate(slot);

    //    node = slotNode;
    //    for (int h = 1; h < height; ++h)
    //    {
    //        slotNode.Link[upIdx] = Instantiate(slot);
    //        slotNode.Link[upIdx].Link[downIdx] = slotNode;
    //        slotNode = slotNode.Link[upIdx];
    //    }

    //    slotNode.Link[upIdx] = Instantiate(slot);
    //    slotNode.Link[upIdx].Link[downIdx] = slotNode;
    //}

    ///** Sewing [down -> up] nothing return **/
    //private void ZigzagSewing(bool zigzag, SlotPrefab list1, SlotPrefab list2, (int way1, int way2) dir1, (int way1, int way2) dir2)
    //{
    //    while (true)
    //    {
    //        if (zigzag)
    //        {
    //            list1.Link[dir1.way1] = list2;
    //            list2.Link[dir2.way1] = list1;

    //            if (list1.Link[(int)ClockWise.up] == null)
    //                break;
    //            list1 = list1.Link[(int)ClockWise.up];
    //        }
    //        else
    //        {
    //            list1.Link[dir1.way2] = list2;
    //            list2.Link[dir2.way2] = list1;

    //            if (list2.Link[(int)ClockWise.up] == null)
    //                break;
    //            list2 = list2.Link[(int)ClockWise.up];
    //        }

    //        zigzag = !zigzag;
    //    }
    //}

    ///*
    // * Nest Class: IEnumerator
    // */
    //private class CustomIEnumerator : IEnumerator
    //{
    //    public SlotPrefab Board;
    //    private bool m_begin;

    //    public CustomIEnumerator (SlotPrefab board)
    //    {
    //        m_begin = false;
    //        Board = board;
    //    }

    //    public bool MoveNext()
    //    {
    //        if (!m_begin || MoveUp() || MoveNextLineBottom())
    //            return m_begin = true;
    //        return false;
    //    }

    //    public void Reset()
    //    {
    //        while (MovePreviousLineBottom()) ;
    //        MoveBottom();
    //        m_begin = false;
    //    }

    //    public object Current
    //    {
    //        get
    //        {
    //            if (Board != null)
    //                return Board;
    //            return new Exception("[null Exception]");
    //        }
    //    }

    //    private bool MoveUp()
    //    {
    //        if (Board.Link[(int)ClockWise.up].SlotNode.Color != PatternColor.generator)
    //        {
    //            Board = Board.Link[(int)ClockWise.up];
    //            return true;
    //        }

    //        return false;
    //    }

    //    private bool MoveNextLineBottom()
    //    {
    //        if (Board.Link[(int)ClockWise.downRight] != null)
    //            Board = Board.Link[(int)ClockWise.downRight];
    //        else if (Board.Link[(int)ClockWise.upRight] != null)
    //            Board = Board.Link[(int)ClockWise.upRight];
    //        else
    //            return false;

    //        MoveBottom();

    //        return true;
    //    }

    //    private bool MovePreviousLineBottom()
    //    {
    //        if (Board.Link[(int)ClockWise.downLeft] != null)
    //            Board = Board.Link[(int)ClockWise.downLeft];
    //        else if (Board.Link[(int)ClockWise.upLeft] != null)
    //            Board = Board.Link[(int)ClockWise.upLeft];
    //        else
    //            return false;

    //        MoveBottom();

    //        return true;
    //    }

    //    private void MoveBottom()
    //    {
    //        while (Board.Link[(int)ClockWise.down] != null)
    //            Board = Board.Link[(int)ClockWise.down];
    //    }
    //}

}
