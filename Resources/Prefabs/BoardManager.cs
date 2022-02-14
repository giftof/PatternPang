using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Pattern.Configs;

public class BoardManager : ManagedPool<SlotPrefab>
{
    public static BoardManager Instance = null;

    public (uint Row, uint Column) Size { get; set; } = (0, 0);
    public BallManager ballManager;
    private Vector2 m_slotSize;
    private float m_widthUnit;
    private List<SlotPrefab> m_bottomList;
    private float m_lineThick = 0.93f;

    protected override void Awake()
    {
        base.Awake();
        m_slotSize = pool.prefab.GetComponent<RectTransform>().sizeDelta * m_lineThick;
        m_widthUnit = m_slotSize.x * CONST.HEXAGON_WIDTH_RATIO;
        m_bottomList = new List<SlotPrefab>();
        Instance = this;
    }

    public override void Clear()
    {
        ClearChild();
        m_bottomList.Clear();
        ballManager.Clear();
        base.Clear();
    }
    
    public void ClearChild()
    {
        foreach (var pair in dictionary)
            pair.Value.Child = null;
    }

    public void Create()
    {
        Clear();
        PublishBoard();
        UnitOffset();
    }

    public Dictionary<int, SlotPrefab> Data
        => dictionary;

    public IReadOnlyList<SlotPrefab> BottomLine
        => m_bottomList;

    private void PublishBoard()
    {
        Vector3 beginPosition = (Size.Row - 1) * .5f * m_widthUnit * Vector3.left;
        Vector3 currentPosition = beginPosition;

        for (uint w = 0; w < Size.Row; ++w)
        {
            currentPosition += IsFloating(w) ? .5f * m_slotSize.y * Vector3.up : Vector3.zero;
            for (uint h = 0; h < Size.Column; ++h)
            {
                SlotPrefab slot = Request(transform, currentPosition);
                currentPosition += Vector3.up * m_slotSize.y;
                slot.name = slot.GetInstanceID().ToString(); /* for test */

                if (h.Equals(0))
                    m_bottomList.Add(slot);
                if (h.Equals(Size.Column - 1))
                    slot.Generate = MakeChild;
                else
                    slot.Generate = null;
            }
            currentPosition = beginPosition + (w + 1) * m_widthUnit * Vector3.right;
        }
    }

    private bool MakeChild(SlotPrefab slot)
    {
        if (slot.Child == null)
        {
            slot.Child = BallManager.Instance.Request(slot.transform.parent, slot.transform.localPosition);
            return true;
        }
        return false;
    }

    private void UnitOffset()
    {
        float ratio = dictionary.First().Value.transform.localToWorldMatrix.m00;
        Vector3 right = m_widthUnit * ratio * Vector3.right;
        Vector3 up = m_slotSize.y * ratio * Vector3.up;

        float min = Mathf.Min(up.y * 2, right.x * 2, Vector3.Distance(up * 1.5f + right, Vector3.zero));
        float max = Mathf.Max(up.y, up.y * .5f + right.x);

        CONST.MAX_DISTANCE = (min + max) * .5f;
        CONST.DIRECTION_OFFSET[0] = up;
        CONST.DIRECTION_OFFSET[1] = up * .5f + right;
        CONST.DIRECTION_OFFSET[2] = -up * .5f + right;
        CONST.DIRECTION_OFFSET[3] = -up;
        CONST.DIRECTION_OFFSET[4] = -up * .5f - right;
        CONST.DIRECTION_OFFSET[5] = up * .5f - right;
    }

    private bool IsFloating(uint row)
        => row % 2 == CONST.EVEN_COLUMN_UP;
}
