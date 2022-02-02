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
    [SerializeField] GamePrefab gamePrefab;
    [SerializeField] PlayPrefab playPrefab;

    void Start()
    {
        gamePrefab.Initialize(CONST.SIZE1);
        //gamePrefab.Size = (7, 8 + CONST.GENERATOR_LINE);
        //gamePrefab.Generate();

        //playPrefab.Generator = gamePrefab.BallGenerator();
        //playPrefab.BaseLine = gamePrefab.BaseLine;

        //SlotPrefab[] generator = gamePrefab.BallGenerator();
        //foreach (var item in generator)
        //{
        //    Debug.Log($"generator item.name = {item.name}");
        //}
        //SlotPrefab[] baseLine = gamePrefab.BaseLine;
        //foreach (var item in baseLine)
        //{
        //    Debug.Log($"baseline item.name = {item.name}");
        //}
    }
}
