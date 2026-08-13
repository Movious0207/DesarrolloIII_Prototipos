using UnityEngine;

public class MovementoObject : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 3f;
    public Vector2 direccion = Vector2.left; // Se mueven de derecha a izquierda

    [Header("Límite")]
    public float limiteX = -10f; // Coordenada X donde se destruyen si nadie los corta

    void Update()
    {
        // Mueve el objeto de forma continua
        transform.Translate(direccion * velocidad * Time.deltaTime);

        // Si pasa el límite de la pantalla, se destruye para optimizar
        if (transform.position.x < limiteX)
        {
            Destroy(gameObject);
        }
    }
}
