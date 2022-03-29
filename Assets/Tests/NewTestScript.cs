using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public delegate bool TEST_DELEGATE();

public class NewTestScript
{
    Setup m_pattern;

    // A Test behaves as an ordinary method
    [Test]
    public void NewTestScriptSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest, Order(0)]
    public IEnumerator MakeScene()
    {
        if (m_pattern == null)
            yield return m_pattern = (new GameObject()).AddComponent<Setup>();
    }

    [UnityTest, Order(1)]
    public IEnumerator NewTestScriptWithEnumeratorPasses2()
    {
        if (m_pattern == null)
            yield return m_pattern = (new GameObject()).AddComponent<Setup>();

        yield return new WaitForSecondsRealtime(.5f);
        m_pattern.BEGIN_SHORT_TEST();

        while(!m_pattern.ENABLE())
            yield return null;
        
        m_pattern.BEGIN_SHORT_TEST();
    }

    [UnityTest, Order(2)]
    public IEnumerator NewTestScriptWithEnumeratorPasses3()
    {
        if (m_pattern == null)
            yield return m_pattern = (new GameObject()).AddComponent<Setup>();

        SlotPrefab[] match = m_pattern.FLOWER_SHAPE_MATCH();
        SlotPrefab[] first = m_pattern.FIRST_COLOR_SET();
        SlotPrefab gene = m_pattern.FIRST_GENERATOR();

        gene.OnPointerDown(null);

        if (match != null)
        {
            yield return new WaitForSecondsRealtime(.5f);
            match[0].OnPointerDown(null);
            yield return new WaitForSecondsRealtime(.5f);
            match[1].OnPointerEnter(null);
            yield return new WaitForSecondsRealtime(.5f);
            match[1].OnPointerUp(null);
        }

        if (match != null)
        {
            yield return new WaitForSecondsRealtime(.5f);
            match[0].OnPointerEnter(null);
            yield return new WaitForSecondsRealtime(.5f);
            match[1].OnPointerDown(null);
            yield return new WaitForSecondsRealtime(.5f);
            match[1].OnPointerUp(null);
        }

        if (match != null)
        {
            yield return new WaitForSecondsRealtime(.5f);
            match[0].OnPointerEnter(null);
            yield return new WaitForSecondsRealtime(.5f);
            match[1].OnPointerEnter(null);
            yield return new WaitForSecondsRealtime(.5f);
            match[2].OnPointerUp(null);
        }

        if (first != null)
        {
            yield return new WaitForSecondsRealtime(.5f);
            first[0].OnPointerDown(null);
            yield return new WaitForSecondsRealtime(.5f);
            first[first.Length - 1].OnPointerEnter(null);
            yield return new WaitForSecondsRealtime(.5f);
            first[1].OnPointerUp(null);
        }

        if (match != null)
        {
            yield return new WaitForSecondsRealtime(.5f);
            match[1].OnPointerDown(null);
            yield return new WaitForSecondsRealtime(.5f);
            match[0].OnPointerEnter(null);
            yield return new WaitForSecondsRealtime(.5f);
            match[2].OnPointerEnter(null);
            yield return new WaitForSecondsRealtime(.5f);
            match[0].OnPointerEnter(null);
            yield return new WaitForSecondsRealtime(.5f);
            match[0].OnPointerUp(null);
        }

        while (!m_pattern.m_gameOverPannel.activeSelf)
            yield return null;

        while (m_pattern.m_gameOverPannel.activeSelf)
            yield return null;
    }

    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPasses4()
    {
        yield return new WaitForSecondsRealtime(.5f);
        
        PointerEventData pointerEventData = new PointerEventData(m_pattern.m_eventSystem);
        pointerEventData.selectedObject = m_pattern.m_generate.gameObject;
        m_pattern.m_generate.OnPointerClick(pointerEventData);

        while(!m_pattern.ENABLE())
            yield return null;
    }

    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPasses5()
    {
        SlotPrefab[] match = m_pattern.FLOWER_SHAPE_MATCH();

        if (match != null)
        {
            Debug.Log("step 1");
            yield return new WaitForSecondsRealtime(.5f);
            match[1].OnPointerDown(null);
            yield return new WaitForSecondsRealtime(.5f);
            match[0].OnPointerEnter(null);
            yield return new WaitForSecondsRealtime(.5f);
            match[2].OnPointerEnter(null);
            yield return new WaitForSecondsRealtime(.5f);
            match[2].OnPointerUp(null);
        }
        
        while(!m_pattern.ENABLE())
            yield return null;
    }

    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPasses6()
    {
        SlotPrefab[] match = m_pattern.FLOWER_SHAPE_MATCH();

        if (match != null)
        {
            Debug.Log("step 2");
            yield return new WaitForSecondsRealtime(.5f);
            match[1].OnPointerDown(null);
            yield return new WaitForSecondsRealtime(.5f);
            match[0].OnPointerEnter(null);
            yield return new WaitForSecondsRealtime(.5f);
            match[2].OnPointerEnter(null);
            yield return new WaitForSecondsRealtime(.5f);
            match[2].OnPointerUp(null);
        }
        
        while(!m_pattern.ENABLE())
            yield return null;

        Debug.Log("finish 5sec.");
        yield return new WaitForSecondsRealtime(1f);
        Debug.Log("finish 4sec.");
        yield return new WaitForSecondsRealtime(1f);
        Debug.Log("finish 3sec.");
        yield return new WaitForSecondsRealtime(1f);
        Debug.Log("finish 2sec.");
        yield return new WaitForSecondsRealtime(1f);
        Debug.Log("finish 1sec.");
        yield return new WaitForSecondsRealtime(1f);
    }
}


