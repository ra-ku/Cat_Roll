using UnityEngine;

public class GameManager : IManager
{
    public enum GameState
    {
        MainMenu,
        Loading,
        Playing,
        Paused,
        GameOver
    }

    public GameState _state;

    public void Init()
    {
        _state = GameState.MainMenu;

        var input = Managers.Instance.Get<InputManager>();
        if (input != null)
        {
            input.OnStartGameEvent -= StartGame;
            input.OnStartGameEvent += StartGame;
        }

        var sceneManager = Managers.Instance.Get<SceneManagerEx>();
        if (sceneManager != null)
        {
            sceneManager.OnSceneInitComplete -= HandleSceneInitComplete;
            sceneManager.OnSceneInitComplete += HandleSceneInitComplete;
        }

        Debug.Log("[GameManager] Initialized.");
    }

    public void Dispose()
    {
        var input = Managers.Instance.Get<InputManager>();
        if (input != null)
        {
            input.OnStartGameEvent -= StartGame;
        }

        var sceneManager = Managers.Instance.Get<SceneManagerEx>();
        if (sceneManager != null)
        {
            sceneManager.OnSceneInitComplete -= HandleSceneInitComplete;
        }
    }

    public void StartGame()
    {
        if (_state != GameState.MainMenu)
        {
            Debug.LogWarning("[GameManager] Cannot start game from current state: " + _state);
            return;
        }

        _state = GameState.Loading;
        Debug.Log("[GameManager] Game Started. Destination: Space, Transit: Loading");

        var sceneManager = Managers.Instance.Get<SceneManagerEx>();
        if (sceneManager != null)
        {
            sceneManager.LoadSceneWithLoading(SceneId.Space);
        }
    }

    private void HandleSceneInitComplete()
    {
        var sceneManager = Managers.Instance.Get<SceneManagerEx>();
        if (sceneManager == null) return;

        if (sceneManager.NextSceneId == SceneId.Space)
        {
            Debug.Log("[GameManager] Space Scene Entered! Spawning Cats.");

            var catSpawnManager = Managers.Instance.Get<CatSpawnManager>();
            if (catSpawnManager != null)
            {
                _state = GameState.Playing;
                catSpawnManager.SpawnCat();
            }
            else
            {
                Debug.LogError("[GameManager] CatSpawnManager not found.");
            }
        }
    }
}