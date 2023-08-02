using System.Collections;
using UnityEngine;

public class EndlessMenuSetup : MonoBehaviour
{
    [SerializeField] private Transform menu;

    private enum Direction
    {
        Left = -1500,
        Right = 1500,
        Random = 2000
    }
    
    public void Enable() =>
        StartCoroutine(ActivateMenu());

    public void Disable() =>
        StartCoroutine(CloseMenu());
    
    public void ShowUpgrades() =>
        StartCoroutine(AnimateCard(transform.GetChild(2), true));
    
    public void HideUpgrades() =>
        StartCoroutine(AnimateCard(transform.GetChild(2), false));

    public void ShowShop() =>
        StartCoroutine(AnimateCard(transform.GetChild(3), true));
    
    public void HideShop() =>
        StartCoroutine(AnimateCard(transform.GetChild(3), false));
    
    public void ShowStore()
    {
        var store = transform.GetChild(4);
        store.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        StartCoroutine(AnimateCard(store, true));
    }
    
    public void HideStore() =>
        StartCoroutine(AnimateCard(transform.GetChild(4), false));
    
    public void ShowStoreEverywhere()
    {
        var store = transform.GetChild(4);
        store.GetComponent<RectTransform>().localScale = new Vector3(1.3f, 1.3f, 1f);
        StartCoroutine(AnimateCard(store, true, -300));
    }

    private IEnumerator ActivateMenu()
    {
        var direction = (Random.Range(0, 2) * 2 - 1) * (int)Direction.Random;
        foreach (Transform child in menu)
        {
            StartCoroutine(AnimateOneMenu(child, false, direction));
            yield return new WaitForSeconds(0.2f);
            direction *= -1;
        }

        var buttons = transform.GetChild(1);
        var childCount = buttons.childCount; 
        
        for (var i = childCount - 1; i >= 0; i--)
        {
            StartCoroutine(AnimateOneMenu(buttons.GetChild(i), true, (int)Direction.Right));
            yield return new WaitForSeconds(0.2f);
        }

        StartCoroutine(AnimateOneMenu(transform.GetChild(0), true, (int)Direction.Left));
    }
    
    private IEnumerator CloseMenu()
    {
        StartCoroutine(AnimateOneMenu(transform.GetChild(0), false, (int)Direction.Left));

        var buttons = transform.GetChild(1);
        var childCount = buttons.childCount; 

        for (var i = childCount - 1; i >= 0; i--)
        {
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(AnimateOneMenu(buttons.GetChild(i), false, (int)Direction.Right));
        }

        var direction = (Random.Range(0, 2) * 2 - 1) * (int)Direction.Random;
        foreach (Transform child in menu)
        {
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(AnimateOneMenu(child, true, direction));
            direction *= -1;
        }
    }

    private IEnumerator AnimateOneMenu(Component component, bool isAppearing, int direction)
    {
        if (isAppearing)
            component.gameObject.SetActive(true);
        
        var currentTime = 0f;
        var objectToMove = component.GetComponent<RectTransform>();
        
        while (currentTime < 0.3f)
        {
            currentTime += Time.deltaTime;
            
            objectToMove.anchoredPosition = 
                new Vector2(Mathf.Lerp(isAppearing? direction : 0f, isAppearing? 0f : direction, currentTime / 0.3f), 0f);

            yield return null;
        }

        if (!isAppearing)
            component.gameObject.SetActive(false);

        objectToMove.anchoredPosition = new Vector2(0f, 0f);
    }

    private IEnumerator AnimateCard(Component card, bool isAppearing, float? finalXPosition = 0f)
    {
        if (isAppearing)
            card.gameObject.SetActive(true);
        
        var currentTime = 0f;
        var objectToMove = card.GetComponent<RectTransform>();
        
        while (currentTime < 0.5f)
        {
            currentTime += Time.deltaTime;
            
            objectToMove.anchoredPosition = 
                new Vector2(Mathf.Lerp(isAppearing? 1500f : (float)finalXPosition, isAppearing? (float)finalXPosition : 1500f, currentTime / 0.5f), Mathf.Lerp(isAppearing? 1500f : 0f, isAppearing? 0f :- 1500f, currentTime / 0.5f));

            objectToMove.rotation = 
                Quaternion.Euler(0f, 0f, Mathf.Lerp(isAppearing? -720f : 0f, isAppearing? 0f : -720f, currentTime / 0.5f));

            yield return null;
        }

        if (!isAppearing)
            card.gameObject.SetActive(false);

        objectToMove.anchoredPosition = new Vector2((float)finalXPosition, 0f);
        objectToMove.rotation = Quaternion.Euler(0, 0, 0);
    }
}
