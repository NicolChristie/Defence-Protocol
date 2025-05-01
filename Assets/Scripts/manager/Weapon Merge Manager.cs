using UnityEngine;
using System.Collections.Generic;

public class WeaponMergeManager : MonoBehaviour
{
    public static WeaponMergeManager Instance { get; private set; }

    [System.Serializable]
    public class WeaponMergeEntry
    {
        public Weaponprefab weaponA;
        public Weaponprefab weaponB;  
        public Weaponprefab result;  
    }

    public List<WeaponMergeEntry> mergeCombinations;

    private void Awake()
    {
            Instance = this;
    }

   public Weaponprefab GetMergeResult(Weaponprefab weapon1, Weaponprefab weapon2)
{
    Debug.Log($"🔄 Merging {weapon1.originalPrefab.name} + {weapon2.originalPrefab.name}");
    foreach (var entry in mergeCombinations)
    {
        if ((entry.weaponA.originalPrefab == weapon1.originalPrefab &&
                entry.weaponB.originalPrefab == weapon2.originalPrefab) ||
            (entry.weaponA.originalPrefab == weapon2.originalPrefab &&
                entry.weaponB.originalPrefab == weapon1.originalPrefab))
        {
            return entry.result;
        }
        Debug.Log($"checking {entry.weaponA.originalPrefab} + {entry.weaponB.originalPrefab} = {entry.result.originalPrefab}");
    }

    Debug.Log($"❌ No merge found for: {weapon1.originalPrefab.name} + {weapon2.originalPrefab.name}");
    return null;
}

}
