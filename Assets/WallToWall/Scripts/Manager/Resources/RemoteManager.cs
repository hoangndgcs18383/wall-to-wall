using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoogleSheetsToUnity;
using MEC;
using UnityEngine;

public class RemoteManager
{
    private static RemoteManager _instance;
    private RemoteData _remoteData;
    private bool _isLoaded;

    public RemoteData RemoteData => _remoteData;
    public bool IsLoaded => _isLoaded;

    private const string PlayerSheet = "Player";
    private const string TriangleSheet = "Triangle";
    private const string BallSheet = "Ball";

    private PlayerConfig _playerConfig;
    private Action<int> _onActionComplete;

    public static RemoteManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new RemoteManager();
            }

            return _instance;
        }
    }

    private RemoteManager()
    {
        _remoteData = Resources.Load<RemoteData>("RemoteData");
        _playerConfig = Resources.Load<PlayerConfig>("PlayerConfig");
        _isLoaded = _remoteData != null;
    }

    public void InitializeAsync(Action<int> onActionComplete)
    {
        _onActionComplete = onActionComplete;
        Timing.RunCoroutine(InitializeAsync());
    }

    private IEnumerator<float> InitializeAsync()
    {
        yield return Timing.WaitUntilTrue(() => IsLoaded);

        if (IsLoaded)
        {
            Debug.Log("RemoteManager: InitializeAsync: RemoteData is loaded" + _remoteData.sheetId + " " +
                      _remoteData.sheetName);

            //load player
            SpreadsheetManager.Read(new GSTU_Search(_remoteData.sheetId, PlayerSheet),
                OnSpreadsheetPlayer);
            yield return Timing.WaitForOneFrame;
            SpreadsheetManager.Read(new GSTU_Search(_remoteData.sheetId, TriangleSheet),
                OnSpreadsheetTriangle);
            yield return Timing.WaitForOneFrame;
            SpreadsheetManager.Read(new GSTU_Search(_remoteData.sheetId, BallSheet),
                OnSpreadsheetBall);
        }
    }

    private void OnSpreadsheetPlayer(GstuSpreadSheet ss)
    {
        _onActionComplete?.Invoke(30);
        if (ss == null)
        {
            return;
        }

        //player
        SaveSystem.Instance.SetInt(PrefKeys.JumpSpeedX, GetIntValue(PlayerSheet, PrefKeys.JumpSpeedX, ss));
        SaveSystem.Instance.SetInt(PrefKeys.JumpSpeedY, GetIntValue(PlayerSheet, PrefKeys.JumpSpeedY, ss));
        SaveSystem.Instance.SetFloat(PrefKeys.Gravity, GetIntValue(PlayerSheet, PrefKeys.Gravity, ss));
    }

    private void OnSpreadsheetTriangle(GstuSpreadSheet ss)
    {
        _onActionComplete?.Invoke(30);
        if (ss == null)
        {
            return;
        }

        //triangle
        SaveSystem.Instance.SetInt(PrefKeys.ScorePerTriangle,
            GetIntValue(TriangleSheet, PrefKeys.ScorePerTriangle, ss));
        SaveSystem.Instance.SetInt(PrefKeys.NumberOfStart, GetIntValue(TriangleSheet, PrefKeys.NumberOfStart, ss));
        SaveSystem.Instance.SetInt(PrefKeys.NumberOfMax, GetIntValue(TriangleSheet, PrefKeys.NumberOfMax, ss));
    }

    private void OnSpreadsheetBall(GstuSpreadSheet ss)
    {
        //ball
        _onActionComplete?.Invoke(40);
        if (ss == null)
        {
            return;
        }

        for (int i = 0; i < _playerConfig.skins.Count; i++)
        {
            string hash = _playerConfig.skins[i].hash;
            SaveSystem.Instance.SetString(string.Concat(hash, "_", PrefKeys.NameDisplay),
                GetStringValue(_playerConfig.skins[i].hash, PrefKeys.NameDisplay, ss));
            SaveSystem.Instance.SetInt(string.Concat(hash, "_", PrefKeys.UnlockPoint),
                GetIntValue(_playerConfig.skins[i].hash, PrefKeys.UnlockPoint, ss));
        }
    }

    private int GetIntValue(string row, string column, GstuSpreadSheet ss)
    {
        return int.Parse(ss[row, column].value);
    }

    private string GetStringValue(string row, string column, GstuSpreadSheet ss)
    {
        return ss[row, column].value;
    }
}