using System;
using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// This class manages the status of the player, including health, buffs, coins, and display.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(PlayerControl), typeof(PlayerAudioControl))]
public class PlayerStatus : MonoBehaviour
{
    [Header("Components")]
    public GameObject display;
    private PlayerControl _playerControl;
    private Rigidbody2D _rigidbody;
    private PlayerAudioControl _playerAudioControl;
    private PlayerManager _playerManager;

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

    /// <summary>
    /// Array of special Powers status (multiplier, duration, order in game editor).
    /// Each entrance correspond with <see cref="SpecialPower"/>.
    /// </summary>
    private readonly (float, float, int)[] _specialPowersStatus =
    {
        (2f, 3f, 2),
        (1.5f, 5f, 3)
    };
    
    private enum SpecialPower
    {
        SpeedBuff,
        JumpBuff
    }

    private void Awake()
    {
        if (display == null)
            display = GameObject.FindWithTag("Status");

        _rigidbody = GetComponent<Rigidbody2D>();
        _playerControl = GetComponent<PlayerControl>();
        _playerAudioControl = GetComponent<PlayerAudioControl>();

        Time.timeScale = 1f;
    }

    private void Start()
    {
        _playerManager = PlayerManager.Instance;
        _endlessRun = LvlManager.Instance.CurrentLvl == 0;
        _hp = _endlessRun ? (_playerManager.HpLvl + 1) * 5 : 20;
        _speedBuffs = _playerManager.SpeedBuffs;
        _jumpBuffs = _playerManager.JumpBuffs;

        ShowLife();

        if (!_endlessRun)
            return;

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
        StartCoroutine(PowerUpTimer(SpecialPower.SpeedBuff));
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
        StartCoroutine(PowerUpTimer(SpecialPower.JumpBuff));
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
    public void GetDamage(Vector3 direction, int amount)
    {
        _hp -= _endlessRun ? amount * (1 - _playerManager.DefLvl / 100) : amount;
        ShowLife();

        FreezeFromDamage = true;
        _rigidbody.velocity = new Vector2(direction.x * 3, (direction.y - 1.4f) * -5);  // *** TO DO *** : improve knock back animation
        _playerAudioControl.PlayGetHitSound();

        if (_hp == 0)
            Die();
    }

    /// <summary>
    /// Coroutine that activates the power up timer for a certain duration.
    /// </summary>
    private IEnumerator PowerUpTimer(SpecialPower specialPower)
    {
        SwitchPowerActive(true);
        _playerControl.JumpPower *= _specialPowersStatus[(int)specialPower].Item1;

        var timer = display.transform.GetChild(_specialPowersStatus[(int)specialPower].Item3).GetChild(0).GetChild(0).gameObject;
        var rectTransform = timer.GetComponent<RectTransform>();
        var timerParentHeight = timer.transform.parent.GetComponent<RectTransform>().rect.height;
        var elapsedTime = 0f;
        var initialPosition = rectTransform.localPosition;

        timer.SetActive(true);

        while (elapsedTime < _specialPowersStatus[(int)specialPower].Item2)
        {
            elapsedTime += Time.deltaTime;
            rectTransform.localPosition = Vector3.Lerp(initialPosition,
                initialPosition + new Vector3(0f, timerParentHeight, 0f),
                Mathf.Clamp01(elapsedTime / _specialPowersStatus[(int)specialPower].Item2));
            yield return null;
        }

        rectTransform.localPosition = initialPosition;
        timer.SetActive(false);

        SwitchPowerActive(false);
        _playerControl.JumpPower /= _specialPowersStatus[(int)specialPower].Item1;

        void SwitchPowerActive(bool active)
        {
            switch (specialPower)
            {
                case SpecialPower.SpeedBuff:
                    SpeedBuffActive = active;
                    break;
                case SpecialPower.JumpBuff:
                    JumpBuffActive = active;
                    break;
            }
        }
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
            endScreen.GetChild(1).gameObject.SetActive(true);
        else
        {
            endScreen.gameObject.SetActive(true);

            ChangeText(1, _coins.ToString());
            ChangeText(2, KillCounter.ToString());
            ChangeText(3, Utility.FormatDoubleWithUnits(transform.position.x / 2.55, true) + "m");

            _playerManager.JumpBuffs = _jumpBuffs;
            _playerManager.SpeedBuffs = _speedBuffs;
            _playerManager.Gold += _coins;

            _playerManager.Save();

            void ChangeText(int childId, string text) =>
                endScreen.GetChild(0).GetChild(childId).GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
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
    private void ShowLife() => ChangeUiText(GetUiTransform(0).GetChild(0), _hp.ToString());

    /// <summary>
    /// Method to update the displayed coin count.
    /// </summary>
    private void ShowCoins() => ChangeUiText(GetUiTransform(1), _coins.ToString());

    /// <summary>
    /// Method to update the displayed speed buffs count.
    /// </summary>
    private void ShowSpeedBuffs() => ChangeUiText(GetUiTransform(2), _speedBuffs.ToString());

    /// <summary>
    /// Method to update the displayed jump buffs count.
    /// </summary>
    private void ShowJumpBuffs() => ChangeUiText(GetUiTransform(3), _jumpBuffs.ToString());

    private static void ChangeUiText(Component element, string text) => element.GetComponent<TextMeshProUGUI>().text = text;
    private Transform GetUiTransform(int childId) => display.transform.GetChild(childId).GetChild(1);
}