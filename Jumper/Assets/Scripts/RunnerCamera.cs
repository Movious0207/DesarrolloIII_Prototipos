using UnityEngine;

public class RunnerCamera : MonoBehaviour
{
    public Transform target;

    public Vector3 offset = new Vector3(0f, 5f, -8f);

    public float smoothSpeed = 8f;

    void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 targetPosition = target.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.LookAt(target.position + Vector3.up);
    }
}