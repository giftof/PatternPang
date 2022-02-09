using UnityEngine;
using Pattern.Configs;

public class BallManager : ManagedPool<BallPrefab>
{
    public uint BallVariation { get; set; } = 0;
    private static readonly int beginVariation = 1;

    protected override void Awake()
        => base.Awake();

    public override BallPrefab Request()
    {
        BallPrefab ball = base.Request();
        SetColor(ball, RandomColor);
        return ball;
    }

    public void Drop(BallPrefab ball)
    {
        ball.Color = SlotAttribute.none;
        ball.Drop(base.Release, ball);
    }

    public override void Release(BallPrefab ball)
    {
        ball.Color = SlotAttribute.none;
        base.Release(ball);
    }

    private BallPrefab SetColor(BallPrefab ball, SlotAttribute color = SlotAttribute.none)
    {
        ball.Color = color;
        return ball;
    }

    private SlotAttribute RandomColor
        => (SlotAttribute)Random.Range(beginVariation, BallVariation + beginVariation);
}
