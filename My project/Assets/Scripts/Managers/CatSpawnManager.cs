using UnityEngine;

public class CatSpawnManager : IManager
{
    /// <summary>
    /// 1. 스폰
    /// 2. 최대 스폰 수
    /// 3. 스폰 위치 범위
    /// 
    /// 위 3개를 담당하는 클래스
    /// </summary>
    /// 

    [Header("Spawn Settings")]
    private int _spawnCount = 0;
    private int _maxSpawnCount = 30;

    [Header("Spawn Area")]
    private float _spawnX_value = 10.0f;
    private float _spawnZ_value = 10.0f;
    private float _spawnY_value = 0f;

    [Header("Reference")]
    private RandomManager _randManager;
    private GameObject[] _catPrefabs;

    public void Init()
    {
        _randManager = Managers.Instance.Get<RandomManager>();
        _catPrefabs  = Resources.LoadAll<GameObject>("Prefabs/Cats");

        Debug.Log("CatSpawnManager Initialized");
    }

    public void SpawnCat()
    {
        if(_catPrefabs.Length == 0)
        {
            Debug.LogError("No cat prefabs found in Resources/Prefabs/Cats");
            return;
        }

        var obj = _randManager.RandomInCollection(_catPrefabs , _maxSpawnCount, true);

        for (int i = 0; i < _maxSpawnCount; i++)
        {
            float x = _randManager.RandomRange(-_spawnX_value, _spawnX_value);
            float y = _randManager.RandomRange(-_spawnY_value, _spawnY_value);
            float z = _randManager.RandomRange(-_spawnZ_value, _spawnZ_value);

            Vector3 pos = new Vector3(x, y, z);

            GameObject.Instantiate(obj[i], pos, Quaternion.identity);
            _spawnCount++;
        }
    }
}
