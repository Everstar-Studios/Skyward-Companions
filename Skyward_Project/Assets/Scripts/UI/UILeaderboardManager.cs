using System;
using System.Collections;
using TMPro;
using Unity.Services.Leaderboards;
using UnityEngine;
using UnityEngine.UI;

public class UILeaderboardManager : MonoBehaviour
{
    [SerializeField] private int playersPerPage = 25;
    [SerializeField] private LeaderboardPlayerItem playerItemPrefab;
    [SerializeField] private LeaderboardPlayerItem localPlayerItemPrefab;
    [SerializeField] private RectTransform container;
    [SerializeField] private RectTransform localContainer;
    [SerializeField] private TextMeshProUGUI pageText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private Transform levelLayoutGroup;

    private string currentLeaderboardId;
    private int currentPage = 1;
    private int totalPages = 0;

    public GameObject leaderboardLevelButtonPrefab;
    
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
            
            int startIndex = (page - 1) * playersPerPage + 1;
            int endIndex = startIndex + playersPerPage - 1;

            bool foundPlayer = false;
            foreach (var score in scores.Results)
            {
                LeaderboardPlayerItem item;

                if (score.PlayerName == PlayerSystem.PlayerFullName)
                {
                    item = Instantiate(localPlayerItemPrefab, container);
                    foundPlayer = true;
                }
                else
                    item = Instantiate(playerItemPrefab, container);
                
                item.Initialize(score);
            }

            if (!foundPlayer)
            {
                try
                {
                    var localScore = await LeaderboardsService.Instance.GetPlayerScoreAsync(currentLeaderboardId);
                    var bottomItem = Instantiate(localPlayerItemPrefab, localContainer);
                    bottomItem.Initialize(localScore);
                }
                catch (Exception e)
                {
                    Debug.Log("Local player has no score yet: " + e.Message);
                }
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
        foreach (Transform child in localContainer)
            Destroy(child.gameObject);
    }
}