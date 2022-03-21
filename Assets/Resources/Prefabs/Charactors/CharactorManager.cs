using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharactorManager : ManagedPool<CharactorPrefab>
{
    public CharactorPrefab First()
    {
        return dictionary.FirstOrDefault(e => e.Value != null).Value;
    }
}
