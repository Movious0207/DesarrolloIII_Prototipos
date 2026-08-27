using System.Collections.Generic;
using UnityEngine;

public class CamaraMultijugador : MonoBehaviour
{
    [Header("Objetos a Seguir")]
    public List<Transform> objetivos; // Arrastra aquí a tus dos jugadores

    [Header("Configuración de Seguimiento")]
    public Vector3 desfase = new Vector3(0, 0, -10); // Mantiene la cámara hacia atrás en Z
    public float tiempoSuavizado = 0.5f;

    [Header("Configuración de Zoom")]
    public float zoomMinimo = 5f;
    public float zoomMaximo = 15f;
    public float factorZoom = 5f; // Qué tan rápido cambia el zoom al separarse

    private Camera camara;
    private Vector3 velocidadActual;

    void Start()
    {
        camara = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        // Si no hay jugadores asignados o alguno fue destruido, no hace nada
        if (objetivos == null || objetivos.Count == 0 || objetivos[0] == null || objetivos[1] == null)
            return;

        MoverCamara();
        AjustarZoom();
    }

    void MoverCamara()
    {
        // Encuentra el centro exacto entre los dos personajes
        Vector3 puntoMedio = ObtenerPuntoCentro();
        Vector3 posicionObjetivo = puntoMedio + desfase;

        // Mueve la cámara suavemente hacia ese punto central
        transform.position = Vector3.SmoothDamp(transform.position, posicionObjetivo, ref velocidadActual, tiempoSuavizado);
    }

    void AjustarZoom()
    {
        // Calcula la distancia más grande entre los jugadores en la pantalla
        float distanciaMaxima = ObtenerDistanciaMaxima();

        // Calcula el nuevo tamaño de la cámara basándose en la distancia
        float nuevoZoom = Mathf.Lerp(zoomMinimo, zoomMaximo, distanciaMaxima / factorZoom);

        // Aplica el tamaño suavemente a la cámara orthográfica 2D
        camara.orthographicSize = Mathf.Lerp(camara.orthographicSize, nuevoZoom, Time.deltaTime * 5f);
    }

    Vector3 ObtenerPuntoCentro()
    {
        var limites = new Bounds(objetivos[0].position, Vector3.zero);
        for (int i = 0; i < objetivos.Count; i++)
        {
            limites.Encapsulate(objetivos[i].position);
        }
        return limites.center;
    }

    float ObtenerDistanciaMaxima()
    {
        var limites = new Bounds(objetivos[0].position, Vector3.zero);
        for (int i = 0; i < objetivos.Count; i++)
        {
            limites.Encapsulate(objetivos[i].position);
        }
        // Devuelve el ancho o alto del cuadro imaginario que encierra a ambos, el que sea mayor
        return Mathf.Max(limites.size.x, limites.size.y);
    }
}
