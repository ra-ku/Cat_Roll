using UnityEditor.UIElements;
using UnityEngine;

public enum AttributeType
{
    hp,
}

public class Attribute
{
    private AttributeType _type;
    public float _value;
    public float _maxValue;

    public Attribute(AttributeType type, float value, float maxValue)
    {
        _type = type;
        _value = value;
        _maxValue = maxValue;
    }

    public void SetValue(float newValue)
    {
        _value = Mathf.Clamp(newValue, 0, _maxValue);
    }
}

public class AttributeManager : IManager , IUpdater
{
    [Header("Attributes")]
    private Attribute _playerHp;

    public Attribute PlayerHp => _playerHp;

    bool isStart = false;

    public void Init()
    {
        _playerHp = new Attribute(AttributeType.hp, 100f, 100f);

        Debug.Log("AttributeManager initialized.");
    }

    public void OnUpdate()
    {
        DecreaseAttribute(_playerHp, 0.1f);
        Debug.Log($"Player HP: {_playerHp._value}");
    }

    private void DecreaseAttribute(Attribute attribute , float amount)
    {
        if(attribute != null)
        {
            attribute._value -= amount;
            if (attribute._value < 0)
            {
                attribute._value = 0;
            }
        }
    }
}
