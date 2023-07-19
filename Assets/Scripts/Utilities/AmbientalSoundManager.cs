using UnityEngine;

public class AmbientalSoundManager : MonoBehaviour
{
    private void Start() =>
        SettingsManager.SetUpSound(transform);
}
