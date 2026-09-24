using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject gameOverPanel;
    public GameObject winPanel;

    private bool isGameOver = false;

    void Start()
    {
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void GameOver()
    {
        if(isGameOver) return;
        isGameOver = true;
        if (gameOverPanel) gameOverPanel.SetActive(true);
        EndGameState();
    }

    public void GameWin()
    {
        if (isGameOver) return;
        isGameOver = true;
        if (winPanel) winPanel.SetActive(true);
        EndGameState();
    }

    private void EndGameState()
    {
        Time.timeScale = 0f; // Pause the game

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Resume the game
        SceneManager.LoadScene("MainMenu");
    }

}
