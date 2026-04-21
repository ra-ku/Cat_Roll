using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public enum Phase { Shooting, Stunned }

    [Header("References")]
    public Transform player;
    public Transform firePoint;
    public GameObject projectilePrefab;
    public GameObject warningDecalPrefab; // 새로 추가 - 바닥 빨간 원
    public Transform modelRoot;

    [Header("Stats")]
    public float maxHP = 100f;
    public float currentHP;

    [Header("Phase 1 - Shooting")]
    public float shootingDuration = 8f;
    public float patternInterval = 2f;   // 패턴 사이 쉬는 시간
    public float projectileSpeed = 5f;   // 느리게!

    [Header("Pattern A - AOE")]
    public int aoeCount = 10;
    public float aoeWarningTime = 1.5f;
    public float aoeRadius = 2f;

    [Header("Pattern B - Circle Wave")]
    public int circleBulletCount = 8;

    [Header("Phase 3 - Stunned")]
    public float stunnedDuration = 4f;
    public float flipAngle = 180f;

    private Phase currentPhase;
    private bool isDead = false;

    void Start()
    {
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
            yield return StartCoroutine(Phase3_Stunned());
        }
    }

    // ---------- PHASE 1: 탄막 ----------
    IEnumerator Phase1_Shooting()
    {
        currentPhase = Phase.Shooting;
        float timer = 0f;
        bool togglePattern = false;

        while (timer < shootingDuration)
        {
            FaceTarget();

            // 패턴 번갈아가며 사용
            if (togglePattern)
                yield return StartCoroutine(PatternA_AOE());
            else
                yield return StartCoroutine(PatternB_Circle());
            
            togglePattern = !togglePattern;
            
            yield return new WaitForSeconds(patternInterval);
            timer += patternInterval + aoeWarningTime;
        }
    }

    void FaceTarget()
    {
        if (player == null) return;
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    // 패턴 A: 장판 3개
    IEnumerator PatternA_AOE()
    {
        if (player == null) yield break;

        // 플레이어 주변 3곳에 경고 표시
        Vector3[] positions = new Vector3[aoeCount];
        for (int i = 0; i < aoeCount; i++)
        {
            float angle = (360f / aoeCount) * i;
            Vector3 offset = Quaternion.Euler(0, angle, 0) * Vector3.forward * 2.5f;
            positions[i] = player.position + offset;
            positions[i].y = 0.1f; // 바닥에 붙임

            if (warningDecalPrefab != null)
                Instantiate(warningDecalPrefab, positions[i], Quaternion.identity, null);
        }

        yield return new WaitForSeconds(aoeWarningTime);

        // 큐브 낙하 (위에서 떨어지는 연출)
        for (int i = 0; i < aoeCount; i++)
        {
            Vector3 spawnPos = positions[i] + Vector3.up * 10f;
            GameObject p = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            Rigidbody prb = p.GetComponent<Rigidbody>();
            if (prb != null) prb.linearVelocity = Vector3.down * 15f;
            Destroy(p, 3f);
        }
    }

    // 패턴 B: 원형 웨이브
    IEnumerator PatternB_Circle()
    {
        if (firePoint == null) yield break;

        for (int i = 0; i < circleBulletCount; i++)
        {
            float angle = (360f / circleBulletCount) * i;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            
            GameObject p = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(dir));
            Rigidbody prb = p.GetComponent<Rigidbody>();
            if (prb != null) prb.linearVelocity = dir * projectileSpeed;
            Destroy(p, 5f);
        }
        yield return null;
    }

    // ---------- PHASE 3: 기절 (기존 그대로) ----------
    IEnumerator Phase3_Stunned()
    {
        currentPhase = Phase.Stunned;
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

        yield return new WaitForSeconds(stunnedDuration);

        t = 0f;
        while (t < flipDuration)
        {
            modelRoot.localRotation = Quaternion.Slerp(endRot, startRot, t / flipDuration);
            t += Time.deltaTime;
            yield return null;
        }
        modelRoot.localRotation = startRot;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        if (currentPhase != Phase.Stunned) return;

        currentHP -= damage;
        Debug.Log($"Boss HP: {currentHP}/{maxHP}");

        if (currentHP <= 0)
        {
            isDead = true;
            Destroy(gameObject, 1f);
        }
    }

    public Phase GetCurrentPhase() => currentPhase;
}