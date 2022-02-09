using System.Linq;
using UnityEngine;
using Pattern.Configs;

public class BoardManager : ManagedPool<SlotPrefab>
{
    public BallManager ballManager;
    public (uint Row, uint Column) Size { get; set; } = (0, 0);
    private Vector2 m_slotSize;
    private float m_widthUnit;

    protected override void Awake()
    {
        base.Awake();
        m_slotSize = pool.prefab.GetComponent<RectTransform>().sizeDelta;
        m_widthUnit = m_slotSize.x * CONST.HEXAGON_WIDTH_RATIO;
    }

    public override void Clear()
    {
        ballManager.Clear();
        base.Clear();
    }

    public void Create()
    {
        Clear();
        PutSlots();
        UnitOffset();
    }

    private void PutSlots()
    {
        Vector3 beginPosition = (Size.Row - 1) * .5f * m_widthUnit * Vector3.left;
        Vector3 currentPosition = beginPosition;

        for (uint w = 0; w < Size.Row; ++w)
        {
            currentPosition += IsFloating(w) ? .5f * m_slotSize.y * Vector3.up : Vector3.zero;
            for (uint h = 0; h < Size.Column; ++h)
            {
                Request(transform, currentPosition);
                currentPosition += Vector3.up * m_slotSize.y;
            }
            currentPosition = beginPosition + (w + 1) * m_widthUnit * Vector3.right;
        }
    }

    private void UnitOffset()
    {
        Matrix4x4 matrix = dictionary.First().Value.transform.localToWorldMatrix;
        Vector3 right = m_widthUnit * matrix.m00 * Vector3.right;
        Vector3 up = m_slotSize.y * matrix.m00 * Vector3.up;

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
