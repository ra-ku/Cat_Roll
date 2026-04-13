using System;
using System.Collections;
using UnityEngine;

public class SceneTaskRunner : MonoBehaviour
{
    private static SceneTaskRunner _instance;

    private static SceneTaskRunner Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("[SceneTaskRunner]");
                DontDestroyOnLoad(go);
                _instance = go.AddComponent<SceneTaskRunner>();
            }

            return _instance;
        }
    }

    public static void RunNextFrame(Action action)
    {
        if (action == null)
            return;

        Instance.StartCoroutine(Co(action));
    }

    public static void RunNextFrame(MonoBehaviour owner, Action action)
    {
        if (owner == null || action == null)
            return;

        Instance.StartCoroutine(Co(owner, action));
    }

    private static IEnumerator Co(Action action)
    {
        yield return null;
        action.Invoke();
    }

    private static IEnumerator Co(MonoBehaviour owner, Action action)
    {
        yield return null;

        if (owner == null || owner.gameObject == null)
            yield break;

        action.Invoke();
    }

    public static Coroutine RunCoroutine(IEnumerator routine)
    {
        if (routine == null) return null;
        return Instance.StartCoroutine(routine);
    }
}
