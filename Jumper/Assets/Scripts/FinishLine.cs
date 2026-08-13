using UnityEngine;

public class Finish : MonoBehaviour
{
    private bool playerFinished = false;

    void OnTriggerEnter(Collider other)
    {
        if (playerFinished)
            return;

        if (other.CompareTag("Player"))
        {
            playerFinished = true;

            Debug.Log("¡GANASTE!");

            GameManager gameManager =
                FindFirstObjectByType<GameManager>();

            if (gameManager != null)
            {
                gameManager.WinGame();
            }
        }
    }
}