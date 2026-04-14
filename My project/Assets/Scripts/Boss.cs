using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public enum Phase { Shooting, Charging, Stunned }

    [Header("References")]
    public Transform player;              // 고양이(플레이어) 트랜스폼
    public Transform firePoint;           // 탄막 발사 위치
    public GameObject projectilePrefab;   // 큐브 탄막 프리팹
    public Transform modelRoot;           // Model 자식 (뒤집기용)

    [Header("Stats")]
    public float maxHP = 100f;
    public float currentHP;

    [Header("Phase 1 - Shooting")]
    public float shootingDuration = 6f;
    public float fireInterval = 0.6f;
    public float projectileSpeed = 8f;
    public int burstCount = 3;            // 한 번에 몇 발 퍼뜨릴지
    public float burstSpread = 15f;       // 퍼짐 각도

    [Header("Phase 2 - Charging")]
    public int chargeCount = 3;
    public float chargeSpeed = 12f;
    public float chargePrepareTime = 0.8f;
    public float chargeCooldown = 0.5f;

    [Header("Phase 3 - Stunned")]
    public float stunnedDuration = 3f;
    public float flipAngle = 180f;

    private Phase currentPhase;
    private Rigidbody rb;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHP = maxHP;
        if (modelRoot == null) modelRoot = transform;
        StartCoroutine(BossLoop());
    }

    IEnumerator BossLoop()
    {
        while (!isDead)
        {
            yield return StartCoroutine(Phase1_Shooting());
            if (isDead) break;
            yield return StartCoroutine(Phase2_Charging());
            if (isDead) break;
            yield return StartCoroutine(Phase3_Stunned());
        }
    }

    // ---------- PHASE 1: 탄막 ----------
    IEnumerator Phase1_Shooting()
    {
        currentPhase = Phase.Shooting;
        Debug.Log("Phase 1: Shooting");

        float timer = 0f;
        while (timer < shootingDuration)
        {
            // 플레이어 방향 바라보기 (Y축만)
            if (player != null)
            {
                Vector3 dir = player.position - transform.position;
                dir.y = 0;
                if (dir.sqrMagnitude > 0.01f)
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        Quaternion.LookRotation(dir),
                        Time.deltaTime * 3f);
            }

            FireBurst();
            yield return new WaitForSeconds(fireInterval);
            timer += fireInterval;
        }
    }

    void FireBurst()
    {
        if (projectilePrefab == null || firePoint == null || player == null) return;

        Vector3 toPlayer = (player.position - firePoint.position).normalized;

        for (int i = 0; i < burstCount; i++)
        {
            float angle = 0f;
            if (burstCount > 1)
                angle = Mathf.Lerp(-burstSpread, burstSpread, (float)i / (burstCount - 1));

            Quaternion rot = Quaternion.Euler(0, angle, 0);
            Vector3 dir = rot * toPlayer;

            GameObject p = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(dir));
            Rigidbody prb = p.GetComponent<Rigidbody>();
            if (prb != null) prb.linearVelocity = dir * projectileSpeed;
        }
    }

    // ---------- PHASE 2: 돌진 ----------
    IEnumerator Phase2_Charging()
    {
        currentPhase = Phase.Charging;
        Debug.Log("Phase 2: Charging");

        for (int i = 0; i < chargeCount; i++)
        {
            if (player == null) yield break;

            // 돌진 준비 (플레이어 위치 락온 + 살짝 흔들림)
            Vector3 targetPos = player.position;
            targetPos.y = transform.position.y;
            Vector3 chargeDir = (targetPos - transform.position).normalized;

            // 바라보기
            transform.rotation = Quaternion.LookRotation(chargeDir);

            // 준비 모션 (살짝 뒤로)
            float prepT = 0f;
            Vector3 startPos = transform.position;
            while (prepT < chargePrepareTime)
            {
                float shake = Mathf.Sin(prepT * 40f) * 0.1f;
                transform.position = startPos + new Vector3(shake, 0, 0);
                prepT += Time.deltaTime;
                yield return null;
            }
            transform.position = startPos;

            // 돌진 — 몸 굴리는 연출 포함
            float chargeDistance = Vector3.Distance(transform.position, targetPos) + 3f;
            float traveled = 0f;
            while (traveled < chargeDistance)
            {
                float step = chargeSpeed * Time.deltaTime;
                transform.position += chargeDir * step;
                modelRoot.Rotate(Vector3.right, 720f * Time.deltaTime, Space.Self); // 굴리기
                traveled += step;
                yield return null;
            }

            yield return new WaitForSeconds(chargeCooldown);
        }
    }

    // ---------- PHASE 3: 기절 (약점 노출) ----------
    IEnumerator Phase3_Stunned()
    {
        currentPhase = Phase.Stunned;
        Debug.Log("Phase 3: Stunned - Weakpoint exposed!");

        // 뒤집기 (Z축 180도 Lerp)
        Quaternion startRot = modelRoot.localRotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, 0, flipAngle);
        float t = 0f;
        float flipDuration = 0.5f;
        while (t < flipDuration)
        {
            modelRoot.localRotation = Quaternion.Slerp(startRot, endRot, t / flipDuration);
            t += Time.deltaTime;
            yield return null;
        }
        modelRoot.localRotation = endRot;

        // 약점 노출 상태로 대기
        yield return new WaitForSeconds(stunnedDuration);

        // 다시 뒤집어서 일어나기
        t = 0f;
        while (t < flipDuration)
        {
            modelRoot.localRotation = Quaternion.Slerp(endRot, startRot, t / flipDuration);
            t += Time.deltaTime;
            yield return null;
        }
        modelRoot.localRotation = startRot;
    }

    // ---------- 데미지 ----------
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        // 기절 상태에서만 데미지 허용
        if (currentPhase != Phase.Stunned) return;

        currentHP -= damage;
        Debug.Log($"Boss HP: {currentHP}/{maxHP}");

        if (currentHP <= 0)
        {
            isDead = true;
            Debug.Log("Boss Defeated!");
            // TODO: 사망 연출
            Destroy(gameObject, 1f);
        }
    }

    public Phase GetCurrentPhase() => currentPhase;
}