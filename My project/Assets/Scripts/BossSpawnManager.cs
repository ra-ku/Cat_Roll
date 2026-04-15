using UnityEngine;

public class BossSpawnManager : MonoBehaviour, IManager
{
    private GameObject _bossPrefab;

    public void Init()
    {
        if (_bossPrefab == null)
        {
            _bossPrefab = Resources.Load<GameObject>("Prefabs/Boss");
        }
    }

    public void SpawnBoss()
    {
        Debug.Log("보스 스폰!");
    }
}
