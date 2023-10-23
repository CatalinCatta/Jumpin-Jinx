using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the custom level menu and its interaction.
/// </summary>
public class CustomLevelMenu : MonoBehaviour
{
    private List<string> _savesNames;

    [SerializeField] private Transform pagesParent;
    [SerializeField] private GameObject pagePrefab, savePrefab, createPrefab;
    
    private void Start()
    {
        var path = Path.GetFullPath(@"CustomLevels");
        
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        else PrepareBook(path);
    }
    
    public void StartMapBuilder()
    {
        var lvlManager = LvlManager.Instance;
        
        lvlManager.LvlTitle = "";
        lvlManager.StartScene(-1);
    }

    private void PrepareBook(string path)
    {
        _savesNames = Directory.GetFiles(path, "*.json").ToList();

        Transform lastPage = null;
        var maxPage = (int)Math.Ceiling((double)(_savesNames.Count + 1) / 3);
        
        for (var i = 0; i < maxPage; i++)
        {
            lastPage = SetUpPage(i, maxPage);
            for (var j = 3 * i; j < _savesNames.Count && j < 3 * (i + 1); j++)
                SetUpSaves(lastPage.GetChild(j - 3 * i), _savesNames[j]);
        }

        pagesParent.GetChild(1).gameObject.SetActive(true);
        Instantiate(createPrefab, lastPage!.GetChild(_savesNames.Count % 3)).GetComponent<Button>().onClick
            .AddListener(StartMapBuilder);
    }

    private Transform SetUpPage(int currentPage, int maxPage)
    {
        var page = Instantiate(pagePrefab, pagesParent).transform;
        var handler = page.GetChild(3);
        var prevButton = handler.GetChild(0).GetComponent<Button>();
        var nextButton = handler.GetChild(1).GetComponent<Button>();

        if (currentPage == 0) prevButton.gameObject.SetActive(false);
        else
            prevButton.onClick.AddListener(() =>
            {
                Debug.Log(page == pagesParent.GetChild(currentPage));
                page.gameObject.SetActive(false);
                pagesParent.GetChild(currentPage).gameObject.SetActive(true);
            });

        if (currentPage + 1 == maxPage) nextButton.gameObject.SetActive(false);
        else
            nextButton.onClick.AddListener(() =>
            {
                page.gameObject.SetActive(false);
                pagesParent.GetChild(currentPage + 2).gameObject.SetActive(true);
            });
        
        handler.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"{currentPage + 1}/{maxPage}";
        page.gameObject.SetActive(false);
        return page;
    }

    private void SetUpSaves(Transform page, string title) =>
        Instantiate(savePrefab, page).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
            Path.GetFileNameWithoutExtension(title);

}