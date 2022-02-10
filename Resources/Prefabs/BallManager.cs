using System.Collections.Generic;
using UnityEngine;
using Pattern.Configs;

public class BallManager : ManagedPool<BallPrefab>
{
    public static BallManager Instance = null;

    public uint BallVariation { get; set; } = 0;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

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

    public Dictionary<int, BallPrefab> Data
        => dictionary;

    public void DropAndClear()
    {
        foreach (var pair in dictionary)
            Drop(pair.Value);
    }

    private BallPrefab SetColor(BallPrefab ball, SlotAttribute color = SlotAttribute.none)
    {
        ball.Color = color;
        return ball;
    }

    private SlotAttribute RandomColor
        => (SlotAttribute)Random.Range(0, BallVariation);
}
