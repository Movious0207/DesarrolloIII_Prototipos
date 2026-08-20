using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Transform target;

    [Header("Ajuste de Altura")]
    [SerializeField] private float offsetY = 0f; // Modifica este valor para subir o bajar la cámara

    private void LateUpdate()
    {
        if (target == null) return;

        // Mantiene el seguimiento directo en X, y le aplica el offset en Y
        transform.position = new Vector3(target.position.x, 0 + offsetY, transform.position.z);
    }
}
