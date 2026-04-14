using UnityEngine;

public class WeakPoint : MonoBehaviour
{
    public Boss boss;
    public float damagePerHit = 15f;
    public float minImpactSpeed = 3f;  // 이 속도 이상으로 부딪혀야 데미지

    void Reset()
    {
        // 컴포넌트 자동 설정 보조
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("CatBall")) return;
        if (boss == null) return;

        // 기절 페이즈에서만 약점 활성
        if (boss.GetCurrentPhase() != Boss.Phase.Stunned) return;

        Rigidbody rb = other.attachedRigidbody;
        float speed = rb != null ? rb.linearVelocity.magnitude : 0f;
        if (speed < minImpactSpeed) return;

        boss.TakeDamage(damagePerHit);

        // 살짝 튕겨내기
        if (rb != null)
        {
            Vector3 bounce = (other.transform.position - transform.position).normalized;
            rb.linearVelocity = bounce * speed * 0.6f + Vector3.up * 3f;
        }
    }
}