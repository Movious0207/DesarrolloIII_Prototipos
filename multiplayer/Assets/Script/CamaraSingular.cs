using UnityEngine;

public class SeguimientoCamara : MonoBehaviour
{
    [Header("Objetivo a seguir")]
    public Transform objetivo; // Arrastra al jugador asignado aquí

    [Header("Ajustes")]
    public float suavizado = 5f;
    public Vector3 desfase = new Vector3(0, 0, -10); // Mantiene la cámara atrás en Z

    void LateUpdate()
    {
        if (objetivo == null) return;

        // Calcula la posición de destino
        Vector3 posicionObjetivo = objetivo.position + desfase;

        // Mueve la cámara de forma fluida hacia el jugador
        transform.position = Vector3.Lerp(transform.position, posicionObjetivo, suavizado * Time.deltaTime);
    }
}
