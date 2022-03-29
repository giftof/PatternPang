using UnityEngine;
using System.Linq;

public class CharactorManager : ManagedPool<CharactorPrefab>
{
    protected override void Awake() 
    {
        base.Awake();
        pool.prefab = Resources.Load<GameObject>("Prefabs/_Charactor/CharactorPrefab");
    }

    public CharactorPrefab Target()
        => dictionary.FirstOrDefault(e => e.Value != null).Value;
}
