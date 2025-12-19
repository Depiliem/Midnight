using UnityEngine;
using TMPro; // Pastikan menggunakan namespace ini untuk TextMeshPro

public class ScoreManager : MonoBehaviour
{
    public int scorePerNote = 10;

    public TextMeshProUGUI scoreText;

    private int currentScore;

    void Start()
    {
        currentScore = 0;
        UpdateScoreDisplay();
    }

    // Fungsi ini dipanggil dari LaneController saat Note berhasil ditekan
    public void AddScore()
    {
        currentScore += scorePerNote;
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore;
        }
    }
    public int GetCurrentScore()
    {
        // Return variabel skor yang Anda gunakan (misal: currentScore)
        return currentScore;
    }
}