public partial class Setup
{
    public void BEGIN_TEST()
    {
        if (m_gameLogic.Finish) 
        {
            m_gameLogic.CreateGame();
            SetDefaultTimer();
            BeginTimer();
        }
    }

    public void BEGIN_SHORT_TEST()
    {
        if (m_gameLogic.Finish) 
        {
            m_gameLogic.CreateGame();
            SetShortTimer();
            BeginTimer();
        }
    }

    public bool ENABLE()
        => m_eventSystem.enabled;

    public Vector3 Position(SlotPrefab target)
    {
        return target.transform.position;
    }

    public SlotPrefab[] FIRST_COLOR_SET()
    {
        SlotPrefab begin = m_gameLogic.Board.First();
        return m_gameLogic.Board.Where(e => e.Child.BallColor.Equals(begin.Child.BallColor)).Select(e => e).ToArray();
    }

    public SlotPrefab FIRST_GENERATOR()
        => m_gameLogic.Board.First(e => e.Generate != null);

    public SlotPrefab[] FLOWER_SHAPE_MATCH()
    {
        foreach (var slot in m_gameLogic.Board)
        {
            if (slot.Generate != null)
                continue;
            
            SlotPrefab begin = slot;

            var list = (from offset in CONST.DIRECTION_OFFSET
                        let hit = Ray.Instance.Shoot(begin.transform.position + offset)
                        where hit != null && hit.Generate == null && hit.Child.BallColor.Equals(begin.Child.BallColor)
                        select hit).ToList();
            list.Insert(0, begin);

            if (list.Count() >= 3)
                return list.ToArray();
        }

        return null;
    }
}



public partial class Setup: MonoBehaviour
{
    public GameLogic m_gameLogic;
    private Camera m_camera;
    private Canvas m_canvas;
    public Button m_generate;
    private ScoreBar m_progressBar;
    public EventSystem m_eventSystem;
    public GameObject m_gameOverPannel;
    private GameObject m_fullBackground;
    private GameObject m_safeBackground;

    private float m_timerDuration;
    private float m_fillAmount;

    void Awake() 
    {
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

    void Start() 
    {
        SetGameLevel();
        SetDefaultTimer();
        SetButtonAction();
        SetSafeScreen();
    }

    private void SetGameLevel() 
    {
        m_gameLogic.Size = CONST.SIZE75;
        m_gameLogic.BallVariation = CONST.LEVEL3; // prefer is 1, 2
    }

    private void SetTimer(float fillAmount, float duration) 
    {
        m_timerDuration = duration;
        m_fillAmount = fillAmount;
    }

    private void SetDefaultTimer() => SetTimer(1, CONST.DURATION_PLAY_TIME);
    private void SetShortTimer() => SetTimer(1, 10);

    private void SetButtonAction() 
    {
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

    private void SetSafeScreen() 
    {
        (Vector2 min, Vector2 max)safeArea = new SafeScreen().RectOffset();
        RectTransform safeRect = m_safeBackground.GetComponent<RectTransform>();
        RectTransform fullRect = m_fullBackground.GetComponent<RectTransform>();

        safeRect.offsetMin = safeArea.min;
        safeRect.offsetMax = safeArea.max;

        fullRect.offsetMin = Vector2.zero;
        fullRect.offsetMax = Vector2.zero;
    }

    private void BeginTimer() 
    {
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

    IEnumerator FIN() 
    {
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

    private void UpdateTimer() 
    {
        float increment = 1 / CONST.DURATION_PLAY_TIME;

        m_fillAmount = m_progressBar.FillAmount;
        m_fillAmount += increment * m_gameLogic.BonusTimeSecond;
        m_fillAmount = Mathf.Min(m_fillAmount, 1);
        m_timerDuration = CONST.DURATION_PLAY_TIME * m_fillAmount;
        m_gameLogic.BonusTimeSecond = CONST.BONUS_TIMER_BEGIN_VALUE;

        BeginTimer();
    }

    private void Update() 
    {
        if (m_gameLogic.BonusTimeSecond > 0) 
            UpdateTimer();
        
        if (m_eventSystem.enabled) 
            DOTween.Play(m_progressBar.GetInstanceID());
        else 
            DOTween.Pause(m_progressBar.GetInstanceID());
        
        m_progressBar.Score = m_gameLogic.Score.ToString();
    }
}
