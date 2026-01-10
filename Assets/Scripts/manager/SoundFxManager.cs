using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SoundEffect
{
    public string name;            // Identifier you can call from code
    public AudioClip[] clips;      // Variations for that sound
}

public class SoundFxManager : MonoBehaviour
{
    public static SoundFxManager Instance;

    [SerializeField] private AudioSource soundFxObject;

    [Header("Sound Effects Library")]
    public List<SoundEffect> soundEffects = new List<SoundEffect>();

    private Dictionary<string, AudioClip[]> soundLookup;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        // Build lookup dictionary
        soundLookup = new Dictionary<string, AudioClip[]>();
        foreach (var sfx in soundEffects)
        {
            if (!soundLookup.ContainsKey(sfx.name))
                soundLookup.Add(sfx.name, sfx.clips);
        }
    }

    public void PlaySound(string soundName, Transform spawnTransform, float volume = 1f)
    {
        if (!soundLookup.TryGetValue(soundName, out AudioClip[] clips) || clips.Length == 0)
        {
            Debug.LogWarning($"Sound '{soundName}' not found or has no clips in SoundFxManager!");
            return;
        }

        // Pick random variation
        AudioClip clip = clips[Random.Range(0, clips.Length)];

        AudioSource audioSource = Instantiate(soundFxObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(audioSource.gameObject, clip.length);
    }
}
