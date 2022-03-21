using UnityEngine;
using DG.Tweening;

public class CharactorPrefab : MonoBehaviour
{
    [SerializeField] GameObject center;
    RectTransform baseRect;
    Vector2 jumpSize;
    float duration;

    private void Start()
    {
        baseRect = GetComponent<RectTransform>();
        jumpSize = Vector2.one * 30;
        duration = .5f;
    }

    public Vector3 Position
    {
        get => center.transform.position;
    }

    public float Duration
    {
        get => duration;
        set => duration = value;
    }

    public Vector2 JumpSize
    {
        get => jumpSize;
        set => jumpSize = value;
    }

    public void Scaling()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(baseRect.DOSizeDelta(jumpSize, duration)).SetEase(Ease.OutExpo);
        sequence.Append(baseRect.DOSizeDelta(default, duration / 2)).SetEase(Ease.OutExpo);
        sequence.Play();
    }
}
