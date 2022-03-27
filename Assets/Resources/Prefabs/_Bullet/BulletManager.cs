using UnityEngine;
using DG.Tweening;

public class BulletManager : ManagedPool<BulletPrefab>
{
    protected override void Awake() {
        base.Awake();
        pool.prefab = Resources.Load<GameObject>("Prefabs/_Bullet/BulletPrefab");
    }

    public void Move(BulletPrefab bullet, CharactorPrefab to)
    {
        float distance = Vector3.Distance(bullet.transform.position, to.transform.position) * .7f;
        Sequence sequence = DOTween.Sequence();

        sequence.Append(bullet.transform.DOMove(to.Position, distance * .15f).SetEase(Ease.InCubic).OnComplete(() => to.Scaling()));
        sequence.AppendInterval(0.1f);
        sequence.OnComplete(() => Release(bullet));
    }
}
