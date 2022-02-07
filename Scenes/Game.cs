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



public class Game : MonoBehaviour
{
    [SerializeField] GameLogic gamePrefab;
    [SerializeField] Button generate;
    [SerializeField] Button clear;
    [SerializeField] Button re;

    void Start()
    {
        gamePrefab.Size = CONST.SIZE0;
        SetButtonAction();
    }

    private void SetButtonAction()
    {
        generate.onClick.AddListener(gamePrefab.Initialize);
        clear.onClick.AddListener(gamePrefab.ClearBoard);
        re.onClick.AddListener(gamePrefab.ClearBall);
    }
}
