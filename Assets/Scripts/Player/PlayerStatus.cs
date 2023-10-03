using System;
using System.Collections;
using System.Collections.Generic;
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
    private LvlManager _lvlManager;
    private float _timer;

    private TextMeshProUGUI _timerDisplay;

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
    private readonly Dictionary<BuffType, int> _buffPosition = new()
        { { BuffType.JumpBuff, 2 }, { BuffType.SpeedBuff, 3 } };
    
    private void Awake()
    {
        if (display == null) display = GameObject.FindWithTag("Status");

        _timerDisplay = GameObject.Find("TimerCounter").transform.GetComponent<TextMeshProUGUI>();
        
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerControl = GetComponent<PlayerControl>();
        _playerAudioControl = GetComponent<PlayerAudioControl>();

        Time.timeScale = 1f;
    }

    private void Start()
    {
        _lvlManager = LvlManager.Instance;
        _playerManager = PlayerManager.Instance;
        _endlessRun = _lvlManager.CurrentScene == Scene.Endless;
        _hp = _endlessRun ? (_playerManager.Upgrades[(int)UpgradeType.MaxHealth].Quantity + 1) * 5 : 20;
        _speedBuffs = _playerManager.Buffs[(int)BuffType.SpeedBuff].Quantity;
        _jumpBuffs = _playerManager.Buffs[(int)BuffType.JumpBuff].Quantity;

        ShowLife();

        if (_endlessRun)
        {
            ShowJumpBuffs();
            ShowSpeedBuffs();
        }
        else StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer()
    {
        while (true)
        {
            _timer += Time.deltaTime;
            _timerDisplay.text = Utility.TimeToString(_timer);
            yield return null;
        }
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
    /// Consume a minAndMaxSpeed buff and activate the minAndMaxSpeed buff effect.
    /// </summary>
    public void ConsumeSpeedBuff()
    {
        if (_speedBuffs == 0 || !_endlessRun) return;

        _speedBuffs--;
        ShowSpeedBuffs();
        StartCoroutine(PowerUpTimer(BuffType.SpeedBuff));
    }

    /// <summary>
    /// Consume a jump buff and activate the jump buff effect.
    /// </summary>
    public void ConsumeJumpBuff()
    {
        if (_jumpBuffs == 0 || !_endlessRun) return;

        _jumpBuffs--;
        ShowJumpBuffs();
        StartCoroutine(PowerUpTimer(BuffType.JumpBuff));
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
        _hp -= _endlessRun ? amount * (1 - _playerManager.Upgrades[(int)UpgradeType.Defence].Quantity / 100) : amount;
        ShowLife();

        //FreezeFromDamage = true;
        //_rigidbody.velocity = new Vector2(direction.x * 3, (direction.y - 1.4f) * -5);  // TODO: Improve knock back animation.
        _playerAudioControl.PlayGetHitSound();

        if (_hp == 0) Die();
    }

    /// <summary>
    /// Coroutine that activates the power up timer for a certain duration.
    /// </summary>
    private IEnumerator PowerUpTimer(BuffType buffType)
    {
        var buffDetail = Dictionaries.BuffDetails[buffType];
        if (!buffDetail.activatable) yield break;

        SwitchPowerStatus(true);

        var timer = display.transform.GetChild(_buffPosition[buffType]).GetChild(0).GetChild(0).gameObject;
        var rectTransform = timer.GetComponent<RectTransform>();
        var timerParentHeight = timer.transform.parent.GetComponent<RectTransform>().rect.height;
        var elapsedTime = 0f;
        var initialPosition = rectTransform.localPosition;

        timer.SetActive(true);

        while (elapsedTime < buffDetail.duration)
        {
            elapsedTime += Time.deltaTime;
            rectTransform.localPosition = Vector3.Lerp(initialPosition,
                initialPosition + new Vector3(0f, timerParentHeight, 0f),
                Mathf.Clamp01(elapsedTime / buffDetail.duration));
            yield return null;
        }

        rectTransform.localPosition = initialPosition;
        timer.SetActive(false);
        SwitchPowerStatus(false);

        void SwitchPowerStatus(bool active)
        {
            var amount = active ? buffDetail.multiplicator : 1 / buffDetail.multiplicator;
            switch (buffType)
            {
                case BuffType.SpeedBuff:
                    SpeedBuffActive = active;
                    _playerControl.MovementSpeed *= amount;
                    break;
                case BuffType.JumpBuff:
                    JumpBuffActive = active;
                    _playerControl.JumpPower *= amount;
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

        if (!_endlessRun) endScreen.GetChild(1).gameObject.SetActive(true);
        else
        {
            endScreen.gameObject.SetActive(true);

            ChangeText(1, _coins.ToString());
            ChangeText(2, KillCounter.ToString());
            ChangeText(3, Utility.FormatDoubleWithUnits(transform.position.x / 2.55, true) + "m");

            _playerManager.Buffs[(int)BuffType.JumpBuff].Quantity = _jumpBuffs;
            _playerManager.Buffs[(int)BuffType.SpeedBuff].Quantity = _speedBuffs;
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
        StopCoroutine(UpdateTimer());
        Time.timeScale = 0f;
        display.SetActive(false);

        var winScreen = display.transform.parent.parent.GetChild(1).GetChild(0);
        winScreen.gameObject.SetActive(true);

        var settingsManager = SettingsManager.Instance;

        _coins = _coins > 3 ? 3 : _coins;
        for (var i = 0; i < _coins; i++)
        {
            var endCoin = winScreen.GetChild(0).GetChild(i + 1).GetChild(0);
            endCoin.gameObject.SetActive(true);
            endCoin.GetComponent<AudioSource>().volume =
                settingsManager.SoundEffectVolume * settingsManager.GeneralVolume;
        }

        SaveNewCampaignHighScore();
        Destroy(gameObject);
    }

    private void SaveNewCampaignHighScore()
    {
        var data = SaveAndLoadSystem.LoadCampaign();
        if (_lvlManager.CurrentLvl != 1)
        {
            if (data == null)
                throw new Exception(
                    $"Player is playing on lvl {_lvlManager.CurrentLvl} while campaign file is missing or empty");
            if (data!.Count < _lvlManager.CurrentLvl - 2)
                throw new Exception(
                    $"Player is playing on lvl {_lvlManager.CurrentLvl} while campaign file have only {data!.Count + 1} entries");
        }
        else
        {
            SaveAndLoadSystem.SaveCampaign(new List<CampaignStatusModel> {new(true, _coins, _timer)});
            return;
        }
        
        if (data.Count < _lvlManager.CurrentLvl - 2 || !data[_lvlManager.CurrentLvl - 1].completed)
            data.Add(new CampaignStatusModel(true, _coins, _timer));
        else
        {
            if (data[_lvlManager.CurrentLvl - 1].maxStarNrObtained < _coins)
                data[_lvlManager.CurrentLvl - 1].maxStarNrObtained = _coins;
            
            if (data[_lvlManager.CurrentLvl - 1].bestTime > _timer)
                data[_lvlManager.CurrentLvl - 1].bestTime = _timer;
        }
        
        SaveAndLoadSystem.SaveCampaign(data);
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
    /// Method to update the displayed minAndMaxSpeed buffs count.
    /// </summary>
    private void ShowSpeedBuffs() => ChangeUiText(GetUiTransform(2), _speedBuffs.ToString());

    /// <summary>
    /// Method to update the displayed jump buffs count.
    /// </summary>
    private void ShowJumpBuffs() => ChangeUiText(GetUiTransform(3), _jumpBuffs.ToString());

    private static void ChangeUiText(Component element, string text) => element.GetComponent<TextMeshProUGUI>().text = text;
    private Transform GetUiTransform(int childId) => display.transform.GetChild(childId).GetChild(1);
}