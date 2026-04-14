using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 4f;
    public float damage = 10f;
    public float spinSpeed = 180f; // 큐브 회전 연출

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Rotate(Vector3.one * spinSpeed * Time.deltaTime, Space.Self);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // TODO: 플레이어 체력 스크립트에 데미지 전달
            // var hp = other.GetComponent<PlayerHealth>();
            // if (hp != null) hp.TakeDamage(damage);
            Debug.Log($"Player hit for {damage}");
            Destroy(gameObject);
        }
        else if (other.CompareTag("CatBall"))
        {
            // 탄막이 고양이 뭉치에 맞으면 그냥 소멸
            Destroy(gameObject);
        }
    }
}