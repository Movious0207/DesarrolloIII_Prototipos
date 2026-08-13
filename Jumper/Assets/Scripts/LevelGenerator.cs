using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Jugador")]
    public Transform player;

    [Header("Generación")]
    public float chunkLength = 10f;
    public int chunksAhead = 12;

    [Header("Nivel")]
    public float levelLength = 200f;

    [Header("Obstáculos")]
    public GameObject obstaclePrefab;
    public float obstacleChance = 0.55f;

    [Header("Meta")]
    public GameObject finishPrefab;

    private float nextChunkZ = 0f;
    private bool finishedGenerating = false;

    void Start()
    {
        // Generamos los primeros segmentos
        for (int i = 0; i < chunksAhead; i++)
        {
            GenerateChunk();
        }
    }

    void Update()
    {
        if (player == null)
            return;

        // Mientras el jugador avanza,
        // seguimos generando terreno.
        while (nextChunkZ < player.position.z + chunksAhead * chunkLength)
        {
            if (nextChunkZ >= levelLength)
            {
                CreateFinish();
                finishedGenerating = true;
                break;
            }

            GenerateChunk();
        }
    }

    void GenerateChunk()
    {
        // Crear piso
        GameObject floor = GameObject.CreatePrimitive(
            PrimitiveType.Cube
        );

        floor.name = "Floor";

        floor.transform.position = new Vector3(
            0f,
            -0.5f,
            nextChunkZ + chunkLength / 2f
        );

        floor.transform.localScale = new Vector3(
            10f,
            1f,
            chunkLength
        );

        // Posibilidad de crear obstáculo
        if (Random.value < obstacleChance)
        {
            CreateObstacle(nextChunkZ);
        }

        nextChunkZ += chunkLength;
    }

    void CreateObstacle(float z)
    {
        if (obstaclePrefab != null)
        {
            GameObject obstacle = Instantiate(
                obstaclePrefab
            );

            obstacle.transform.position = new Vector3(
                Random.Range(-3f, 3f),
                1f,
                z + chunkLength / 2f
            );
        }
        else
        {
            // Si no tenemos prefab,
            // creamos directamente un cubo.
            GameObject obstacle = GameObject.CreatePrimitive(
                PrimitiveType.Cube
            );

            obstacle.name = "Obstacle";

            obstacle.transform.position = new Vector3(
                Random.Range(-3f, 3f),
                1f,
                z + chunkLength / 2f
            );

            obstacle.transform.localScale = new Vector3(
                2f,
                2f,
                1f
            );
        }
    }

    void CreateFinish()
    {
        GameObject finish = GameObject.CreatePrimitive(
            PrimitiveType.Cube
        );

        finish.name = "FINISH";

        finish.transform.position = new Vector3(
            0f,
            2.5f,
            levelLength
        );

        finish.transform.localScale = new Vector3(
            10f,
            5f,
            1f
        );

        Collider collider = finish.GetComponent<Collider>();
        collider.isTrigger = true;

        finish.AddComponent<Finish>();

        Renderer renderer = finish.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.material.color = Color.green;
        }

        Debug.Log("¡META CREADA!");
    }
}