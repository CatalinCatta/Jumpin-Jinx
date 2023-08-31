using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages the custom level menu and its interaction.
/// </summary>
public class CustomLevelMenu : MonoBehaviour
{
    [SerializeField] private Transform contentPlace, menu, revenue, saveModel;

    private List<string> _savesNames;

    private void Start()
    {
        var path = Path.GetFullPath(@"CustomLevels");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            return;
        }

        _savesNames = Directory.GetFiles(path, "*.json").ToList();
        foreach (var saveName in _savesNames)
            Instantiate(saveModel, contentPlace).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                Path.GetFileNameWithoutExtension(saveName);
    }
    
    public void StartMapBuilder()
    {
        LvlManager.LvlTitle = "";
        LvlManager.Instance.StartLevel(-1);
    }

    public void Enable() => StartCoroutine(ActivateMenu());

    public void Disable() => StartCoroutine(CloseMenu());

    public void EnableLocalLevels() => StartCoroutine(DisappearMenu());

    public void DisableLocalLevels() => StartCoroutine(EnableMenu());

    private IEnumerator ActivateMenu()
    {
        var direction = (Random.Range(0, 2) * 2 - 1) * (int)MenuDirection.Random;
        var menuChildNr = menu.childCount;

        StartCoroutine(AnimateOneMenu(menu.GetChild(menuChildNr - 2), false, (int)MenuDirection.Left));
        StartCoroutine(AnimateOneMenu(menu.GetChild(menuChildNr - 1), false, (int)MenuDirection.Right));
        yield return new WaitForSeconds(0.2f);

        for (var i = 1; i < menuChildNr - 2; i++)
        {
            StartCoroutine(AnimateOneMenu(menu.GetChild(i), false, direction));
            yield return new WaitForSeconds(0.2f);
            direction *= -1;
        }

        var currentTransform = transform;
        var childCount = currentTransform.childCount - 1;

        for (var i = 1; i < childCount; i++)
        {
            StartCoroutine(AnimateOneMenu(currentTransform.GetChild(i), true, direction));
            yield return new WaitForSeconds(0.2f);
            direction *= -1;
        }
    }

    private IEnumerator CloseMenu()
    {
        var direction = (Random.Range(0, 2) * 2 - 1) * (int)MenuDirection.Random;
        var currentTransform = transform;
        var childCount = currentTransform.childCount - 1;
        var menuChildNr = menu.childCount;

        for (var i = 1; i < childCount; i++)
        {
            StartCoroutine(AnimateOneMenu(currentTransform.GetChild(i), false, direction));
            yield return new WaitForSeconds(0.2f);
            direction *= -1;
        }

        for (var i = 1; i < menuChildNr - 2; i++)
        {
            StartCoroutine(AnimateOneMenu(menu.GetChild(i), true, direction));
            yield return new WaitForSeconds(0.2f);
            direction *= -1;
        }

        StartCoroutine(AnimateOneMenu(menu.GetChild(menuChildNr - 2), true, (int)MenuDirection.Left));
        StartCoroutine(AnimateOneMenu(menu.GetChild(menuChildNr - 1), true, (int)MenuDirection.Right));
    }

    private IEnumerator DisappearMenu()
    {
        var direction = (Random.Range(0, 2) * 2 - 1) * (int)MenuDirection.Random;
        var currentTransform = transform;
        var childCount = currentTransform.childCount - 1;

        StartCoroutine(AnimateOneMenu(revenue, false, (int)MenuDirection.Right));

        for (var i = 1; i < childCount; i++)
        {
            StartCoroutine(AnimateOneMenu(currentTransform.GetChild(i), false, direction));
            direction *= -1;
        }

        yield return new WaitForSeconds(0.2f);

        StartCoroutine(AnimateLocalLevels(true));
    }

    private IEnumerator EnableMenu()
    {
        StartCoroutine(AnimateLocalLevels(false));

        var direction = (Random.Range(0, 2) * 2 - 1) * (int)MenuDirection.Random;
        var currentTransform = transform;
        var childCount = currentTransform.childCount - 1;

        StartCoroutine(AnimateOneMenu(revenue, true, (int)MenuDirection.Right));

        for (var i = 1; i < childCount; i++)
        {
            StartCoroutine(AnimateOneMenu(currentTransform.GetChild(i), true, direction));
            direction *= -1;
        }

        yield return new WaitForSeconds(0.5f);
    }

    private static IEnumerator AnimateOneMenu(Component component, bool isAppearing, int direction)
    {
        if (isAppearing)
            component.gameObject.SetActive(true);

        var currentTime = 0f;
        var objectToMove = component.GetComponent<RectTransform>();

        while (currentTime < 0.3f)
        {
            currentTime += Time.deltaTime;

            objectToMove.anchoredPosition =
                new Vector2(Mathf.Lerp(isAppearing ? direction : 0f, isAppearing ? 0f : direction, currentTime / 0.3f),
                    0f);

            yield return null;
        }

        if (!isAppearing)
            component.gameObject.SetActive(false);

        objectToMove.anchoredPosition = new Vector2(0f, 0f);
    }

    private IEnumerator AnimateLocalLevels(bool isAppearing)
    {
        var component = transform.GetChild(transform.childCount - 1);

        if (isAppearing)
            component.gameObject.SetActive(true);

        var currentTime = 0f;

        var objectToMove = component.GetComponent<RectTransform>();
        while (currentTime < 0.5f)
        {
            currentTime += Time.deltaTime;

            objectToMove.localScale =
                new Vector3(Mathf.Lerp(isAppearing ? 0f : 1f, isAppearing ? 1f : 0f, currentTime / 0.3f),
                    Mathf.Lerp(isAppearing ? 0f : 1f, isAppearing ? 1f : 0f, currentTime / 0.3f), 1f);

            yield return null;
        }

        if (!isAppearing)
            component.gameObject.SetActive(false);

        objectToMove.localScale = new Vector3(1f, 1f, 1f);
    }
}