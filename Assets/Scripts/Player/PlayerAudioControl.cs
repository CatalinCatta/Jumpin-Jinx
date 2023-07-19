using UnityEngine;

public class PlayerAudioControl : MonoBehaviour
{
    [SerializeField] private AudioClip walk;
    [SerializeField] private AudioClip jump;
    [SerializeField] private AudioClip getHit;
    [SerializeField] private AudioClip die;
    [SerializeField] private AudioClip shootArrow;

    private AudioSource _audioSource;

    private void Start() =>
        _audioSource = transform.GetComponent<AudioSource>();

    public void PlayWalkSound() 
    {
        _audioSource.volume =
            SettingsManager.Instance.soundEffectVolume * SettingsManager.Instance.generalVolume;
        _audioSource.clip = walk;
        _audioSource.Play();
    }
    
    public void PlayJumpSound() 
    {
        _audioSource.volume =
            SettingsManager.Instance.soundEffectVolume * SettingsManager.Instance.generalVolume;
        _audioSource.clip = jump;
        _audioSource.Play();
    }
    
    public void PlayGetHitSound() 
    {
        _audioSource.volume =
            SettingsManager.Instance.soundEffectVolume * SettingsManager.Instance.generalVolume;
        _audioSource.clip = getHit;
        _audioSource.Play();
    }
    
    public void PlayDieSound() 
    {
        _audioSource.volume =
            SettingsManager.Instance.soundEffectVolume * SettingsManager.Instance.generalVolume;
        _audioSource.clip = die;
        _audioSource.Play();
    }
    
    public void PlayShootArrowSound() 
    {
        _audioSource.volume =
            SettingsManager.Instance.soundEffectVolume * SettingsManager.Instance.generalVolume;
        _audioSource.clip = shootArrow;
        _audioSource.Play();
    }
}
