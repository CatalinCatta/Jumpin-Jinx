﻿using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages level loading and transitions.
/// </summary>
public class LvlManager : IndestructibleManager<LvlManager>
{
    [NonSerialized] public string LvlTitle;
    [NonSerialized] public int CurrentLvl, CoinsInLevel;
    [NonSerialized] public Scene CurrentScene;
    [NonSerialized] public (int, int, int) TimerLimitForStars;
    [NonSerialized] public bool IsCampaign;

    private static SettingsManager _settings;

    private void Start() => _settings = SettingsManager.Instance;

    /// <summary>
    /// Start loading a level.
    /// </summary>
    /// <param name="lvl">The index of the level to load.</param>
    public void StartScene(int lvl)
    {
        CurrentLvl = lvl;
        CurrentScene = lvl >= 1 ? Scene.Campaign : (Scene)lvl;

        _settings.Save();
        StartCoroutine(LoadAsync(Dictionaries.Scene[CurrentScene]));
    }

    private static IEnumerator LoadAsync(string scene)
    {
        var loadingScreen = GameObject.Find("Loading Screen").transform;
        var progressbar = loadingScreen.GetChild(1);
        var quote = Dictionaries.Quote[_settings.Language];
        
        loadingScreen.GetChild(0).gameObject.SetActive(true);
        progressbar.gameObject.SetActive(true);
        progressbar.GetChild(1).GetComponent<TextMeshProUGUI>().text =
            quote[Utility.GetRandomNumberBetween(0, quote.Count)];
        
        var operation = SceneManager.LoadSceneAsync(scene);
        var slider = progressbar.GetChild(0).GetComponent<Slider>();
        var percentage = progressbar.GetChild(2).GetComponent<TextMeshProUGUI>();

        while (!operation.isDone)
        {
            var progress = Mathf.Clamp01(operation.progress / 0.9f);

            slider.value = progress;
            percentage.text = Mathf.RoundToInt(progress * 100) + "%";

            yield return null;
        }
    }
}