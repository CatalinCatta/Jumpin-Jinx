﻿using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages level loading and transitions.
/// </summary>
public class LvlManager : MonoBehaviour
{
    [Header("Singleton Instance")] [NonSerialized]
    public static LvlManager Instance;

    [Header("Current Level")] [NonSerialized]
    public static string LvlTitle;
    [NonSerialized] public int CurrentLvl;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Start loading a level.
    /// </summary>
    /// <param name="lvl">The index of the level to load.</param>
    public void StartLevel(int lvl)
    {
        CurrentLvl = lvl;
        SettingsManager.Instance.Save();

        StartCoroutine(LoadAsync(lvl switch
        {
            -1 => "Local Game",
            0 => "EndlessRun",
            _ => "Grass Lvl"
        }, lvl == 13));
    }

    private static IEnumerator LoadAsync(string scene, bool delay)
    {
        var loadingScreen = GameObject.Find("Loading Screen").transform;
        var progressbar = loadingScreen.GetChild(1);

        loadingScreen.GetChild(0).gameObject.SetActive(true);
        progressbar.gameObject.SetActive(true);

        // *** TO DO *** : add more inspirational quote in different language.
        progressbar.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Some inspirational quote";

        if (delay)
            yield return new WaitForSeconds(10);

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