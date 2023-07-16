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

    public void PlayWalkSoud() 
    {
        _audioSource.clip = walk;
        _audioSource.Play();
    }
    
    public void PlayJumpSoud() 
    {
        _audioSource.clip = jump;
        _audioSource.Play();
    }
    
    public void PlayGetHitSoud() 
    {
        _audioSource.clip = getHit;
        _audioSource.Play();
    }
    
    public void PlayDieSoud() 
    {
        _audioSource.clip = die;
        _audioSource.Play();
    }
    
    public void PlayShootArrowSoud() 
    {
        _audioSource.clip = shootArrow;
        _audioSource.Play();
    }
}
