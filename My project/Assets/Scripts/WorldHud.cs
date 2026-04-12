using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;

public class WorldHud : MonoBehaviour
{
    [Header("Widgets")]
    [SerializeField] private Slider _hpBarWidget;
    [SerializeField] private Text _hpTextWidget;
    [SerializeField] private Image _dashIcon;


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
