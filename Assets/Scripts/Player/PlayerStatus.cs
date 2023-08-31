using System;
using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// This class manages the status of the player, including health, buffs, coins, and display.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(PlayerControl), typeof(PlayerControl))]
public class PlayerStatus : MonoBehaviour
{
    [Header("Components")]
    public GameObject display;
    private PlayerAudioControl _playerAudioControl;
    private PlayerControl _playerControl;

    [Header("Movement Control Bool")] [NonSerialized]
    public bool
        FreezeFromDamage,
        SpeedBuffActive,
        JumpBuffActive;

    [Header("Consumables")] [NonSerialized] public int KillCounter;
    private int
        _coins,
        _hp,
        _jumpBuffs,
        _speedBuffs;
    
    [Header("Utilities")]
    private bool _endlessRun;
    private IEnumerator _knockBack;

    private void Awake()
    {
        // Find the display object if it is not assigned
        if (display == null)
            display = GameObject.FindWithTag("Status");

        // Get the required components
        _playerControl = GetComponent<PlayerControl>();
        _playerAudioControl = GetComponent<PlayerAudioControl>();
        
        // Set the time scale to 1 to be sure that the game is playing
        Time.timeScale = 1f;
    }

    private void Start()
    {
        // Check if it is an endless run level
        _endlessRun = LvlManager.Instance.CurrentLvl == 0;

        // Set the initial health based on the level and player's health level
        _hp = _endlessRun ? (PlayerManager.Instance.HpLvl + 1) * 5 : 20;
        
        // Set the initial speed and jump buffs based on the player's buffs
        _speedBuffs = PlayerManager.Instance.SpeedBuffs;
        _jumpBuffs = PlayerManager.Instance.JumpBuffs;

        // Show the initial health
        ShowLife();

        if (!_endlessRun)
            return;

        // Show the initial jump and speed buffs
        ShowJumpBuffs();
        ShowSpeedBuffs();
    }

    /// <summary>
    /// Add a coin to the player's inventory.
    /// </summary>
    public void AddCoin()
    {
        _coins++;
        ShowCoins();
    }

    /// <summary>
    /// Consume a speed buff and activate the speed buff effect.
    /// </summary>
    public void ConsumeSpeedBuff()
    {
        if (_speedBuffs == 0 || !_endlessRun)
            return;

        _speedBuffs--;
        ShowSpeedBuffs();
        StartCoroutine(SpeedTimer());
    }

    /// <summary>
    /// Consume a jump buff and activate the jump buff effect.
    /// </summary>
    public void ConsumeJumpBuff()
    {
        if (_jumpBuffs == 0 || !_endlessRun)
            return;

        _jumpBuffs--;
        ShowJumpBuffs();
        StartCoroutine(JumpTimer());
    }

    /// <summary>
    /// Increase player Health.
    /// </summary>
    public void Heal()
    {
        _hp += 5;
        ShowLife();
    }

    /// <summary>
    /// Apply damage to the player and handle knock-back effect.
    /// </summary>
    /// <param name="direction">The direction of the knock-back.</param>
    /// <param name="amount">The amount of damage to apply.</param>
    public void GetDamage(Vector3 direction, int amount)    // *** TO DO *** : improve knock back animation
    {
        _hp -= _endlessRun ? amount * (1 - PlayerManager.Instance.DefLvl / 100) : amount;
        ShowLife();

        //if (_knockBack != null)
        //    StopCoroutine(_knockBack);

        FreezeFromDamage = true;

        GetComponent<Rigidbody2D>().velocity =
            new Vector2(direction.x * 3, (direction.y - 1.4f) * -5);

        //_knockBack = MoveToDirection();
        //StartCoroutine(_knockBack);

        _playerAudioControl.PlayGetHitSound();

        if (_hp == 0)
            Die();
    }

