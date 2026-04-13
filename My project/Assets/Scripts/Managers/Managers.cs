using System;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    public static Managers Instance { get; private set; }

    [Header("Manager")]
    private readonly Dictionary<Type, IManager> _managers = new();

    private bool _initialized;

    private void Awake()
    {
        if( Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Engine
        Register(new RandomManager());
        Register(new InputManager());

        // Game
        Register(new CameraManager());
        Register(new AttributeManager());
        Register(new CatSpawnManager());
        Register(new GameManager());
        Register(new SceneManagerEx());

        InitAll();
    }

    private void Update()
    {
        foreach (var kv in _managers)
        {
            if (kv.Value is IUpdater updatable)
            {
                try
                {
                    updatable.OnUpdate();
                }
                catch (Exception e)
                {
                    Debug.LogError($"[Managers] Update failed: {kv.Key.Name}\n{e}");
                }
            }
        }
    }

    private void LateUpdate()
    {
        foreach (var kv in _managers)
        {
            if (kv.Value is ILateUpdater lateUpdatable)
            {
                try
                {
                    lateUpdatable.OnLateUpdate();
                }
                catch (Exception e)
                {
                    Debug.LogError($"[Managers] LateUpdate failed: {kv.Key.Name}\n{e}");
                }
            }
        }
    }
    public void Register(IManager manager)
    {
        if (manager == null) return;

        var type = manager.GetType();
        if (_managers.ContainsKey(type))
        {
            Debug.LogWarning($"[Managers] Manager already registered: {type.Name}");
            return;
        }

        _managers.Add(type, manager);
    }

    private void InitAll()
    {
        if (_initialized) return;
        _initialized = true;

        foreach (var kv in _managers)
        {
            try
            {
                kv.Value.Init();
            }
            catch (Exception e)
            {
                Debug.LogError($"[Managers] Init failed: {kv.Key.Name}\n{e}");
            }
        }
    }

    public T Get<T>() where T : class, IManager
    {
        if (_managers.TryGetValue(typeof(T), out var mgr))
            return mgr as T;

        Debug.LogError($"[Managers] Manager not found: {typeof(T).Name}");
        return null;
    }
}
