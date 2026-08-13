using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player;

    public float speed = 7f;
    public float catchDistance = 1.2f;

    void Update()
    {
        if (player == null)
            return;

        Vector3 direction = player.position - transform.position;

        direction.y = 0f;

        if (direction.magnitude > 0.1f)
        {
            direction.Normalize();

            transform.position +=
                direction * speed * Time.deltaTime;
        }

        if (Vector3.Distance(transform.position, player.position)
            <= catchDistance)
        {
            Debug.Log("¡ATRAPADO!");
        }
    }
}