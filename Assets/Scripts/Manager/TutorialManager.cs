using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Manages the tutorial elements and animations.
/// </summary>
public class TutorialManager : MonoBehaviour
{
    [Header("Tutorials")] [SerializeField] private Transform
        movement,
        spike,
        watter,
        enemy,
        platforms,
        temporaryPlatform,
        heal;

    [Header("Marks")] [SerializeField] private GameObject
        questionMarks,
        exclamationMarks,
        mergedMarks;

    private static string ReformatKeyCodeAsString(KeyCode keyCode) =>
        Regex.Replace(Enum.GetName(typeof(KeyCode), keyCode)!, @"(\p{Lu})", " $1").Trim();

    public void SetUpMovement()
    {
        gameObject.SetActive(true);
        questionMarks.SetActive(true);
        movement.gameObject.SetActive(true);
        var settingsManager = SettingsManager.Instance;

        SetUpKeyCode(0, settingsManager.MoveLeftKeyCode);
        SetUpKeyCode(1, settingsManager.MoveRightKeyCode);
        SetUpKeyCode(2, settingsManager.JumpKeyCode);

        void SetUpKeyCode(int child, KeyCode keyCode) => movement.GetChild(child)
            .GetComponent<ParameterizedLocalizedString>().SetObject(new object[] { ReformatKeyCodeAsString(keyCode) });
    }

    public void SetUpSpike()
    {
        gameObject.SetActive(true);
        exclamationMarks.SetActive(true);
        spike.gameObject.SetActive(true);
    }

    public void SetUpWatter()
    {
        gameObject.SetActive(true);
        exclamationMarks.SetActive(true);
        watter.gameObject.SetActive(true);
    }

    public void SetUpEnemy()
    {
        gameObject.SetActive(true);
        mergedMarks.SetActive(true);
        enemy.gameObject.SetActive(true);

        enemy.GetChild(0).GetComponent<ParameterizedLocalizedString>().SetObject(new object[]
            { ReformatKeyCodeAsString(SettingsManager.Instance.FireKeyCode) });
    }

    public void SetUpPlatform()
    {
        gameObject.SetActive(true);
        questionMarks.SetActive(true);
        platforms.gameObject.SetActive(true);
    }

    public void SetUpTemporaryPlatform()
    {
        gameObject.SetActive(true);
        questionMarks.SetActive(true);
        temporaryPlatform.gameObject.SetActive(true);
    }

    public void SetUpHeal()
    {
        gameObject.SetActive(true);
        questionMarks.SetActive(true);
        heal.gameObject.SetActive(true);
    }
}