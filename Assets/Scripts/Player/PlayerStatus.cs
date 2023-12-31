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
    public Transform endScreen;
    private PlayerControl _playerControl;
    private Rigidbody2D _rigidbody;
    private PlayerAudioControl _playerAudioControl;
    private PlayerManager _playerManager;
    private LvlManager _lvlManager;
    private float _timer;

    [Header("Display")]
    private TextMeshProUGUI _hpDisplay, _coinDisplay, _timerDisplay, _speedBuffDisplay, _jumpBuffDisplay;

    [Header("Movement Control Bool")] [NonSerialized]
    public bool
        FreezeFromDamage,
        SpeedBuffActive,
        JumpBuffActive,
        HasCollectedAllCoins;

    [Header("Consumables")] [NonSerialized] public int KillCounter;
    private int
        _coins,
        _gems,
        _hp,
        _jumpBuffs,
        _speedBuffs,
        _coinsInLevel;
    
    [Header("Utilities")]
    private bool _endlessRun;
    
    private void Awake()
    {
        if (display == null) display = GameObject.FindWithTag("Status");

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
        
        _hpDisplay = GetTextMeshProUGUI(0, 0).GetChild(0).GetComponent<TextMeshProUGUI>();
        _coinDisplay = GetTextMeshProUGUI(0, 1).GetComponent<TextMeshProUGUI>();
        _timerDisplay = GetTextMeshProUGUI(1, 0).GetComponent<TextMeshProUGUI>();
        _speedBuffDisplay = GetTextMeshProUGUI(2, 0).GetComponent<TextMeshProUGUI>();
        _jumpBuffDisplay = GetTextMeshProUGUI(2, 1).GetComponent<TextMeshProUGUI>();
            
        ShowLife();

        if (_endlessRun)
        {
            ShowJumpBuffs();
            ShowSpeedBuffs();
        }
        else
        {
            StartCoroutine(UpdateTimer());
            _coinsInLevel = _lvlManager.CoinsInLevel;
        }

        Transform GetTextMeshProUGUI(int child1, int child2) => display.transform.GetChild(child1)
            .GetChild(child2).GetChild(1).transform;
    }

    private IEnumerator UpdateTimer()
    {
        while (true)
        {
            _timer += Time.deltaTime;
            ShowTimer();
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
        HasCollectedAllCoins = _coins == _coinsInLevel;
    }
    
    /// <summary>
    /// Add a coin to the player's inventory.
    /// </summary>
    public void AddGem() => _gems++;

    /// <summary>
    /// Consume a minAndMaxSpeed buff and activate the minAndMaxSpeed buff effect.
    /// </summary>
    public void ConsumeSpeedBuff()
    {
        if (_speedBuffs == 0 || !_endlessRun) return;

        _speedBuffs--;
        ShowSpeedBuffs();
        StartCoroutine(PowerUpTimer(BuffType.SpeedBuff,
            _speedBuffDisplay.transform.parent.GetChild(0).GetChild(0).gameObject));
    }

    /// <summary>
    /// Consume a jump buff and activate the jump buff effect.
    /// </summary>
    public void ConsumeJumpBuff()
    {
        if (_jumpBuffs == 0 || !_endlessRun) return;

        _jumpBuffs--;
        ShowJumpBuffs();
        StartCoroutine(PowerUpTimer(BuffType.JumpBuff,
            _jumpBuffDisplay.transform.parent.GetChild(0).GetChild(0).gameObject));
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
    /// <param name="damageCause">The object that caused the damage of player</param>
    public void GetDamage(Vector3 direction, int amount, ObjectBuildType damageCause)
    {
        _hp -= _endlessRun ? amount * (1 - _playerManager.Upgrades[(int)UpgradeType.Defence].Quantity / 100) : amount;
        ShowLife();

        //FreezeFromDamage = true;
        //_rigidbody.velocity = new Vector2(direction.x * 3, (direction.y - 1.4f) * -5);  // TODO: Improve knock back animation.
        _playerAudioControl.PlayGetHitSound();

        if (_hp == 0) Die(damageCause);
    }

    /// <summary>
    /// Coroutine that activates the power up timer for a certain duration.
    /// </summary>
    private IEnumerator PowerUpTimer(BuffType buffType, GameObject timerGameObject)
    {
        var buffDetail = Dictionaries.BuffDetails[buffType];
        if (!buffDetail.activatable) yield break;

        SwitchPowerStatus(true);

        var rectTransform = timerGameObject.GetComponent<RectTransform>();
        var timerParentHeight = timerGameObject.transform.parent.GetComponent<RectTransform>().rect.height;
        var elapsedTime = 0f;
        var initialPosition = rectTransform.localPosition;

        timerGameObject.SetActive(true);

        while (elapsedTime < buffDetail.duration)
        {
            elapsedTime += Time.deltaTime;
            rectTransform.localPosition = Vector3.Lerp(initialPosition,
                initialPosition + new Vector3(0f, timerParentHeight, 0f),
                Mathf.Clamp01(elapsedTime / buffDetail.duration));
            yield return null;
        }

        rectTransform.localPosition = initialPosition;
        timerGameObject.SetActive(false);
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
    /// <param name="damageCause">The object that caused the death of player</param>
    public void Die(ObjectBuildType? damageCause = ObjectBuildType.Null)
    {
        _playerAudioControl.PlayDieSound();
        gameObject.SetActive(false);

        Time.timeScale = 0f;
        display.SetActive(false);

        endScreen = endScreen == null ? display.transform.parent.parent.GetChild(1) : endScreen;

        if (!_endlessRun) endScreen.GetChild(1).gameObject.SetActive(true);
        else
        {
            endScreen.gameObject.SetActive(true);
            endScreen.GetChild(0).GetChild(damageCause switch
            {
                ObjectBuildType.Spike => 1,
                ObjectBuildType.SpikeUpsideDown => 1,
                ObjectBuildType.SpikeLeft => 1,
                ObjectBuildType.SpikeRight => 1,
                ObjectBuildType.Spider => 2,
                _ => 0
            }).gameObject.SetActive(true);
            
            ChangeText(0, _coins.ToString());
            ChangeText(1, KillCounter.ToString());
            ChangeText(2, Utility.FormatDoubleWithUnits(transform.position.x / 2.55, true) + "m");

            _playerManager.Buffs[(int)BuffType.JumpBuff].Quantity = _jumpBuffs;
            _playerManager.Buffs[(int)BuffType.SpeedBuff].Quantity = _speedBuffs;
            _playerManager.Gold += _coins;
            _playerManager.Gems += _gems;

            _playerManager.Save();

            void ChangeText(int childId, string text) =>
                endScreen.GetChild(1).GetChild(childId).GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
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
        var timeLimits = _lvlManager.TimerLimitForStars;

        _coins = _timer <= timeLimits.Item3 ? 3 : _timer <= timeLimits.Item2 ? 2 : _timer <= timeLimits.Item1 ? 1 : 0;
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
        else if (data == null)
        {
            SaveAndLoadSystem.SaveCampaign(new List<CampaignStatusModel> {new(true, _coins, _timer)});
            return;
        }

        if (data.Count < _lvlManager.CurrentLvl || !data[_lvlManager.CurrentLvl - 1].completed)
            data.Add(new CampaignStatusModel(true, _coins, _timer));
        else if (data[_lvlManager.CurrentLvl - 1].bestTime > _timer)
            data[_lvlManager.CurrentLvl - 1] = new CampaignStatusModel(true, _coins, _timer);

        SaveAndLoadSystem.SaveCampaign(data);
    }

    /// <summary>
    /// Method to update the displayed life count.
    /// </summary>
    private void ShowLife() => ChangeUiText(_hpDisplay, _hp.ToString());

    /// <summary>
    /// Method to update the displayed coin count.
    /// </summary>
    private void ShowCoins() =>
        ChangeUiText(_coinDisplay, _endlessRun ? _coins.ToString() : $"{_coins} / {_coinsInLevel}");
    
    /// <summary>
    /// Method to update the displayed coin count.
    /// </summary>
    private void ShowTimer() => ChangeUiText(_timerDisplay, Utility.TimeToString(_timer));

    /// <summary>
    /// Method to update the displayed minAndMaxSpeed buffs count.
    /// </summary>
    private void ShowSpeedBuffs() => ChangeUiText(_speedBuffDisplay, _speedBuffs.ToString());

    /// <summary>
    /// Method to update the displayed jump buffs count.
    /// </summary>
    private void ShowJumpBuffs() => ChangeUiText(_jumpBuffDisplay, _jumpBuffs.ToString());

    private static void ChangeUiText(Component element, string text) =>
        element.GetComponent<TextMeshProUGUI>().text = text;
}