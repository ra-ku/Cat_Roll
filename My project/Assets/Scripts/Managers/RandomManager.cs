using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class RandomManager : IManager
{
    public void Init()
    {
        _random = new System.Random();
        Debug.Log("RandomManager is Initialized");
    }

    // ─────────────────────────────────────────────
    // Fields
    // ─────────────────────────────────────────────
    private System.Random _random;
    private int _seed;

    // ─────────────────────────────────────────────
    // Seed
    // ─────────────────────────────────────────────
    public int GetCurrentSeed() => _seed;

    /// <summary>
    /// 시드를 세팅하면 이후 랜덤 결과가 재현 가능해짐.
    /// </summary>
    public void SetCurrentSeed(int seed)
    {
        _seed = seed;
        _random = new System.Random(_seed);
    }

    // ─────────────────────────────────────────────
    // RandomRange
    // ─────────────────────────────────────────────

    /// <summary>
    /// [min, max) : max 미포함 (System.Random.Next 규칙)
    /// </summary>
    public int RandomRange(int min, int max)
    {
        EnsureRandom();
        return _random.Next(min, max);
    }

    /// <summary>
    /// [min, max) : max 미포함 (double 기반)
    /// </summary>
    public long RandomRange(long min, long max)
    {
        EnsureRandom();
        if (max <= min) return min;

        double t = _random.NextDouble(); // [0,1)
        return (long)(t * (max - min) + min);
    }

    /// <summary>
    /// [min, max) : max 미포함 (double 기반)
    /// </summary>
    public float RandomRange(float min, float max)
    {
        EnsureRandom();
        if (max <= min) return min;

        double t = _random.NextDouble(); // [0,1)
        return (float)(t * (max - min) + min);
    }

    /// <summary>
    /// [min, max) : max 미포함
    /// </summary>
    public double RandomRange(double min, double max)
    {
        EnsureRandom();
        if (max <= min) return min;

        double t = _random.NextDouble(); // [0,1)
        return t * (max - min) + min;
    }

    // ─────────────────────────────────────────────
    // Random pick
    // ─────────────────────────────────────────────

    /// <summary>
    /// 컬렉션에서 랜덤 1개 뽑기.
    /// ICollection은 인덱스 접근이 없어 Enumerator로 이동한다.
    /// </summary>
    public T RandomInCollection<T>(ICollection<T> collection)
    {
        EnsureRandom();

        if (collection == null || collection.Count == 0)
        {
            Debug.LogError($"[RandomManager.RandomInCollection<{typeof(T).Name}>] collection is null or empty.");
            return default;
        }

        int index = _random.Next(0, collection.Count);

        using IEnumerator<T> enumerator = collection.GetEnumerator();
        for (int i = 0; i <= index; i++)
            enumerator.MoveNext();

        return enumerator.Current;
    }

    /// <summary>
    /// 컬렉션에서 랜덤 여러 개 뽑기.
    /// allowDuplicate=true면 중복 허용.
    /// allowDuplicate=false면 중복 없이 amount개(불가능하면 default 반환).
    /// </summary>
    public T[] RandomInCollection<T>(ICollection<T> collection, int amount, bool allowDuplicate)
    {
        EnsureRandom();

        if (collection == null || collection.Count == 0)
        {
            Debug.LogError($"[RandomManager.RandomInCollection<{typeof(T).Name}>] collection is null or empty.");
            return default;
        }

        if (amount <= 0) return Array.Empty<T>();

        if (amount == 1)
            return new[] { RandomInCollection(collection) };

        return allowDuplicate
            ? RandomAmountWithDuplicate(collection, amount)
            : RandomAmountWithoutDuplicate(collection, amount);
    }

    private T[] RandomAmountWithDuplicate<T>(ICollection<T> collection, int amount)
    {
        var result = new T[amount];
        for (int i = 0; i < amount; i++)
            result[i] = RandomInCollection(collection);

        return result;
    }

    /// <summary>
    /// 중복 없이 amount개 뽑기:
    /// 리스트로 복사 후 Fisher–Yates 셔플 → 앞에서 amount개 취함 (편향 없음)
    /// </summary>
    private T[] RandomAmountWithoutDuplicate<T>(ICollection<T> collection, int amount)
    {
        if (amount > collection.Count)
        {
            Debug.LogWarning($"[RandomManager.RandomAmountWithoutDuplicate<{typeof(T).Name}>] amount({amount}) > collection.Count({collection.Count}).");
            return default;
        }

        var tempList = new List<T>(collection);

        // Fisher–Yates shuffle (unbiased)
        for (int i = tempList.Count - 1; i > 0; i--)
        {
            int j = _random.Next(0, i + 1); // ✅ 0..i
            (tempList[i], tempList[j]) = (tempList[j], tempList[i]);
        }

        return tempList.GetRange(0, amount).ToArray();
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────
    private void EnsureRandom()
    {
        if (_random == null)
        {
            _seed = Environment.TickCount;
            _random = new System.Random(_seed);
        }
    }
}