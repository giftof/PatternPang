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
using UnityEngine.UI;
using Pattern.Managers;
using Pattern.Objects;
using Pattern.Configs;
using Pattern.Business;



public class Game : MonoBehaviour
{
    [SerializeField] InputField inputField;
    [SerializeField] InputField width;
    [SerializeField] InputField height;
    [SerializeField] Button generate;
    [SerializeField] Button view;
    [SerializeField] Button add;
    [SerializeField] Button shape;
    [SerializeField] Button end;



    void Start()
    {
        AddButtonAction();
    }

    
    private void AddButtonAction()
    {
        width.text = "3"; // 7
        height.text = "3"; // 8
        add.onClick.AddListener(Add);
        shape.onClick.AddListener(Shape);
        // end.onClick.AddListener(GameLogic.Instance.ClearTrace);
        end.onClick.AddListener(GameLogic.Instance.Dispose);
        generate.onClick.AddListener(Generate);
        view.onClick.AddListener(View);
    }

    private void Add()
    {
/**/        Debug.Log("press Add");

/**/        if (!int.TryParse(inputField.text, out int slotId))
/**/            throw new Exception("[numeric only]");

/**/        SlotNode node = FindSlotNode(slotId);
        GameLogic.Instance.AddTrace(node);
        
/**/        Debug.LogWarning($"{node.Id}");
    }

    private void Shape()
    {
/**/        Debug.Log("press Shape");
        int[] shape = GameLogic.Instance.Shape;

/**/        StringBuilder stringBuilder = new StringBuilder("Shape = { ");
        
/**/        if (shape == null)
/**/        {
/**/            Debug.LogWarning("selected amount less then 3");
/**/            if (PatternManager.Instance.m_selectedList.Count > 0)
/**/                foreach (SlotNode item in PatternManager.Instance.m_selectedList)
/**/                    Debug.LogWarning($"selected = {item.Id}");
/**/            else
/**/                Debug.LogWarning("selected count = 0");
/**/            return;
/**/        }

/**/        foreach (int dir in shape)
/**/            stringBuilder.Append($"{((ClockWise)dir).ToString()}, ");

/**/        stringBuilder.Append(" }");

/**/        Debug.LogWarning(stringBuilder);

            SlotNode[] array = GameLogic.Instance.SearchPattern();
            Debug.Log($"match count = {array.Length}");
            foreach (SlotNode node in array)
                Debug.Log($"id = {node.Id}, color = {node.Color}");
    }

    private SlotNode FindSlotNode(int id)
    {
/**/        foreach (SlotNode node in SlotManager.Instance)
/**/        {
/**/            if (id == 0)
/**/                return node;
/**/            --id;
/**/        }

/**/        throw new Exception("[id Error!]");
    }

    private void View()
    {
        if (SlotManager.Instance.Board != null)
            foreach (SlotNode node in SlotManager.Instance)
                DisplayClockWise(node);
    }

    private void Generate()
    {
/**/        Debug.Log("press Generate");

/**/        if (!int.TryParse(this.width.text, out int width))
/**/        {
/**/            Debug.LogWarning($"Wrong input width = ({this.width.text}_");
/**/            return;
/**/        }

/**/        if (!int.TryParse(this.height.text, out int height))
/**/        {
/**/            Debug.LogWarning($"Wrong input height = ({this.height.text}_");
/**/            return;
/**/        }

        GameLogic.Instance.BoardWidth = Math.Abs(width);
        GameLogic.Instance.BoardHeight = Math.Abs(height);
        GameLogic.Instance.Generate();
        GameLogic.Instance.Dispose();

        View();
    }

    private void DisplayClockWise(SlotNode node)
    {
        StringBuilder stringBuilder = new StringBuilder("Node = {");

        if (node == null)
            stringBuilder.Append("[null object!]");
        else
        {
            stringBuilder.Append($"[{node.Id}: {node.Color.ToString()}] == ");
            foreach ( int index in Enum.GetValues( typeof( ClockWise ) ) )
            {
                if (index < (int)ClockWise.count)
                {
                    stringBuilder.Append($"{index}: [{(node.Link[index] != null ? node.Link[index].Id.ToString() : "_")}:");
                    stringBuilder.Append($"{(node.Link[index] != null ? node.Link[index].Color.ToString() : "_")}], ");
                }
            }
        }
        
        stringBuilder.Append("}");
        Debug.LogWarning(stringBuilder);
    }
}
