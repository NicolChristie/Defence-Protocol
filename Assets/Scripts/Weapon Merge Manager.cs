using UnityEngine;
using System.Collections.Generic;

public class WeaponMergeManager : MonoBehaviour
{
    public static WeaponMergeManager Instance { get; private set; }

    [System.Serializable]
    public class WeaponMergeEntry
    {
        public Weaponprefab weaponA;  // Now referencing Weaponprefab, not GameObject
        public Weaponprefab weaponB;  // Same here
        public Weaponprefab result;   // The result after merge, also Weaponprefab
    }

    public List<WeaponMergeEntry> mergeCombinations;

    private void Awake()
    {
            Instance = this;
        // Debug to list all potential merges on startup
    }

   public Weaponprefab GetMergeResult(Weaponprefab weapon1, Weaponprefab weapon2)
{
    foreach (var entry in mergeCombinations)
    {
        if ((entry.weaponA.originalPrefab == weapon1.originalPrefab &&
             entry.weaponB.originalPrefab == weapon2.originalPrefab) ||
            (entry.weaponA.originalPrefab == weapon2.originalPrefab &&
             entry.weaponB.originalPrefab == weapon1.originalPrefab))
        {
            return entry.result;
        }
    }

    Debug.Log($"❌ No merge found for: {weapon1.originalPrefab.name} + {weapon2.originalPrefab.name}");
    return null;
}

}
