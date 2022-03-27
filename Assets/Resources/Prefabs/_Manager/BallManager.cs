using System.Collections.Generic;
using UnityEngine;

public class BallManager : ManagedPool<BallPrefab>
{
    public int BallVariation { get; set; } = 0;

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
        ball.BallColor = SlotAttribute.none;
        ball.Drop(base.Release, ball);
    }

    public override void Release(BallPrefab ball)
    {
        ball.BallColor = SlotAttribute.none;
        base.Release(ball);
    }

    public Dictionary<int, BallPrefab> Data
        => dictionary;

    public void ToPunch(List<SlotPrefab> list)
    {
        foreach (var e in list)
            e.PunchScale();
    }

    public void DropAndClear()
    {
        foreach (var pair in dictionary)
            Drop(pair.Value);
    }

    private BallPrefab SetColor(BallPrefab ball, SlotAttribute color = SlotAttribute.none)
    {
        ball.BallColor = color;
        return ball;
    }

    private SlotAttribute RandomColor
        => (SlotAttribute)Random.Range(0, BallVariation);
}
