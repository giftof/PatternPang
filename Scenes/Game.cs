using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Pattern.Configs;
using Pattern.Tools;
using DG.Tweening;


public class Game : MonoBehaviour
{
    [SerializeField] GameLogic m_gameLogic;
    [SerializeField] BoardManager m_boardManager;
    [SerializeField] Button m_generate;
    [SerializeField] Button m_clear;
    [SerializeField] Button m_re;
    [SerializeField] Text m_score;
    [SerializeField] Image m_progressBar;
    [SerializeField] EventSystem m_eventSystem;
    [SerializeField] GameObject m_gameOverPannel;

    //[SerializeField] Canvas m_canvas;
    [SerializeField] GameObject m_fullBackground;
    [SerializeField] GameObject m_safeBackground;

    [SerializeField] Text m_screen;
    [SerializeField] Text m_safeArea0;
    [SerializeField] Text m_safeArea1;

    [SerializeField] Text min;
    [SerializeField] Text max;
    [SerializeField] Text size;
    //[SerializeField] GameObject m_backGround;

    void Start()
    {
        SetGameLevel();
        SetButtonAction();
        SafeScreen();

        TEST_SAFE_SCREEN();
    }

    private void TEST_SAFE_SCREEN()
    {
        RectTransform rect = m_safeBackground.GetComponent<RectTransform>();

        m_screen.text = Screen.width.ToString() + "/" + Screen.height.ToString();
        m_safeArea0.text = Screen.safeArea.min.ToString();
        m_safeArea1.text = Screen.safeArea.max.ToString();

        min.text = rect.offsetMin.ToString();
        max.text = rect.offsetMax.ToString();
        size.text = rect.rect.size.ToString();
    }

    private void SafeScreen()
    {
        (Vector2 min, Vector2 max) safeArea = new SafeScreen().RectOffset();
        RectTransform rect = m_safeBackground.GetComponent<RectTransform>();
        RectTransform fullRect = m_fullBackground.GetComponent<RectTransform>();

        rect.offsetMin = safeArea.min;
        rect.offsetMax = safeArea.max;

        fullRect.offsetMin = Vector2.zero;
        fullRect.offsetMax = Vector2.zero;
    }

    private void SetGameLevel()
    {
        m_boardManager.Size = CONST.SIZE1;
        m_boardManager.ballManager.BallVariation = CONST.LEVEL1; // prefer is 1, 2
    }

    private void SetButtonAction()
    {
        m_generate.onClick.AddListener(m_gameLogic.CreateGame);
        m_generate.onClick.AddListener(BeginTimer);
        m_clear.onClick.AddListener(m_gameLogic.ClearGame);
        m_re.onClick.AddListener(m_gameLogic.ClearBall);
    }

    private void BeginTimer()
    {
        m_progressBar.fillAmount = 1;
        DOTween.Kill(m_progressBar.GetInstanceID());
        DOTween.To(() => m_progressBar.fillAmount, x => m_progressBar.fillAmount = x, 0, CONST.DURATION_PLAY_TIME).SetId(m_progressBar.GetInstanceID())
            .OnComplete( ()=> { StartCoroutine(FIN()); });
    }

    IEnumerator FIN()
    {
        m_eventSystem.enabled = false;
        m_gameOverPannel.SetActive(true);

        yield return new WaitForSecondsRealtime(0.5f);

        m_gameLogic.Finish = true;
        m_gameLogic.ClearBall();
        yield return new WaitForSecondsRealtime(4.5f);

        m_gameOverPannel.SetActive(false);
        m_eventSystem.enabled = true;
    }

    private void Update()
    {
        if (m_eventSystem.enabled)
            DOTween.Play(m_progressBar.GetInstanceID());
        else
            DOTween.Pause(m_progressBar.GetInstanceID());
        m_score.text = m_gameLogic.Score.ToString();
    }
}
