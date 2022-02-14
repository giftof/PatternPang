using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Pattern.Managers;
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
        SetGameLevel();
        SetButtonAction();
    }

    private void SetGameLevel()
    {
        boardManager.Size = CONST.SIZE1;
        boardManager.ballManager.BallVariation = CONST.LEVEL2;
    }

    private void SetButtonAction()
    {
        generate.onClick.AddListener(gameLogic.CreateGame);
        generate.onClick.AddListener(BeginTimer);
        clear.onClick.AddListener(gameLogic.ClearGame);
        re.onClick.AddListener(gameLogic.ClearBall);
    }

    private void BeginTimer()
    {
        progressBar.fillAmount = 1;
        DOTween.Kill(progressBar.GetInstanceID());
        DOTween.To(() => progressBar.fillAmount, x => progressBar.fillAmount = x, 0, CONST.DURATION_PLAY_TIME).SetId(progressBar.GetInstanceID())
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
            DOTween.Play(progressBar.GetInstanceID());
        if (!eventSystem.enabled)
            DOTween.Pause(progressBar.GetInstanceID());

        score.text = gameLogic.Score.ToString();
    }
}
