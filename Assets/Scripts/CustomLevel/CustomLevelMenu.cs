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
    private readonly List<string> _savesNames = new();

    [SerializeField] private Transform pagesParent;
    [SerializeField] private GameObject pagePrefab, savePrefab, createPrefab;
    
    private void Start()
    {
        var path = Path.GetFullPath(@"CustomLevels");
        
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        PrepareBook(path);
    }
    
    public void StartMapBuilder()
    {
        var lvlManager = LvlManager.Instance;
        lvlManager.LvlTitle = Utility.ReturnFirstPossibleName("New map", _savesNames);
        lvlManager.StartScene(-1);
    }

    private void PrepareBook(string path)
    {
        foreach (var save in Directory.GetDirectories(path).ToList().Where(save =>
                     File.Exists(Path.Join(save, Path.GetFileNameWithoutExtension(save) + ".json"))))
            _savesNames.Add(save);
        
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

    private void SetUpSaves(Transform page, string title)
    {
        var save = Instantiate(savePrefab, page).transform;
        save.GetChild(1).GetComponent<TextMeshProUGUI>().text = Path.GetFileNameWithoutExtension(title);
        if (File.Exists(Path.Join(title, "Screenshot.jpg")))
        {
            var texture = new Texture2D(2, 2);
            texture.LoadImage(File.ReadAllBytes(Path.Join(title, "Screenshot.jpg")));
            save.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(texture,
                new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
        }

        var lastTimeEdited = File.GetLastWriteTime(Path.Join(title, Path.GetFileNameWithoutExtension(title) + ".json"));
        save.GetChild(2).GetComponent<ParameterizedLocalizedString>().SetObject(new object[]
            { lastTimeEdited.ToString("HH:mm:ss"), lastTimeEdited.ToString("dd/MM/yyyy") });
    }

}