using System;
using UnityEngine;
using DG.Tweening;

public class BulletManager : ManagedPool<BulletPrefab>
{
    protected override void Awake() 
    {
        base.Awake();
        pool.prefab = Resources.Load<GameObject>("Prefabs/_Bullet/BulletPrefab");
    }

    public void FireTo(Vector3 begin, Vector3 end, Action action)
    {
        float distance = Vector3.Distance(begin, end) * .7f;
        BulletPrefab bullet = Request();
        bullet.transform.position = begin;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(bullet.transform.DOMove(end, distance * .15f).SetEase(Ease.InCubic).OnComplete( () => action?.Invoke() ));
        sequence.AppendInterval(0.1f);
        sequence.OnComplete(() => Release(bullet));
    }
}
