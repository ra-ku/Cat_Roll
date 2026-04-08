using UnityEngine;

public class CatCatcher : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    private GameObject player_Cat;
    private CharacterController _characterController;
    private CameraManager cm; 

    [Header("Settings")]
    public float baseRadius = 1.0f;
    public float radiusGrowthPerCat = 0.1f;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        cm = Managers.Instance.Get<CameraManager>();


        if (_playerController == null)
        {
            Debug.LogError("PlayerController 참조가 없습니다!");
            return;
        }
        player_Cat = _playerController._playerModel;

        UpdateCharacterControllerRadius();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cat"))
        {
            Debug.Log("고양이 잡았다!");
            CatCollectParentsBySphere(other);
            UpdateCharacterControllerRadius();

            float newRadius = baseRadius + (player_Cat.transform.childCount * radiusGrowthPerCat);
            _characterController.radius = newRadius;

            if (cm != null)
            {
                 cm.UpdateCameraSetting(_characterController.radius);
            }
        }
    }

    private void OnDrawGizmos() 
    {
        if (_characterController == null)
            _characterController = GetComponent<CharacterController>();

        if (_characterController != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 center = transform.position + _characterController.center;
            Gizmos.DrawWireSphere(center, _characterController.radius);

            Gizmos.color = new Color(1, 0.92f, 0.016f, 0.2f);
            Gizmos.DrawSphere(center, _characterController.radius);
        }
    }

    private void CatCollectParentsBySphere(Collider other)
    {
        other.transform.SetParent(player_Cat.transform);
        int catCount = player_Cat.transform.childCount;

        float spawnRadius = baseRadius + (catCount * radiusGrowthPerCat);
        Vector3 randomDirection = Random.onUnitSphere;

        other.transform.localPosition = randomDirection * spawnRadius;
        other.transform.localRotation = Quaternion.LookRotation(randomDirection);

        if (other.TryGetComponent<Collider>(out var col))
        {
            col.enabled = false;
        }
    }

    private void UpdateCharacterControllerRadius()
    {
        if (_characterController == null || player_Cat == null) return;

        int catCount = player_Cat.transform.childCount;
        float newRadius = baseRadius + (catCount * radiusGrowthPerCat);


        _characterController.radius = newRadius;

        Debug.Log($"새로운 반지름: {newRadius} (고양이 수: {catCount-2})");
    }
}