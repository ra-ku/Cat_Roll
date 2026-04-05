using UnityEngine;

public class CatPusher : MonoBehaviour
{
    public Transform targetBall; // 굴러가는 공
    public float followDistance = 1.0f; // 공과 고양이 사이의 거리
    public Transform mainCameraTransform;

    void LateUpdate()
    {
        if (targetBall == null || mainCameraTransform == null) return;

        var inputManager = Managers.Instance.Get<InputManager>();
        Vector2 input = inputManager != null ? inputManager.MoveInput : Vector2.zero;

        Vector3 camForward = mainCameraTransform.forward;
        Vector3 camRight = mainCameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;

        Vector3 pushDirection = (camForward * input.y + camRight * input.x).normalized;

        if (pushDirection.sqrMagnitude > 0.01f)
        {
            Vector3 targetPosition = targetBall.position - (pushDirection * followDistance);
            targetPosition.y = targetBall.position.y - (targetBall.localScale.y * 0.5f);

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);

            transform.LookAt(new Vector3(targetBall.position.x, transform.position.y, targetBall.position.z));
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, transform.position, Time.deltaTime);
            transform.LookAt(new Vector3(targetBall.position.x, transform.position.y, targetBall.position.z));
        }
    }
}