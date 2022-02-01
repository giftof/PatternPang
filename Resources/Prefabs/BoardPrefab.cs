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



public class BoardPrefab : MonoBehaviour
{
    [SerializeField] int Width {get; set;}
    [SerializeField] int Height {get; set;}
    [SerializeField] SlotPrefab slot;
    [SerializeField] BallPrefab ball;
    [SerializeField] List<SlotPrefab> list = new List<SlotPrefab>();
    [SerializeField] List<SlotPrefab> generator = new List<SlotPrefab>();
    public SlotNode Board {get; set;}



    public void RemoveBall()
    {
        foreach (var item in list)
        {
            if (item.SlotNode == null)
                Destroy(item.Ball.gameObject);
        }
    }

    private void Clear()
    {
        foreach (var item in list)
            Destroy(item.gameObject);
        
        foreach (var item in generator)
            Destroy(item.gameObject);

        list.Clear();
        generator.Clear();
    }

    public void FillColor()
    {
        foreach(SlotPrefab prefab in list)
        {
            if (prefab.Ball == null)
            {
                BallPrefab pball = Instantiate(ball);
                pball.GetComponent<Image>().color = SetColor(prefab.SlotNode.Color);
                pball.transform.SetParent(prefab.transform);
                pball.transform.localScale = Vector2.one;
                pball.transform.localPosition = Vector3.zero;
                prefab.Ball = pball;
            }
            else
                prefab.Ball.GetComponent<Image>().color = SetColor(prefab.SlotNode.Color);
        }
    }

    private Color SetColor(PatternColor color)
    {
        switch (color)
        {
            case PatternColor.red:
                return Color.red;
            case PatternColor.green:
                return Color.green;
            case PatternColor.blue:
                return Color.blue;
            case PatternColor.yellow:
                return Color.yellow;
            case PatternColor.purple:
                return Color.cyan;
            default:
                return Color.black;
        }
    }
    
    public void CreateBoard(int width, int height)
    {
        Clear();

        Width = width;
        Height = height + 1;

        float heightInterval = slot.GetComponent<RectTransform>().sizeDelta.x;
        float widthInterval = slot.GetComponent<RectTransform>().sizeDelta.y * 0.75f;
        float leftBottom = width % 2 == 0 ? -widthInterval * (width / 2) + (widthInterval / 2) : -widthInterval * (width / 2);
        Vector2 position = new Vector2(leftBottom, 0);
        int index = 0;
        
        foreach (SlotNode node in SlotManager.Instance)
        {
            SlotPrefab prefab = Instantiate(slot);

            if (index % Height == 0)
            {
                position = new Vector2(leftBottom + (index / Height) * widthInterval, 0);
                if ((index / Height) % 2 == 1)
                    position += Vector2.up * heightInterval * 0.5f;
            }

            position += Vector2.up * heightInterval;
            prefab.transform.SetParent(transform);
            prefab.transform.localScale = Vector2.one;
            prefab.transform.localPosition = position;
            prefab.FillAction = FillColor;

            if (index % Height == height)
            {
                prefab.SlotNode = node as ColorGenerator;
                generator.Add(prefab);
            }
            else
            {
                prefab.SlotNode = node;
                list.Add(prefab);
            }

            ++index;
        }
    }
}
