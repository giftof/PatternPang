using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Pattern.Configs;
using Pattern.Managers;

public class BoardManager : ManagedPool<SlotPrefab>
{
    [SerializeField] BallManager m_ballHandler;
    [SerializeField] CoverManager m_coverHandler;
    private PatternHandler m_patternHandler;
    private Action m_beginAction;
    private Action m_addAction;
    private Action m_removeAction;
    private DELEGATE_T<SlotPrefab> m_bombAction;

    private Vector2 m_slotSize;
    private float m_widthUnit;
    private float m_lineThick = 1f;
    // private float m_lineThick = 0.93f;
    /*private float m_lineThick = 1;*/

    public (uint Row, uint Column) Size { get; set; } = (0, 0);
    public BallManager ballManager;

    protected override void Awake()
    {
        base.Awake();
        m_slotSize = pool.prefab.GetComponent<RectTransform>().sizeDelta * m_lineThick;
        m_widthUnit = m_slotSize.x * CONST.HEXAGON_WIDTH_RATIO;
    }

    public PatternHandler SetPatternHandler
    {
        set => m_patternHandler = value;
    }

    public DELEGATE_T<SlotPrefab> SetBombAction
    {
        set => m_bombAction = value;
    }

    public Action SetRemoveAction
    {
        set => m_removeAction = value;
    }

    public Action SetAddAction
    {
        set => m_addAction = value;
    }

    public Action SetBeginAction
    {
        set => m_beginAction = value;
    }

    public override void Clear()
    {
        ClearChild();
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

    public IReadOnlyDictionary<int, SlotPrefab> Data
        => dictionary;

    private void PublishBoard()
    {
        Vector3 beginPosition = (Size.Row - 1) * .5f * m_widthUnit * Vector3.left;
        Vector3 currentPosition = beginPosition;

        for (uint w = 0; w < Size.Row; ++w)
        {
            currentPosition += m_slotSize.y * Vector3.up * (IsHalfFloating(w) ? 1 : .5f);
            for (uint h = 0; h < Size.Column; ++h)
            {
                SlotPrefab slot = Request(transform, currentPosition);
                slot.id = Size.Column * w + h;
                slot.SetPatternHandler = m_patternHandler;
                slot.SetBeginAction = m_beginAction;
                slot.SetAddAction = m_addAction;
                slot.SetRemoveAction = m_removeAction;
                slot.SetBombAction = m_bombAction;
// Debug.Log($"height = {m_slotSize.y}");
                currentPosition += Vector3.up * m_slotSize.y;
                slot.name = slot.GetInstanceID().ToString(); /* for test */

                if (h.Equals(Size.Column - 1))
                    GeneratorCap(slot);
                else
                    slot.Generate = null;
            }
            currentPosition = beginPosition + (w + 1) * m_widthUnit * Vector3.right;
        }
    }

    private void GeneratorCap(SlotPrefab cap)
    {
        m_coverHandler.Request(m_coverHandler.transform).transform.position = cap.transform.position;
        cap.Generate = MakeChild;
    }

    private bool MakeChild(SlotPrefab slot)
    {
        if (slot.Child == null)
        {
            slot.Child = m_ballHandler.Request(slot.transform.parent, slot.transform.localPosition);
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

        CONST.OFFSET = up.y;
        CONST.MAX_DISTANCE = (min + max) * .5f;
        CONST.DIRECTION_OFFSET[0] = up;
        CONST.DIRECTION_OFFSET[1] = up * .5f + right;
        CONST.DIRECTION_OFFSET[2] = -up * .5f + right;
        CONST.DIRECTION_OFFSET[3] = -up;
        CONST.DIRECTION_OFFSET[4] = -up * .5f - right;
        CONST.DIRECTION_OFFSET[5] = up * .5f - right;
    }

    private bool IsHalfFloating(uint row)
        => row % 2 == CONST.EVEN_COLUMN_UP;
}
