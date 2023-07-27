using System.Collections;
using TMPro;
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

            currentTransform.position = new Vector3(x, y, 0f);

            if (Mathf.Approximately(y, targetY))
                targetY = UnityEngine.Random.Range(3f, 5f);

            speedX = x <= -10f ? 1f : 
                x >= 10f ? -1f : speedX;

            yield return null;
        }
    }

    public void SetUpMovement()
    {
        gameObject.SetActive(true);
        questionMarks.SetActive(true);
        movement.gameObject.SetActive(true);

        var leftMovement = Regex.Replace(Enum.GetName(typeof(KeyCode), SettingsManager.Instance.moveLeftKeyCode)!,
            @"(\p{Lu})", " $1").Trim();
        var rightMovement = Regex.Replace(Enum.GetName(typeof(KeyCode), SettingsManager.Instance.moveRightKeyCode)!,
            @"(\p{Lu})", " $1").Trim();
        var jump = Regex.Replace(Enum.GetName(typeof(KeyCode), SettingsManager.Instance.jumpKeyCode)!,
            @"(\p{Lu})", " $1").Trim();
        
        movement.GetChild(0).GetComponent<TextMeshPro>().text = SettingsManager.Instance.language == Language.English? $"<- Press [{leftMovement}] to move left <-" : $"<- Apasa [{leftMovement}] pentru a te misca la stanga <-" ;
        movement.GetChild(1).GetComponent<TextMeshPro>().text = SettingsManager.Instance.language == Language.English? $"-> Press [{rightMovement}] to move right ->" : $"-> Apasa [{rightMovement}] pentru a te misca la dreapta ->";
        movement.GetChild(2).GetComponent<TextMeshPro>().text = SettingsManager.Instance.language == Language.English? $"^ Press [{jump}] to jump ^" : $"^ Apasa [{jump}] pentru a sari ^";
    }

    public void SetUpSpike()
    {
        gameObject.SetActive(true);
        exclamationMarks.SetActive(true);
        spike.gameObject.SetActive(true);

        spike.GetChild(0).GetComponent<TextMeshPro>().text = SettingsManager.Instance.language == Language.English? "Be Carreful" : "Atentie" ;
        spike.GetChild(1).GetComponent<TextMeshPro>().text = SettingsManager.Instance.language == Language.English? "!!! Don't touch the spikes !!!" : "!!! Nu atinge tepii !!!" ;
    }

    public void SetUpWatter()
    {
        gameObject.SetActive(true);
        exclamationMarks.SetActive(true);
        watter.gameObject.SetActive(true);

        watter.GetChild(0).GetComponent<TextMeshPro>().text = SettingsManager.Instance.language == Language.English? "! DANGER !" : "! PERICOL !" ;
        watter.GetChild(1).GetComponent<TextMeshPro>().text = SettingsManager.Instance.language == Language.English? "!!! Don't fall in watter !!!" : "!!! Nu cadea în apa !!!" ;
    }
    
    public void SetUpEnemy()
    {
        gameObject.SetActive(true);
        mergedMarks.SetActive(true);
        enemy.gameObject.SetActive(true);
        
        var shoot = Regex.Replace(Enum.GetName(typeof(KeyCode), SettingsManager.Instance.fireKeyCode)!,
            @"(\p{Lu})", " $1").Trim();

        enemy.GetChild(0).GetComponent<TextMeshPro>().text = SettingsManager.Instance.language == Language.English? $"Press [{shoot}] to shoot" : $"Apasa [{shoot}] pentru a trage" ;
        enemy.GetChild(1).GetComponent<TextMeshPro>().text = SettingsManager.Instance.language == Language.English? "!!! Avoid touching enemies !!!" : "!!! Evita sa atingi inamicii !!!" ;
    }
    
    public void SetUpPlatform()
    {
        gameObject.SetActive(true);
        questionMarks.SetActive(true);
        platforms.gameObject.SetActive(true);

        platforms.GetChild(0).GetComponent<TextMeshPro>().text = SettingsManager.Instance.language == Language.English? "Some platforms Will move around" : "Unele platforme se vor misca" ;
    }
    
    public void SetUpTemporaryPlatform()
    {
        gameObject.SetActive(true);
        questionMarks.SetActive(true);
        temporaryPlatform.gameObject.SetActive(true);

        temporaryPlatform.GetChild(0).GetComponent<TextMeshPro>().text = SettingsManager.Instance.language == Language.English? "Some platforms will disappear after touching" : "Unele platforme vor disparea dupa ce le atingi" ;
    }
    
    public void SetUpHeal()
    {
        gameObject.SetActive(true);
        questionMarks.SetActive(true);
        heal.gameObject.SetActive(true);

        heal.GetChild(0).GetComponent<TextMeshPro>().text = SettingsManager.Instance.language == Language.English? "Pick up hearts on your way to regenerate your hp" : "Aduna inimioare in drumul tau pentru a-ti regenera viata" ;
    }
    
}
