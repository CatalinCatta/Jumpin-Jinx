using UnityEngine;

public class PlayerAudioControl : MonoBehaviour
{
    [SerializeField] private AudioClip walk;
    [SerializeField] private AudioClip jump;
    [SerializeField] private AudioClip getHit;
    [SerializeField] private AudioClip die;
    [SerializeField] private AudioClip shootArrow;

    private AudioSource _audioSourceWalk;
    private AudioSource _audioSourceJump;
    private AudioSource _audioSourceGetHit;
    private AudioSource _audioSourceDie;
    private AudioSource _audioSourceShootArrow;
    
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSourceWalk = gameObject.AddComponent<AudioSource>();
        _audioSourceWalk.clip = walk;
        
        _audioSourceJump = gameObject.AddComponent<AudioSource>();
        _audioSourceJump.clip = jump;
        
        _audioSourceGetHit = gameObject.AddComponent<AudioSource>();
        _audioSourceGetHit.clip = getHit;
        
        _audioSourceDie = gameObject.AddComponent<AudioSource>();
        _audioSourceDie.clip = die;
        
        _audioSourceShootArrow = gameObject.AddComponent<AudioSource>();
        _audioSourceShootArrow.clip = shootArrow;
    }

    public void PlayWalkSound() 
    {
        _audioSourceWalk.volume =
            SettingsManager.Instance.soundEffectVolume * SettingsManager.Instance.generalVolume;
        _audioSourceWalk.Play();
    }
    
    public void PlayJumpSound() 
    {
        _audioSourceJump.volume =
            SettingsManager.Instance.soundEffectVolume * SettingsManager.Instance.generalVolume;
        _audioSourceJump.Play();
    }
    
    public void PlayGetHitSound() 
    {
        _audioSourceGetHit.volume =
            SettingsManager.Instance.soundEffectVolume * SettingsManager.Instance.generalVolume;
        _audioSourceGetHit.Play();
    }
    
    public void PlayDieSound() 
    {
        _audioSourceDie.volume =
            SettingsManager.Instance.soundEffectVolume * SettingsManager.Instance.generalVolume;
        _audioSourceDie.Play();
    }
    
    public void PlayShootArrowSound() 
    {
        _audioSourceShootArrow.volume =
            SettingsManager.Instance.soundEffectVolume * SettingsManager.Instance.generalVolume;
        _audioSourceShootArrow.Play();
    }
}
