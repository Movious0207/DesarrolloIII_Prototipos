using UnityEngine;

public class PlataformaRunner2D : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 8f;

    [Header("Referencias y Ajustes")]
    public Transform otraPlataforma;
    public float puntoLimiteX = -15f;

    private float anchoPlataforma;

    void Start()
    {
        // Calcula automáticamente el ancho de la plataforma en 2D usando el SpriteRenderer
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            anchoPlataforma = sprite.bounds.size.x;
        }
        else
        {
            // Ancho por defecto si no se detecta el SpriteRenderer
            anchoPlataforma = 20f;
        }
    }

    void Update()
    {
        transform.Translate(Vector2.left * velocidad * Time.deltaTime, Space.World);

        if (transform.position.x <= puntoLimiteX)
        {
            Reposicionar();
        }
    }

    void Reposicionar()
    {
        Vector3 nuevaPosicion = transform.position;
        nuevaPosicion.x = anchoPlataforma + 10f;

        transform.position = nuevaPosicion;
    }
}