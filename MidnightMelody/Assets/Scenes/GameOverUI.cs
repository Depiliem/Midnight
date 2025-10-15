using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    // Fungsi ini akan dipanggil saat tombol PlayAgain diklik
    public void PlayAgain()
    {
        Debug.Log("Kembali ke Main Menu...");
        SceneManager.LoadScene("MainMenu"); // pastikan nama scene persis
    }

    // (opsional) tombol Quit juga bisa kamu tambahkan
    public void QuitGame()
    {
        Debug.Log("Keluar game...");
        Application.Quit();
    }
}
