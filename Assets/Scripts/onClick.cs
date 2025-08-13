using UnityEngine;
using UnityEngine.UI;

public class onClick : MonoBehaviour
{
    public void playSound()
    {
        SoundFxManager.Instance.PlaySound("Click", transform, 1f);
    }
}
