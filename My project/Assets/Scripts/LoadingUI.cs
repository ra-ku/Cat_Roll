using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 비동기 제어를 위해 추가

public class LoadingUI : MonoBehaviour
{
    [Header("Widgets")]
    [SerializeField] private Image loadingIcon;
    [SerializeField] private Text loadingValue;

    [Header("Animation Settings")]
    [SerializeField] private float fadeSpeed = 3f;

    [Tooltip("아무리 로딩이 빨라도 최소한 이 시간(초) 동안은 로딩 화면을 보여줍니다.")]
    [SerializeField] private float minLoadingTime = 2.0f;

    void Start()
    {
        SetUI();

        // 코루틴을 사용하여 최소 대기 시간 및 로딩 진행률을 제어합니다.
        StartCoroutine(CoLoadSceneProcess());
    }

    void Update()
    {
        if (loadingIcon != null)
        {
            float alpha = Mathf.Abs(Mathf.Sin(Time.time * fadeSpeed));
            Color color = loadingIcon.color;
            color.a = alpha;
            loadingIcon.color = color;
        }
    }

    public void SetUI()
    {
        if (loadingIcon != null) loadingIcon.fillAmount = 0f;
        if (loadingValue != null) loadingValue.text = "0%";
    }

    public void UpdateProgress(float progress)
    {
        if (loadingIcon != null) loadingIcon.fillAmount = progress;
        if (loadingValue != null)
        {
            int percent = Mathf.RoundToInt(progress * 100f);
            loadingValue.text = $"{percent}%";
        }
    }

    private IEnumerator CoLoadSceneProcess()
    {
        var sceneManager = Managers.Instance.Get<SceneManagerEx>();
        if (sceneManager == null) yield break;

        SceneId targetScene = sceneManager.NextSceneId;
        Debug.Log($"[LoadingUI] {targetScene} 씬 로드 시작! (최소 {minLoadingTime}초 보장)");

        AsyncOperation op = SceneManager.LoadSceneAsync(targetScene.ToString());

        op.allowSceneActivation = false;

        float timer = 0f;

        while (timer < minLoadingTime || op.progress < 0.9f)
        {
            timer += Time.deltaTime;

            float realProgress = Mathf.Clamp01(op.progress / 0.9f);

            float fakeProgress = Mathf.Clamp01(timer / minLoadingTime);

            float displayProgress = Mathf.Min(realProgress, fakeProgress);

            UpdateProgress(displayProgress);

            yield return null;
        }       

        UpdateProgress(1f); 

        yield return new WaitForSeconds(0.1f);

        op.allowSceneActivation = true;
    }
}