using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;

    public GameState State { get; private set; }

    private PaddleManager _paddleManager = null;
    private UIManager _uiManager = null;
    private LevelManager _levelManager = null;


    private void Start()
    {
        Application.targetFrameRate = 60;
        _paddleManager = PaddleManager.Instance;
        _uiManager = UIManager.Instance;
        _levelManager = LevelManager.Instance;
        ChangeState(GameState.LoadingLevel);
    }
    public void ChangeState(GameState newState)
    {
        OnBeforeStateChanged?.Invoke(newState);

        State = newState;
        switch (newState)
        {
            case GameState.LoadingLevel:
                HandleLoadingLevel();
                break;
            case GameState.Starting:
                HandleStarting();
                break;
            case GameState.Playing:
                HandlePlaying();
                break;
            case GameState.Pausing:
                HandlePausing();
                break;
            case GameState.Win:
                HandleWinning();
                break;
            case GameState.Lose:
                HandleLosing();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);

        }

        OnAfterStateChanged?.Invoke(newState);

        Debug.Log($"New state: {newState}");
    }



    private void HandleLoadingLevel()
    {
        _paddleManager.ChangeState(PaddleState.Idle);
        _uiManager.ResetUI();
        ScoreManager.Instance.ResetLevelScore();
        StartCoroutine(LoadLevel());
    }

    private IEnumerator LoadLevel()
    {
        _uiManager.SetActiveUI(false);
        yield return _levelManager.InstantiateLevel();
        _uiManager.SetActiveUI(true);
        yield return _levelManager.ShowCurrentLevel();
        ChangeState(GameState.Starting);
    }

    private void HandleStarting()
    {
        _paddleManager.ChangeState(PaddleState.Waiting);
    }

    private void HandlePlaying()
    {
        _paddleManager.ChangeState(PaddleState.Active);
    }

    private void HandlePausing()
    {
        _paddleManager.ChangeState(PaddleState.Stop);
    }

    private void HandleLosing()
    {
        _paddleManager.ChangeState(PaddleState.Idle);
        _uiManager.SetLosingUI();
    }

    private void HandleWinning()
    {
        _paddleManager.ChangeState(PaddleState.Idle);
        _uiManager.SetWinningUI();
    }

}

[Serializable]
public enum GameState
{   
   LoadingLevel,
   Starting,
   Playing,
   Pausing,
   Win,
   Lose
};
