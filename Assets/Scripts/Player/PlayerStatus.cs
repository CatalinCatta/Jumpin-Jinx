using UnityEngine;
using TMPro;
using System.Collections;
    
public class PlayerStatus : MonoBehaviour
{
    public GameObject display;
    private bool _endlessRun;
    
    public int killCounter;
    
    private int _coins;
    private int _hp;
    private int _speedBuffs;
    private int _jumpBuffs;

    public bool freezeFromDamage;
    public bool speedBuffActive;
    public bool jumpBuffActive;

    private PlayerControl _playerControl;
    private PlayerAudioControl _playerAudioControl;

    private IEnumerator _knockBack;

    private void Awake()
    {
        if (display == null)
            display = GameObject.FindWithTag("Status");
    
        _playerControl = transform.GetComponent<PlayerControl>();
        _playerAudioControl = transform.GetComponent<PlayerAudioControl>();
        Time.timeScale = 1f;
    }
    
    private void Start()
    {
        _endlessRun = LvlManager.Instance.currentLvl == 0;
        
        _hp = _endlessRun ? (PlayerManager.Instance.hpLvl + 1) * 5 : 20;
        _speedBuffs = PlayerManager.Instance.speedBuffs;
        _jumpBuffs = PlayerManager.Instance.jumpBuffs;
        
        ShowLife();
        if (!_endlessRun)
            return;
        
        ShowJumpBuffs();
        ShowSpeedBuffs();
    }

    public void AddCoin()
    {
        _coins++;
        ShowCoins();
    }

    public void ConsumeSpeedBuff()
    {
        if (_speedBuffs == 0 || !_endlessRun)
            return;
        
        _speedBuffs--;
        ShowSpeedBuffs();
        StartCoroutine(SpeedTimer());
    }

    public void ConsumeJumpBuff()
    {
        if (_jumpBuffs == 0 || !_endlessRun)
            return;

        _jumpBuffs--;
        ShowJumpBuffs();
        StartCoroutine(JumpTimer());
    }

    public void Heal()
    {
        _hp += 5;
        ShowLife();
    }

    public void GetDamage(Vector3 direction, int amount)
    {
        _hp -= _endlessRun? amount * (1 - PlayerManager.Instance.defLvl / 100) : amount;
        ShowLife();

        if (_knockBack != null)
            StopCoroutine(_knockBack);
        
        freezeFromDamage = true;
        
        GetComponent<Rigidbody2D>().velocity = 
            new Vector2(direction.x * 3, (direction.y - 1.4f) * -5);

        _knockBack = MoveToDirection();
        StartCoroutine(_knockBack);
        
        _playerAudioControl.PlayGetHitSound();
        
        if (_hp == 0)
            Die();
    }
    
    private IEnumerator JumpTimer()
    {
        jumpBuffActive = true;
        _playerControl.jumpPower *= 1.5f;
        
        var timer = display.transform.GetChild(3).GetChild(0).GetChild(0).gameObject;
        var rectTransform = timer.GetComponent<RectTransform>();

        var elapsedTime = 0f;
        var initialPosition = rectTransform.localPosition;

        timer.SetActive(true);

        while (elapsedTime < 5f)
        {
            elapsedTime += Time.deltaTime;
            rectTransform.localPosition = Vector3.Lerp(initialPosition, initialPosition + new Vector3(0f, timer.transform.parent.GetComponent<RectTransform>().rect.height, 0f), Mathf.Clamp01(elapsedTime / 5f));
            yield return null;
        }

        rectTransform.localPosition = initialPosition;
        timer.SetActive(false);
        
        jumpBuffActive = false;
        _playerControl.jumpPower /= 1.5f;
    }
    
    private IEnumerator SpeedTimer()
    {
        speedBuffActive = true;
        _playerControl.movementSpeed *= 2;
        
        var timer = display.transform.GetChild(2).GetChild(0).GetChild(0).gameObject;
        var rectTransform = timer.GetComponent<RectTransform>();

        var elapsedTime = 0f;
        var initialPosition = rectTransform.localPosition;

        timer.SetActive(true);

        while (elapsedTime < 3f)
        {
            elapsedTime += Time.deltaTime;
            rectTransform.localPosition = Vector3.Lerp(initialPosition, initialPosition + new Vector3(0f, timer.transform.parent.GetComponent<RectTransform>().rect.height, 0f), Mathf.Clamp01(elapsedTime / 5f));
            yield return null;
        }

        rectTransform.localPosition = initialPosition;
        timer.SetActive(false);
       
        speedBuffActive = false;
        _playerControl.movementSpeed /= 2;
    }

    private IEnumerator MoveToDirection()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
            
        spriteRenderer.color = Color.red;
        
        yield return new WaitForSeconds(0.3f);
        
        spriteRenderer.color = Color.white;

        freezeFromDamage = false;
    }
    
    public void Die()
    {
        _playerAudioControl.PlayDieSound();
        gameObject.SetActive(false);
        
        Time.timeScale = 0f;
        display.SetActive(false);

        var endScreen = display.transform.parent.parent.GetChild(1);
        
        if (!_endlessRun)
            endScreen.GetChild(1).gameObject.SetActive(true);
        else
        {
            endScreen.gameObject.SetActive(true);
            
            var insideEndScreenFrame = endScreen.GetChild(0);

            insideEndScreenFrame.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = _coins.ToString();
            insideEndScreenFrame.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = killCounter.ToString();
            insideEndScreenFrame.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>().text = Utils.DoubleToString(transform.position.x / 2.55) + "m";

            PlayerManager.Instance.jumpBuffs = _jumpBuffs;
            PlayerManager.Instance.speedBuffs = _speedBuffs;
        }
        
    }
    
    public void Win()
    {
        Time.timeScale = 0f;
        display.SetActive(false);
        
        var winScreen =  display.transform.parent.parent.GetChild(1).GetChild(0);
        winScreen.gameObject.SetActive(true);
        
        for (var i = 0; i < _coins; i++)
        {
            var endCoin = winScreen.GetChild(0).GetChild(i + 1).GetChild(0);
            endCoin.gameObject.SetActive(true);
            endCoin.GetComponent<AudioSource>().volume = 
                SettingsManager.Instance.soundEffectVolume * SettingsManager.Instance.generalVolume;    
        }

        Destroy(gameObject);
    }

    private void ShowLife() =>
        display.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = _hp.ToString();
    
    private void ShowCoins() =>
        display.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = _coins.ToString();

    private void ShowSpeedBuffs() =>
        display.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = _speedBuffs.ToString();
    
    private void ShowJumpBuffs() =>
        display.transform.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>().text = _jumpBuffs.ToString();

}
