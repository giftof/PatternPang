
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pattern.Configs;
using Pattern.Objects;



public class BoardManager : MonoBehaviour
{
    [SerializeField] ObjectPool slotPool;
    [SerializeField] ObjectPool ballPool;

    public SlotPrefab[] BaseLine { get; set; } = null;
    public (uint Row, uint Column) Size { get; set; } = (0, 0);
    public uint BallVariation { get; set; } = 0;

    private Vector2 m_slotSize;
    private float m_widthUnit;
    private SlotPrefab[] m_slotArray;



    private void Awake()
    {
        m_slotSize = slotPool.prefab.GetComponent<RectTransform>().sizeDelta;
        m_widthUnit = m_slotSize.x * 0.75f;
    }

    public void GenerateBoard()
    {
        ClearBoard();
        CreateBoard(GameLogic.Instance.AfterDraw);
        PresetOffset();
    }

    private void CreateBoard(Action finishAction)
    {
        Vector3 beginPosition = Vector3.left * (Size.Row - 1) * m_widthUnit * .5f;
        Vector3 currentPosition = beginPosition;

        BaseLine = new SlotPrefab[Size.Row];
        m_slotArray = new SlotPrefab[Size.Row * Size.Column];

        for (uint w = 0; w < Size.Row; ++w)
        {
            if (IsFloating(w))
                currentPosition += Vector3.up * m_slotSize.y * .5f;
            for (uint h = 0; h < Size.Column; ++h)
            {
                if (slotPool.Request<SlotPrefab>(transform, currentPosition) is SlotPrefab slot)
                {
                    slot.Initialize(new Slot(w * Size.Column + h), finishAction);

                    m_slotArray[slot.Slot.Id] = slot;
                    currentPosition += Vector3.up * m_slotSize.y;

                    BaseLine[w] = BaseLine[w] ?? slot;
                }
                else
                    throw new Exception("slotPool make Exception");
            }
            currentPosition = beginPosition + Vector3.right * (w + 1) * m_widthUnit;
        }
    }

    private void ClearBoard()
    {
        //LineManager.Instance.Clear();

        if (m_slotArray != null)
            foreach (var item in m_slotArray)
            {
                if (item.Ball != null)
                {
                    item.Ball.Color = SlotAttribute.none;
                    ballPool.Release(item.Ball.gameObject);
                }

                item.Ball = null;
                item.Slot.Color = SlotAttribute.none;
                slotPool.Release(item.gameObject);
            }

        m_slotArray = null;
        BaseLine = null;
    }


    public void ClearBall()
    {
        //eventSystem.enabled = false;
        LineManager.Instance.Clear();

        if (m_slotArray != null)
            foreach (var item in m_slotArray)
            {
                if (item.Ball != null)
                    item.Ball.Drop(ballPool.Release, item.Ball.gameObject);
                item.Ball = null;
            }

        //StartCoroutine(WaitDrop(() => { Finish(); }));
    }

    /* at least 3 x 3 brard */
    private void PresetOffset()
    {
        float[] distance = new float[5];
        distance[0] = Vector3.Distance(m_slotArray[0].transform.position, m_slotArray[Size.Column * 2 + 1].transform.position);
        distance[1] = Vector3.Distance(m_slotArray[0].transform.position, m_slotArray[2].transform.position);
        distance[2] = Vector3.Distance(m_slotArray[0].transform.position, m_slotArray[Size.Column + 1].transform.position);
        distance[3] = Vector3.Distance(m_slotArray[0].transform.position, m_slotArray[Size.Column].transform.position);
        distance[4] = Vector3.Distance(m_slotArray[0].transform.position, m_slotArray[1].transform.position);
        float min = Mathf.Min(distance[0], distance[1], distance[2]);
        float max = Mathf.Max(distance[3], distance[4]);

        CONST.MAX_DISTANCE = (min + max) * .5f;
        CONST.DIRECTION_OFFSET[0] = m_slotArray[1].transform.position - m_slotArray[0].transform.position;
        CONST.DIRECTION_OFFSET[3] = CONST.DIRECTION_OFFSET[0] * -1;
        CONST.DIRECTION_OFFSET[1] = m_slotArray[Size.Column].transform.position - m_slotArray[0].transform.position;
        CONST.DIRECTION_OFFSET[4] = CONST.DIRECTION_OFFSET[1] * -1;
        CONST.DIRECTION_OFFSET[2] = CONST.DIRECTION_OFFSET[1] + Vector3.down * CONST.DIRECTION_OFFSET[1].y * 2;
        CONST.DIRECTION_OFFSET[5] = CONST.DIRECTION_OFFSET[2] * -1;
    }

    private bool IsFloating(uint row)
        => row % 2 == CONST.EVEN_COLUMN_UP;
}
