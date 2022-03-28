using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class NewTestScript
{
    // A Test behaves as an ordinary method
    [Test]
    public void NewTestScriptSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return new MonoBehaviourTest<PatternTest>();
        // yield return null;
    }
}

public class PatternTest: MonoBehaviour, IMonoBehaviourTest
{
    private Camera m_camera;
    private Canvas m_canvas;
    private GameLogic m_gameLogic;
    private Button m_generate;
    private ScoreBar m_progressBar;
    private EventSystem m_eventSystem;
    private GameObject m_gameOverPannel;
    private GameObject m_fullBackground;
    private GameObject m_safeBackground;

    private float m_timerDuration;
    private float m_fillAmount;

    void Awake() {
        m_camera = (new GameObject()).AddComponent<Camera>();
        m_camera.orthographic = true;

        m_canvas = Instantiate(Resources.Load<Canvas>("Prefabs/Canvas"));
        m_canvas.worldCamera = m_camera;

        m_fullBackground = Instantiate(Resources.Load<GameObject>("Prefabs/Background"), m_canvas.transform);
        m_safeBackground = Instantiate(Resources.Load<GameObject>("Prefabs/Background"), m_canvas.transform);

        m_eventSystem = (new GameObject()).AddComponent<EventSystem>();
        m_eventSystem.gameObject.AddComponent<StandaloneInputModule>();

        m_gameLogic = Instantiate(Resources.Load<GameLogic>("Prefabs/GameLogic"), m_safeBackground.transform);
        m_gameLogic.EventSystem = m_eventSystem;

        m_progressBar = Instantiate(Resources.Load<ScoreBar>("Prefabs/ScoreBar"), m_safeBackground.transform);
        m_generate = Instantiate(Resources.Load<Button>("Prefabs/Button"), m_safeBackground.transform);

        m_gameOverPannel = Instantiate(Resources.Load<GameObject>("Prefabs/MessagePannel"), m_safeBackground.transform);
        m_gameOverPannel.SetActive(false);
        Instantiate(Resources.Load<Ray>("Prefabs/Ray"), m_safeBackground.transform);
    }

    void Start() {
        SetGameLevel();
        SetDefaultTimer();
        SetButtonAction();
        SetSafeScreen();

        StartCoroutine(BEGIN_TEST());
    }

    private void SetGameLevel() {
        m_gameLogic.Size = CONST.SIZE75;
        m_gameLogic.BallVariation = CONST.LEVEL3; // prefer is 1, 2
    }

    private void SetSafeScreen() {
        (Vector2 min, Vector2 max)safeArea = new SafeScreen().RectOffset();
        RectTransform safeRect = m_safeBackground.GetComponent<RectTransform>();
        RectTransform fullRect = m_fullBackground.GetComponent<RectTransform>();

        safeRect.offsetMin = safeArea.min;
        safeRect.offsetMax = safeArea.max;

        fullRect.offsetMin = Vector2.zero;
        fullRect.offsetMax = Vector2.zero;
    }

    private void SetButtonAction() {
        m_generate
            .onClick
            .AddListener(() => {
                if (m_gameLogic.Finish) {
                    m_gameLogic.CreateGame();
                    SetDefaultTimer();
                    BeginTimer();
                }
            });
    }

    private void SetDefaultTimer() => SetTimer(1, CONST.DURATION_PLAY_TIME);

    private void SetTimer(float fillAmount, float duration) {
        m_timerDuration = duration;
        m_fillAmount = fillAmount;
    }

    private void BeginTimer() {
        m_progressBar.FillAmount = m_fillAmount;

        DOTween.Kill(m_progressBar.GetInstanceID());
        DOTween
            .To(
                () => m_progressBar.FillAmount,
                x => m_progressBar.FillAmount = x,
                0,
                m_timerDuration
            )
            .SetId(m_progressBar.GetInstanceID())
            .SetEase(Ease.Linear)
            .SetUpdate(true)
            .OnComplete(() => {
                StartCoroutine(FIN());
            });
    }

    private void UpdateTimer() {
        float increment = 1 / CONST.DURATION_PLAY_TIME;

        m_fillAmount = m_progressBar.FillAmount;
        m_fillAmount += increment * m_gameLogic.BonusTimeSecond;
        m_fillAmount = Mathf.Min(m_fillAmount, 1);
        m_timerDuration = CONST.DURATION_PLAY_TIME * m_fillAmount;
        m_gameLogic.BonusTimeSecond = CONST.BONUS_TIMER_BEGIN_VALUE;

        BeginTimer();
    }

    IEnumerator FIN() {
        m_eventSystem.enabled = false;
        m_gameOverPannel.SetActive(true);
        m_gameOverPannel.transform.SetAsLastSibling();

        yield return new WaitForSecondsRealtime(.5f);

        m_gameLogic.Finish = true;
        m_gameLogic.ClearBall();
        yield return new WaitForSecondsRealtime(4.5f);

        m_gameOverPannel.SetActive(false);
        m_eventSystem.enabled = true;
    }

    public bool IsTestFinished
    {
        get { return int.Parse(m_progressBar.Score) > 100; }
    }

    private void Update() {

        if (m_gameLogic.BonusTimeSecond > 0) 
            UpdateTimer();
        
        if (m_eventSystem.enabled) 
            DOTween.Play(m_progressBar.GetInstanceID());
        else 
            DOTween.Pause(m_progressBar.GetInstanceID());
        
        m_progressBar.Score = m_gameLogic
            .Score
            .ToString();
    }

    IEnumerator BEGIN_TEST()
    {
        yield return new WaitForSecondsRealtime(.5f);

        if (m_gameLogic.Finish) {
            m_gameLogic.CreateGame();
            SetDefaultTimer();
            BeginTimer();
        }
    }



}