    /// <summary>
    /// Coroutine that activates a jump buff for a certain duration.
    /// </summary>
    private IEnumerator JumpTimer()
    {
        JumpBuffActive = true;
        _playerControl.JumpPower *= 1.5f;

        var timer = display.transform.GetChild(3).GetChild(0).GetChild(0).gameObject;
        var rectTransform = timer.GetComponent<RectTransform>();

        var elapsedTime = 0f;
        var initialPosition = rectTransform.localPosition;

        timer.SetActive(true);

        while (elapsedTime < 5f)
        {
            elapsedTime += Time.deltaTime;
            rectTransform.localPosition = Vector3.Lerp(initialPosition,
                initialPosition + new Vector3(0f, timer.transform.parent.GetComponent<RectTransform>().rect.height, 0f),
                Mathf.Clamp01(elapsedTime / 5f));
            yield return null;
        }

        rectTransform.localPosition = initialPosition;
        timer.SetActive(false);

        JumpBuffActive = false;
        _playerControl.JumpPower /= 1.5f;
    }

    /// <summary>
    /// Coroutine that activates a speed buff for a certain duration.
    /// </summary>
    private IEnumerator SpeedTimer()
    {
        SpeedBuffActive = true;
        _playerControl.MovementSpeed *= 2;

        var timer = display.transform.GetChild(2).GetChild(0).GetChild(0).gameObject;
        var rectTransform = timer.GetComponent<RectTransform>();

        var elapsedTime = 0f;
        var initialPosition = rectTransform.localPosition;

        timer.SetActive(true);

        while (elapsedTime < 3f)
        {
            elapsedTime += Time.deltaTime;
            rectTransform.localPosition = Vector3.Lerp(initialPosition,
                initialPosition + new Vector3(0f, timer.transform.parent.GetComponent<RectTransform>().rect.height, 0f),
                Mathf.Clamp01(elapsedTime / 5f));
            yield return null;
        }

        rectTransform.localPosition = initialPosition;
        timer.SetActive(false);

        SpeedBuffActive = false;
        _playerControl.MovementSpeed /= 2;
    }

    /// <summary>
    /// Coroutine that changes the player's sprite color to red for a short duration.
    /// </summary>
    private IEnumerator MoveToDirection()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(.3f);

        spriteRenderer.color = Color.white;

        FreezeFromDamage = false;
    }

    /// <summary>
    /// Method called when the player dies.
    /// </summary>
    public void Die()
    {
        _playerAudioControl.PlayDieSound();
        gameObject.SetActive(false);

        Time.timeScale = 0f;
        display.SetActive(false);

        var endScreen = display.transform.parent.parent.GetChild(1);

        if (!_endlessRun)
        {
            endScreen.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            endScreen.gameObject.SetActive(true);

            var insideEndScreenFrame = endScreen.GetChild(0);

            insideEndScreenFrame.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = _coins.ToString();
            insideEndScreenFrame.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = KillCounter.ToString();
            insideEndScreenFrame.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>().text =
                Utils.DoubleToString(transform.position.x / 2.55, true) + "m";

            PlayerManager.Instance.JumpBuffs = _jumpBuffs;
            PlayerManager.Instance.SpeedBuffs = _speedBuffs;
            PlayerManager.Instance.Gold += _coins;

            PlayerManager.Instance.Save();
        }
    }

    /// <summary>
    /// Method called when the player wins.
    /// </summary>
    public void Win()
    {
        Time.timeScale = 0f;
        display.SetActive(false);

        var winScreen = display.transform.parent.parent.GetChild(1).GetChild(0);
        winScreen.gameObject.SetActive(true);

        for (var i = 0; i < _coins; i++)
        {
            var endCoin = winScreen.GetChild(0).GetChild(i + 1).GetChild(0);
            endCoin.gameObject.SetActive(true);
            endCoin.GetComponent<AudioSource>().volume =
                SettingsManager.Instance.SoundEffectVolume * SettingsManager.Instance.GeneralVolume;
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Method to update the displayed life count.
    /// </summary>
    private void ShowLife() =>
        display.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = _hp.ToString();

    /// <summary>
    /// Method to update the displayed coin count.
    /// </summary>
    private void ShowCoins() =>
        display.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = _coins.ToString();

    /// <summary>
    /// Method to update the displayed speed buffs count.
    /// </summary>
    private void ShowSpeedBuffs() =>
        display.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = _speedBuffs.ToString();

    /// <summary>
    /// Method to update the displayed jump buffs count.
    /// </summary>
    private void ShowJumpBuffs() =>
        display.transform.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>().text = _jumpBuffs.ToString();
}