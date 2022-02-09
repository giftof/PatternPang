using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pattern.Configs;
using Pattern.Objects;

public class BoardManager : ManagedPool<SlotPrefab>
{
    public BallManager ballManager;
    public (uint Row, uint Column) Size { get; set; } = (0, 0);
    private Vector2 m_slotSize;
    private float m_widthUnit;
    private List<SlotPrefab> m_baseLine;

    protected override void Awake()
    {
        base.Awake();
        m_slotSize = pool.prefab.GetComponent<RectTransform>().sizeDelta;
        m_widthUnit = m_slotSize.x * 0.75f;
        m_baseLine = new List<SlotPrefab>();
    }

    public override void Clear()
    {
        ballManager.Clear();
        base.Clear();
        m_baseLine.Clear();
    }

    public void Create()
    {
        Clear();
        PutSlots();
        UnitOffset();
    }

    /* at least 3 x 3 brard */
    private void UnitOffset()
    {
        //float[] distance = new float[5];
        //distance[0] = Vector3.Distance(m_slotArray[0].transform.position, m_slotArray[Size.Column * 2 + 1].transform.position);
        //distance[1] = Vector3.Distance(m_slotArray[0].transform.position, m_slotArray[2].transform.position);
        //distance[2] = Vector3.Distance(m_slotArray[0].transform.position, m_slotArray[Size.Column + 1].transform.position);
        //distance[3] = Vector3.Distance(m_slotArray[0].transform.position, m_slotArray[Size.Column].transform.position);
        //distance[4] = Vector3.Distance(m_slotArray[0].transform.position, m_slotArray[1].transform.position);
        //float min = Mathf.Min(distance[0], distance[1], distance[2]);
        //float max = Mathf.Max(distance[3], distance[4]);

        //CONST.MAX_DISTANCE = (min + max) * .5f;
        //CONST.DIRECTION_OFFSET[0] = m_slotArray[1].transform.position - m_slotArray[0].transform.position;
        //CONST.DIRECTION_OFFSET[3] = CONST.DIRECTION_OFFSET[0] * -1;
        //CONST.DIRECTION_OFFSET[1] = m_slotArray[Size.Column].transform.position - m_slotArray[0].transform.position;
        //CONST.DIRECTION_OFFSET[4] = CONST.DIRECTION_OFFSET[1] * -1;
        //CONST.DIRECTION_OFFSET[2] = CONST.DIRECTION_OFFSET[1] + Vector3.down * CONST.DIRECTION_OFFSET[1].y * 2;
        //CONST.DIRECTION_OFFSET[5] = CONST.DIRECTION_OFFSET[2] * -1;
    }

    private bool IsFloating(uint row)
        => row % 2 == CONST.EVEN_COLUMN_UP;

    //private void SetBallGenerator()
    //{
    //    BaseLine.Select(e => {
    //        while (true)
    //        {
    //            SlotPrefab hit = Ray.Instance.Shot(e.transform.position + CONST.DIRECTION_OFFSET[(int)ClockWise.up]);

    //            if (hit != null)
    //                e = hit;
    //            else
    //                break;
    //        }
    //        e.GetComponent<Image>().raycastTarget = false;
    //        e.Generate = GenerateBall;
    //        e.Slot.Color = SlotAttribute.generator;
    //        return e;
    //    }).ToArray();
    //}

    //private void GenerateBall(SlotPrefab destination)
    //{
    //    if (destination.Child != null)
    //        return;

    //    StartCoroutine(CreateBall(destination));
    //}

    //IEnumerator CreateBall(SlotPrefab destination)
    //{
    //    while (BallPrefab.TweeningCount > 0)
    //        yield return null;

    //    BallPrefab ball = ballManager.Request();
    //    ball.transform.SetParent(destination.transform.parent);
    //    ball.transform.position = destination.transform.position;
    //    destination.Child = ball;
    //}

    private void PutSlots()
    {
        Vector3 beginPosition = Vector3.left * (Size.Row - 1) * m_widthUnit * .5f;
        Vector3 currentPosition = beginPosition;

        for (uint w = 0; w < Size.Row; ++w)
        {
            currentPosition += IsFloating(w) ? Vector3.up * m_slotSize.y * .5f : Vector3.zero;
            for (uint h = 0; h < Size.Column; ++h)
            {
                Request(transform, currentPosition);
                currentPosition += Vector3.up * m_slotSize.y;
            }
            currentPosition = beginPosition + Vector3.right * (w + 1) * m_widthUnit;
        }
    }

}
