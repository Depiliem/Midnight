using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup gameOverUI;   // CanvasGroup dari GameOver UI
    [SerializeField] private Button playAgainButton;   // Tombol restart
    [SerializeField] private float fadeDuration = 0.5f;

    private bool isGameOver = false;

    void Start()
    {
        // Pastikan UI tidak aktif di awal
        gameOverUI.alpha = 0;
        gameOverUI.interactable = false;
        gameOverUI.blocksRaycasts = false;

        // Tambahkan listener ke tombol
        playAgainButton.onClick.AddListener(RestartLevel);
    }

    /// <summary>
    /// Dipanggil ketika player mati
    /// </summary>
    public void ShowGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        // Munculkan UI dengan animasi fade
        StartCoroutine(FadeInUI());

        // Pause gameplay
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Coroutine untuk efek fade-in halus
    /// </summary>
    private IEnumerator FadeInUI()
    {
        float t = 0;
        gameOverUI.interactable = true;
        gameOverUI.blocksRaycasts = true;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime; // gunakan unscaledDeltaTime karena game dipause
            gameOverUI.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }

        gameOverUI.alpha = 1;
    }

    /// <summary>
    /// Restart scene ketika tombol Play Again ditekan
    /// </summary>
    private void RestartLevel()
    {
        Time.timeScale = 1f;
        PlayerHealth player = FindObjectOfType<PlayerHealth>();

        if (player != null)
        {
            player.Respawn();
        }

        // Tutup UI GameOver
        gameOverUI.alpha = 0;
        gameOverUI.interactable = false;
        gameOverUI.blocksRaycasts = false;
        isGameOver = false;
    }

}
