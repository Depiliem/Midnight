using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public void PlayAgain()
    {
        // 🔹 Ganti "Level1" sesuai nama scene gameplay kamu
        SceneManager.LoadScene("Level1");
    }

    public void QuitGame()
    {
        // Opsional: keluar dari game
        Debug.Log("Quit game");
        Application.Quit();
    }
}
