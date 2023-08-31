using UnityEngine;

/// <summary>
/// Controls audio playback for player actions and events.
/// </summary>
public class PlayerAudioControl : MonoBehaviour
{
    [Header("Player SFX")] [SerializeField]
    private AudioClip 
        walk, 
        jump, 
        getHit, 
        die, 
        shootArrow;

    [Header("Audio Sources Components")] private AudioSource 
        _audioSourceDie,
        _audioSourceGetHit,
        _audioSourceJump,
        _audioSourceShootArrow,
        _audioSourceWalk;

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

    /// <summary>
    /// Plays the walk sound effect.
    /// </summary>
    public void PlayWalkSound()
    {
        _audioSourceWalk.volume =
            SettingsManager.Instance.SoundEffectVolume * SettingsManager.Instance.GeneralVolume;
        _audioSourceWalk.Play();
    }

    /// <summary>
    /// Plays the jump sound effect.
    /// </summary>
    public void PlayJumpSound()
    {
        _audioSourceJump.volume =
            SettingsManager.Instance.SoundEffectVolume * SettingsManager.Instance.GeneralVolume;
        _audioSourceJump.Play();
    }

    /// <summary>
    /// Plays the hit sound effect.
    /// </summary>
    public void PlayGetHitSound()
    {
        _audioSourceGetHit.volume =
            SettingsManager.Instance.SoundEffectVolume * SettingsManager.Instance.GeneralVolume;
        _audioSourceGetHit.Play();
    }

    /// <summary>
    /// Plays the die sound effect.
    /// </summary>
    public void PlayDieSound()
    {
        _audioSourceDie.volume =
            SettingsManager.Instance.SoundEffectVolume * SettingsManager.Instance.GeneralVolume;
        _audioSourceDie.Play();
    }

    /// <summary>
    /// Plays the shoot arrow sound effect.
    /// </summary>
    public void PlayShootArrowSound()
    {
        _audioSourceShootArrow.volume =
            SettingsManager.Instance.SoundEffectVolume * SettingsManager.Instance.GeneralVolume;
        _audioSourceShootArrow.Play();
    }
}