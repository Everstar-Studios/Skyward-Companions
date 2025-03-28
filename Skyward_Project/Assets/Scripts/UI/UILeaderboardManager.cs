using System;
using TMPro;
using Unity.Services.Leaderboards;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UILeaderboardManager : MonoBehaviour
{
    [SerializeField] private int playersPerPage = 25;
    [SerializeField] private LeaderboardPlayerItem playerItemPrefab;
    [SerializeField] private RectTransform container;
    [SerializeField] private TextMeshProUGUI pageText;
    [Header("Debug")] 
    [SerializeField] private int addScoreAmount = 10;
    [SerializeField] private Button addScoreButton;
    
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    private int currentPage = 1;
    private int totalPages = 0;

    private void OnEnable()
    {
        ClearPlayersList();
        nextButton.onClick.AddListener(NextPage);
        prevButton.onClick.AddListener(PreviousPage);
        addScoreButton.onClick.AddListener(AddScore);

        currentPage = 1;
        totalPages = 0;
        LoadPlayers(1);
    }

    private void AddScore()
    {
        AddScore(addScoreAmount);
    }

    private static int multiplier = 1;
    public async void AddScore(int score)
    {
        addScoreButton.interactable = false;
        

        multiplier++;
        LoadPlayers(currentPage);
        
        addScoreButton.interactable = true;
    }
    
    public async void LoadPlayers(int page)
    {
        nextButton.interactable = false;
        prevButton.interactable = false;

        GetScoresOptions options = new();
        options.Offset = (page - 1) * playersPerPage;
        options.Limit = playersPerPage;
        var scores = await LeaderboardsService.Instance.GetScoresAsync("Skyward-Leaderboard", options);
        ClearPlayersList();
        for (int i = 0; i < scores.Results.Count; i++)
        {
            LeaderboardPlayerItem item = Instantiate(playerItemPrefab, container);
            item.Initialize(scores.Results[i]);
        }

        totalPages = Mathf.CeilToInt((float)scores.Total / scores.Limit);
        currentPage = page;

        pageText.text = currentPage + "/" + totalPages;
        nextButton.interactable = currentPage < totalPages && totalPages > 1;
        prevButton.interactable = currentPage > 1 && totalPages > 1;
    }

    private void NextPage()
    {
        int nextPage = currentPage + 1;
        if (nextPage > totalPages)
            LoadPlayers(1);
        else
            LoadPlayers(nextPage);
    }

    private void PreviousPage()
    {
        if (currentPage - 1 <= 0)
            LoadPlayers(totalPages);
        else
            LoadPlayers(currentPage - 1);
    }

    private void ClearPlayersList()
    {
        LeaderboardPlayerItem[] items = container.GetComponentsInChildren<LeaderboardPlayerItem>();
        if (items == null)
            return;

        foreach (var item in items)
            Destroy(item.gameObject);

    }
}
