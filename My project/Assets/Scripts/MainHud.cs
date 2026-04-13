using UnityEngine;
using UnityEngine.UI;

public class MainHud : MonoBehaviour
{
    [Header("Widgets")]
    [SerializeField] private Text _startGameText;

    [Header("Animation Settings")]
    [SerializeField] private float _moveDistance = 30f; 
    [SerializeField] private float _moveSpeed = 2f;  

    private Vector2 _startPosition; 
    void Start()
    {
        SetUI();

        if (_startGameText != null)
        {
            _startPosition = _startGameText.rectTransform.anchoredPosition;
        }
    }

    void Update()
    { 
        if (_startGameText == null) return;

        float newX = _startPosition.x + Mathf.Sin(Time.time * _moveSpeed) * _moveDistance;

        _startGameText.rectTransform.anchoredPosition = new Vector2(newX, _startPosition.y);
    }

    public void SetUI()
    {
        if (_startGameText == null)
        {
            Debug.LogError("[MainHud] Start Game Text is not assigned.");
            return;
        }
        _startGameText.text = "Press Space to Start";
        _startGameText.fontSize = 40;
    }
}