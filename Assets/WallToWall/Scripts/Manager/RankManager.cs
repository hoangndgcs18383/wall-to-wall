using System;
using System.Collections.Generic;
using System.Linq;
using GoogleSheetsToUnity;
using Unity.Services.Authentication;
using UnityEngine;

public class RankManager
{
    #region Variables

    private static RankManager _instance;

    private Dictionary<string, int> _rankList = new Dictionary<string, int>();
    private readonly int _maxRank = 10;
    private const string RankPrefKey = "Rank_{index}";
    private event Action OnRankChanged;
    private bool _isInitialized;
    private string _currentUid;

    #endregion

    #region Properties

    public static RankManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new RankManager();
                _instance.Initialize();
            }

            return _instance;
        }
    }

    public Dictionary<string, int> GetRankList
    {
        get => _rankList;
        private set => _rankList = value;
    }

    #endregion

    private void Initialize()
    {
        if (_isInitialized) return;
        _isInitialized = true;
        GetRankList.Clear();
        _currentUid = AuthenticationService.Instance.PlayerId;
        if (!string.IsNullOrWhiteSpace(_currentUid))
        {
            Debug.Log($"{_currentUid} has been initialized");

            ValueRange inputData = new ValueRange(new List<string> { _currentUid });
            inputData.Add(new List<string>
            {
                "100"
            });
            
            SpreadsheetManager.Write(new GSTU_Search(RemoteManager.Instance.RemoteData.sheetId, "LeaderBoard"),
                inputData, null);
        }

        for (int i = 0; i < _maxRank; i++)
        {
            string key = RankPrefKey.Replace("{index}", i.ToString());
            GetRankList.Add(key, PlayerPrefs.GetInt(key, -1));
        }

        SortRank();
        OnRankChanged?.Invoke();
    }

    public void SetRank(int currentRank)
    {
        if (currentRank >= 1)
        {
            SoftRankHasEmptySlot(currentRank);
        }
    }

    private bool SoftRankHasEmptySlot(int bestScore)
    {
        if (bestScore >= 1)
        {
            //check best score if exist -1
            var firstOrDefault = GetRankList.Where(r => r.Value == -1).FirstOrDefault(e =>
            {
                GetRankList[e.Key] = bestScore;
                return true;
            });

            if (firstOrDefault.Key != null)
            {
                PlayerPrefs.SetInt(firstOrDefault.Key, bestScore);
                return true;
            }

            if (!SoftRankHasSlot(bestScore))
            {
                PlayerPrefs.SetInt(firstOrDefault.Key, bestScore);
            }
        }

        return false;
    }

    private bool SoftRankHasSlot(int bestScore)
    {
        var find = GetRankList.Where(r => r.Value < bestScore).FirstOrDefault(e =>
        {
            GetRankList[e.Key] = bestScore;
            return true;
        });

        if (find.Key != null)
        {
            PlayerPrefs.SetInt(find.Key, bestScore);
        }

        return find.Key != null;
    }

    public void SortRank()
    {
        GetRankList = GetRankList.OrderByDescending(r => r.Value).ToDictionary(r => r.Key, r => r.Value);
    }

    public void AddListenerRankChanged(Action action)
    {
        OnRankChanged += action;
    }

    public void RemoveListenerRankChanged(Action action)
    {
        OnRankChanged -= action;
    }

    public void CheatRank()
    {
        Debug.Log("RankManager CheatRank");
        for (int i = 0; i < 10; i++)
        {
            string key = RankPrefKey.Replace("{index}", i.ToString());
            PlayerPrefs.SetInt(key, i + 2);
        }
    }

    public void UpdateRank()
    {
        Initialize();
    }

    public void ClearRank()
    {
        PlayerPrefs.DeleteAll();
    }
}