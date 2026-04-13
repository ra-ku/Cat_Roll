using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// 씬들을 정의하는 Enum
// 유니티 Build Settings에 등록된 씬 이름과 정확히 일치해야 합니다.
public enum SceneId
{
    MainMenu,
    Loading, // 중간 로딩 씬 추가
    Space,
}

public class SceneManagerEx : IManager
{
    // 씬 로드 및 초기화가 완료되었을 때 호출될 이벤트
    public event Action OnSceneInitComplete;

    // 🔥 로딩 씬(Loading)에서 비동기로 불러올 '최종 목적지 씬'을 기억하는 프로퍼티
    public SceneId NextSceneId { get; private set; }

    public void Init()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("[SceneManagerEx] Initialized.");
    }

    public void Dispose()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadSceneWithLoading(SceneId nextSceneId)
    {
        NextSceneId = nextSceneId;

        SceneManager.LoadScene(SceneId.Loading.ToString());
    }

    public void LoadSceneAsync(SceneId sceneId, Action<float> onProgress = null)
    {
        SceneTaskRunner.RunCoroutine(CoLoadSceneAsync(sceneId, onProgress));
    }

    private IEnumerator CoLoadSceneAsync(SceneId sceneId, Action<float> onProgress)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneId.ToString());

        while (!op.isDone)
        {
            onProgress?.Invoke(op.progress);

            yield return null;
        }
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[SceneManagerEx] 씬 로드 완료: {scene.name}");

        // 불러온 씬의 이름이 Enum(SceneId)에 있는지 확인
        if (Enum.TryParse(scene.name, out SceneId loadedSceneId))
        {
            switch (loadedSceneId)
            {
                case SceneId.MainMenu:
                case SceneId.Space:
                    Debug.Log($"[SceneManagerEx] {scene.name} 씬 전용 데이터/매니저 초기화 완료");
                    OnSceneInitComplete?.Invoke();
                    break;
            }
        }
    }

    public void NotifySceneInitComplete()
    {
        OnSceneInitComplete?.Invoke();
    }
}