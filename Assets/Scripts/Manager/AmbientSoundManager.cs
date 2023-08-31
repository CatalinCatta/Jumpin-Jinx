using UnityEngine;

public class AmbientSoundManager : MonoBehaviour
{
    private void Start() =>
        SettingsManager.SetUpSound(transform);
}