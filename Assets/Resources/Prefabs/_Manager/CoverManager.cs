using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverManager : ManagedPool<CoverPrefab>
{
    protected override void Awake() {
        base.Awake();
        pool.prefab = Resources.Load<GameObject>("Prefabs/_Manager/CoverPrefab");
    }
}
