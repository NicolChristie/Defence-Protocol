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
        Debug.Log("List of potential merges on start:");

        if (mergeCombinations != null)
        {
            foreach (var entry in mergeCombinations)
            {
                Debug.Log($"Merge Pair: {entry.weaponA.name} + {entry.weaponB.name} -> Result: {entry.result.name}");
            }
        }
        else
        {
            Debug.LogWarning("Merge combinations list is null!");
        }
    }

   public Weaponprefab GetMergeResult(Weaponprefab weapon1, Weaponprefab weapon2)
   {
        foreach (var entry in mergeCombinations)
        {
            // Compare the actual Weaponprefab references, not just the names
            if ((entry.weaponA == weapon1 && entry.weaponB == weapon2) ||
                (entry.weaponA == weapon2 && entry.weaponB == weapon1))
            {
                Debug.Log($"Found merge: {weapon1.name} + {weapon2.name} -> {entry.result.name}");
                return entry.result;
            }
        }

        return null;
   }
}
