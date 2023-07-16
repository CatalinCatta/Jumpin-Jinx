using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
    
public class PlayerStatus : MonoBehaviour
{
    [SerializeField] private GameObject display;
    [SerializeField] private bool endlessRun;
    
    public int KillCounter;
    
    private int _coins;
    private int _hp;
    private int _speedBuffs;
    private int _jumpBuffs;

    public bool FreezedFromDamage;
    public bool SpeedBuffActive;
    public bool JumpBuffActive;

    private PlayerControl _playerControl;

    private void Awake()
    {
        _playerControl = transform.GetComponent<PlayerControl>();
        Time.timeScale = 1f;
    }
    
    private void Start()
    {
        _hp = endlessRun ? 50 : 20;
        _speedBuffs = endlessRun ? 5 : 2;
        _jumpBuffs = endlessRun ? 5 : 2;
        
        ShowLife();
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
        if (_speedBuffs == 0)
            return;
        
        _speedBuffs--;
        ShowSpeedBuffs();
        StartCoroutine(SpeedTimer());
    }

    public void ConsumeJumpBuff()
    {
        if (_jumpBuffs == 0)
            return;

        _jumpBuffs--;
        ShowJumpBuffs();
        StartCoroutine(JumpTimer());
    }

    public void Heal()
    {
        _hp += 10;
        ShowLife();
    }

    public void GetDamage(Vector3 direction, int amount)
    {
        _hp -= amount;
        ShowLife();
        StartCoroutine(MoveToDirection(direction.x));
        
        if (_hp == 0)
            Die();
    }

    private void ShowLife() =>
        display.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = _hp.ToString();
    
    private void ShowCoins() =>
        display.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = _coins.ToString();

    private void ShowSpeedBuffs() =>
        display.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = _speedBuffs.ToString();
    
    private void ShowJumpBuffs() =>
        display.transform.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>().text = _jumpBuffs.ToString();

    private IEnumerator JumpTimer()
    {
        JumpBuffActive = true;
        _playerControl.jumpPower *= 2;
        
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
        
        JumpBuffActive = false;
        _playerControl.jumpPower /= 2;
    }
    
    private IEnumerator SpeedTimer()
    {
        SpeedBuffActive = true;
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
       
        SpeedBuffActive = false;
        _playerControl.movementSpeed /= 2;
    }
    
    private IEnumerator MoveToDirection(float direction)
    {
        FreezedFromDamage = true;
     
        var oldPosition = transform.position; 
        var targetPosition = new Vector3(oldPosition.x * 2.5f - direction * 1.5f, oldPosition.y + 1, -5);
        var elapsedTime = 0f;

        transform.GetComponent<SpriteRenderer>().color = Color.red;
        
        while (elapsedTime < 1f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, elapsedTime);
            elapsedTime += Time.deltaTime * 5;
            yield return null;
        }
        
        transform.GetComponent<SpriteRenderer>().color = Color.white;
        transform.position = targetPosition;

        FreezedFromDamage = false;
    }
    
    public void Die()
    {
        Time.timeScale = 0f;
        display.SetActive(false);

        var endScreen = display.transform.parent.parent.GetChild(1);
        
        if (!endlessRun)
            endScreen.GetChild(1).gameObject.SetActive(true);
        else
        {
            endScreen.gameObject.SetActive(true);
            
            var insideEndScreenFrame = endScreen.GetChild(0);

            insideEndScreenFrame.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = _coins.ToString();
            insideEndScreenFrame.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = KillCounter.ToString();
            insideEndScreenFrame.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>().text = Utils.DoubleToString(transform.position.x / 2.55) + "m";
        }
        
        Destroy(gameObject);
    }
    
    public void Win()
    {
        Time.timeScale = 0f;
        display.SetActive(false);
        
        var winScreen =  display.transform.parent.parent.GetChild(1).GetChild(0);
        winScreen.gameObject.SetActive(true);
        
        for (var i = 0; i < _coins; i++)
            winScreen.GetChild(0).GetChild(i + 1).GetComponent<Image>().color = Color.white;

        Destroy(gameObject);
    }
}
