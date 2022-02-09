/*
 * Create: [Game.cs] on Thu Jan 27 2022 4:58:52 PM
 * Author: [cloudeven@gmail.com]
 *
 * Copyright (c) 2022 [noname]
 */

using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Pattern.Managers;
using Pattern.Objects;
using Pattern.Configs;
using DG.Tweening;


public class Game : MonoBehaviour
{
    [SerializeField] GameLogic gameLogic;
    [SerializeField] BoardManager boardManager;
    [SerializeField] Button generate;
    [SerializeField] Button clear;
    [SerializeField] Button re;
    [SerializeField] Text score;
    [SerializeField] Image progressBar;
    [SerializeField] EventSystem eventSystem;
    [SerializeField] GameObject gameOverPannel;

    void Start()
    {
        boardManager.Size = CONST.SIZE1;
        boardManager.ballManager.BallVariation = CONST.LEVEL2;

        gameLogic.Size = CONST.SIZE1;
        gameLogic.BallVar = CONST.LEVEL2;
        SetButtonAction();
    }

    private void SetButtonAction()
    {
        generate.onClick.AddListener(gameLogic.Initialize);
        generate.onClick.AddListener(BeginTimer);
        clear.onClick.AddListener(gameLogic.ClearBoard);
        re.onClick.AddListener(gameLogic.ClearBall);
    }

    private void BeginTimer()
    {
        progressBar.fillAmount = 1;
        DOTween.Kill("timer");
        DOTween.To(() => progressBar.fillAmount, x => progressBar.fillAmount = x, 0, 30).SetId("timer")
            .OnComplete( ()=> {
                StartCoroutine(FIN());
            });
    }

    IEnumerator FIN()
    {
        gameOverPannel.SetActive(true);
        eventSystem.enabled = false;

        yield return new WaitForSecondsRealtime(0.5f);
        gameLogic.ClearBall();
        yield return new WaitForSecondsRealtime(4.5f);

        eventSystem.enabled = true;
        gameOverPannel.SetActive(false);
    }

    private void Update()
    {
        if (eventSystem.enabled)
            DOTween.Play("timer");
        if (!eventSystem.enabled)
            DOTween.Pause("timer");

        score.text = gameLogic.Score.ToString();
    }
}
