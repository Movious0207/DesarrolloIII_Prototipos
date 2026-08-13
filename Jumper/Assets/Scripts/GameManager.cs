using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool gameEnded = false;

    public void WinGame()
    {
        if (gameEnded)
            return;

        gameEnded = true;

        Debug.Log("================================");
        Debug.Log("          ¡GANASTE!");
        Debug.Log("================================");

        Time.timeScale = 0f;
    }

    public void LoseGame()
    {
        if (gameEnded)
            return;

        gameEnded = true;

        Debug.Log("GAME OVER");

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}