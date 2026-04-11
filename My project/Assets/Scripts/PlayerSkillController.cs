using System.Collections;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Transform _playerModel;          

    [Header("Skill Settings - Dash")]
    [SerializeField] private float _dashCooldown = 2f;
    [SerializeField] private float _dashAmount = 20f;         

    [Header("Visual Effects - Dash Spin")]
    [SerializeField] private float _dashSpinDuration = 0.3f; 
    [SerializeField] private Vector3 _dashSpinAxis = Vector3.right; 

    private Skill _Dash;
    private float _nextDashTime = 0f;

    private void Awake()
    {
        if (_playerController == null) _playerController = GetComponent<PlayerController>();
        if(_playerModel ==null) _playerModel = _playerController._playerModel.transform;

        _Dash = new Skill("Dash", _dashCooldown, _dashAmount);
    }

    private void Start()
    {
        var input = Managers.Instance.Get<InputManager>();
        if (input != null)
            input.OnDashEvent += HandleDashInput;
    }

    private void OnDisable()
    {
        if (Managers.Instance != null)
        {
            var input = Managers.Instance.Get<InputManager>();
            if (input != null) input.OnDashEvent -= HandleDashInput;
        }
    }

    private void HandleDashInput()
    {
        if (Time.time >= _nextDashTime)
        {
            UseSkill(_Dash);
            _nextDashTime = Time.time + _dashCooldown;
        }
    }

    public void UseSkill(Skill skill)
    {
        switch (skill.Name)
        {
            case "Dash":
                _playerController.AddImpact(transform.forward, _Dash.Amount);

                StartCoroutine(DashVisualEffectRoutine());
                break;
        }
    }

    private IEnumerator DashVisualEffectRoutine()
    {
        if (_playerController == null || _playerModel == null) yield break;
        _playerController.isModelSpinningEnabled = false;

        Quaternion initialRotation = _playerModel.localRotation;

        float elapsedTime = 0f;

        while (elapsedTime < _dashSpinDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / _dashSpinDuration;

            float angle = Mathf.Lerp(0f, 720f, t);

            _playerModel.localRotation = initialRotation * Quaternion.AngleAxis(angle, _dashSpinAxis);

            yield return null;
        }
        
        _playerModel.localRotation = initialRotation;
        _playerController.isModelSpinningEnabled = true;
    }
}