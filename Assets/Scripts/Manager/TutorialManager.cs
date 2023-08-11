using System.Collections;
using UnityEngine;
using System;
using System.Text.RegularExpressions;


public class TutorialManager : MonoBehaviour
{
    [SerializeField] private Transform movement;
    [SerializeField] private Transform spike;
    [SerializeField] private Transform watter;
    [SerializeField] private Transform enemy;
    [SerializeField] private Transform platforms;
    [SerializeField] private Transform temporaryPlatform;
    [SerializeField] private Transform heal;
    
    
    [SerializeField] private GameObject questionMarks;
    [SerializeField] private GameObject exclamationMarks;
    [SerializeField] private GameObject mergedMarks;
    
    private void OnEnable() =>
        StartCoroutine(MovementAnimation());

    private IEnumerator MovementAnimation()
    {
        var currentTransform = transform;
        var position = currentTransform.position; 
        var (x, y) = (position.x, position.y);
        var speedX = Utils.RandomPickNumberBetween(0, 2) == 0 ? -1f : 1f;
        var targetY = UnityEngine.Random.Range(3f, 5f);

        while (true)
        {
            x += speedX * Time.deltaTime;
            y = Mathf.MoveTowards(y, targetY, Time.deltaTime / 2);

            currentTransform.position = new Vector3(x, y, 10f);

            if (Mathf.Approximately(y, targetY))
                targetY = UnityEngine.Random.Range(3f, 5f);

            speedX = x <= -10f ? 1f : 
                x >= 10f ? -1f : speedX;

            yield return null;
        }
    }

    private static string ReformatKeyCodeAsString(KeyCode keyCode) =>
        Regex.Replace(Enum.GetName(typeof(KeyCode), keyCode)!,
            @"(\p{Lu})", " $1").Trim();
    
    public void SetUpMovement()
    {
        gameObject.SetActive(true);
        questionMarks.SetActive(true);
        movement.gameObject.SetActive(true);

        movement.GetChild(0).GetComponent<ParameterizedLocalizedString>().SetObject(new object[]
            { ReformatKeyCodeAsString(SettingsManager.Instance.moveLeftKeyCode) });

        movement.GetChild(1).GetComponent<ParameterizedLocalizedString>().SetObject(new object[]
            { ReformatKeyCodeAsString(SettingsManager.Instance.moveRightKeyCode) });

        movement.GetChild(2).GetComponent<ParameterizedLocalizedString>().SetObject(new object[]
            { ReformatKeyCodeAsString(SettingsManager.Instance.jumpKeyCode) });
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
            { ReformatKeyCodeAsString(SettingsManager.Instance.fireKeyCode) });
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
