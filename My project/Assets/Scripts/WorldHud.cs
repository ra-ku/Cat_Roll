using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;

public class WorldHud : MonoBehaviour
{
    [Header("PlayerWidgets")]
    [SerializeField] private Slider _hpBarWidget;
    [SerializeField] private Text _hpTextWidget;
    [SerializeField] private Image _dashIcon;

    [Header("BossWidgets")]
    [SerializeField] private Slider _bossHpBarWidget;
    [SerializeField] private Text _bossName;


    private void Start()
    {
        SetUI();
    }

    private void Update()
    {
        UpdateUI();
    }

    private void SetUI()
    {
        var attributeManager = Managers.Instance.Get<AttributeManager>();
        if (attributeManager != null)
        {
            _hpBarWidget.maxValue = attributeManager.PlayerHp._maxValue;
            _hpBarWidget.value = attributeManager.PlayerHp._value;
            _hpTextWidget.text = $"{attributeManager.PlayerHp._value}/{attributeManager.PlayerHp._maxValue}";

            if (_bossHpBarWidget == null) return; 
            _bossHpBarWidget.maxValue = attributeManager.BossHp._maxValue;
            _bossHpBarWidget.value = attributeManager.BossHp._value;
            //_bossName.text = "보스";
        }
    }

    private void UpdateUI()
    {
        var attributeManager = Managers.Instance.Get<AttributeManager>();
        if (attributeManager != null)
        {
            _hpBarWidget.value = attributeManager.PlayerHp._value;
            _hpTextWidget.text = $"{attributeManager.PlayerHp._value}/{attributeManager.PlayerHp._maxValue}";
        }
    }
}
