using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private PlayerHealth playerHealth;

    void Start()
    {
        gameOverUI.SetActive(false);
        // Pastikan cursor disembunyikan saat bermain
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ShowGameOver()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0f;

        // Aktifkan kembali cursor supaya bisa klik
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PlayAgain()
    {
        // Pulihkan waktu
        Time.timeScale = 1f;

        // Sembunyikan UI Game Over
        gameOverUI.SetActive(false);

        // Respawn player
        playerHealth.Respawn();

        // Sembunyikan cursor lagi
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void GoToMainMenu()
    {
        // Pastikan waktu normal lagi sebelum ganti scene
        Time.timeScale = 1f;

        // Tampilkan cursor di menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("MainMenu");
    }
}
