using System;
using System.Collections;
using TMPro;
using Unity.Services.Leaderboards;
using UnityEngine;
using UnityEngine.UI;

public class UILeaderboardManager : MonoBehaviour
{
    public static UILeaderboardManager Instance { get; private set; }

    [SerializeField] private int playersPerPage = 25;
    [SerializeField] private LeaderboardPlayerItem playerItemPrefab;
    [SerializeField] private RectTransform container;
    [SerializeField] private TextMeshProUGUI pageText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private Transform levelLayoutGroup;

    private string currentLeaderboardId;
    private int currentPage = 1;
    private int totalPages = 0;

    public GameObject leaderboardLevelButtonPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        
        
    }
    
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => ConfigSystem.Instance != null);
        yield return ConfigSystem.AllConfigurationsLoaded;

        foreach (var level in ConfigSystem.GetConfig<LevelConfig>().levels)
        {
            string levelName = level.sceneLabel.labelString;
            var split = levelName.Split("_0");
            string beautifiedLevelName = split[0] + " " + split[1];
            var levelButton = Instantiate(leaderboardLevelButtonPrefab, levelLayoutGroup.transform).GetComponent<LeaderboardLevelButton>();
            levelButton.GetComponentInChildren<TMP_Text>().text = beautifiedLevelName;
            levelButton.GetComponent<Button>().onClick.AddListener(() => ShowLeaderboardForLevel(levelName));
        }
    }

    private void OnEnable()
    {
        nextButton.onClick.AddListener(NextPage);
        prevButton.onClick.AddListener(PreviousPage);
    }

    private void OnDisable()
    {
        ClearPlayersList();
        nextButton.onClick.RemoveListener(NextPage);
        prevButton.onClick.RemoveListener(PreviousPage);
    }

    private void ShowLeaderboardForLevel(string levelName)
    {
        currentLeaderboardId = $"Skyward-{levelName}";
        currentPage = 1;
        LoadPlayers(currentPage);
    }

    public async void LoadPlayers(int page)
    {
        nextButton.interactable = false;
        prevButton.interactable = false;

        GetScoresOptions options = new()
        {
            Offset = (page - 1) * playersPerPage,
            Limit = playersPerPage
        };

        try
        {
            var scores = await LeaderboardsService.Instance.GetScoresAsync(currentLeaderboardId, options);
            ClearPlayersList();

            foreach (var score in scores.Results)
            {
                var item = Instantiate(playerItemPrefab, container);
                item.Initialize(score);
            }

            totalPages = Mathf.CeilToInt((float)scores.Total / scores.Limit);
            currentPage = page;

            pageText.text = $"{currentPage}/{totalPages}";
            nextButton.interactable = currentPage < totalPages;
            prevButton.interactable = currentPage > 1;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load leaderboard: {e.Message}");
            ClearPlayersList();
            pageText.text = "0/0";
        }
    }

    private void NextPage() => LoadPlayers(currentPage + 1);
    private void PreviousPage() => LoadPlayers(currentPage - 1);
    
    private void ClearPlayersList()
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);
    }
}