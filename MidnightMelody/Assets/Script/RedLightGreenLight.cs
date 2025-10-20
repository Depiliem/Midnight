using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class RedLightGreenLight : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Transform player;                 // drag hero
    [SerializeField] TextMeshProUGUI uiText;           // 1 teks untuk semua notifikasi
    [SerializeField] GameObject uiPanel;               // panel/background UI (optional)

    [Header("Timing (seconds)")]
    [SerializeField] float countdownTime = 3f;         // "Game akan dimulai" -> 3..2..1
    [SerializeField] float greenDuration = 4f;         // durasi GREEN
    [SerializeField] float redDuration = 3f;           // durasi RED

    [Header("Rules")]
    [SerializeField] string playerTag = "Player";
    [SerializeField] string gameOverScene = "GameOver";
    [SerializeField] float moveThreshold = 0.04f;      // toleransi gerak saat RED

    bool started = false;
    bool inZone = false;

    void Reset()
    {
        // otomatis cari player bertag Player
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
    }

    void OnTriggerEnter(Collider other)
    {
        if (started) return;
        if (other.CompareTag(playerTag))
        {
            inZone = true;
            if (!player) player = other.transform;
            StartCoroutine(GameFlow());
            started = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag)) inZone = false;
    }

    IEnumerator GameFlow()
    {
        // UI on
        if (uiPanel) uiPanel.SetActive(true);

        // COUNTDOWN
        float t = countdownTime;
        while (t > 0f)
        {
            if (uiText) uiText.text = $"Game akan dimulai\n{Mathf.CeilToInt(t)}";
            t -= Time.deltaTime;
            yield return null;
        }

        // GREEN LIGHT
        if (uiText) uiText.text = "GREEN LIGHT";
        yield return new WaitForSeconds(greenDuration);

        // RED LIGHT
        if (uiText) uiText.text = "RED LIGHT\nJangan bergerak!";
        Vector3 lastPos = player.position;
        float timer = 0f;

        while (timer < redDuration)
        {
            // cek gerak horizontal saja
            Vector3 now = player.position;
            now.y = 0f;
            Vector3 lp = lastPos; lp.y = 0f;

            if (Vector3.Distance(now, lp) > moveThreshold)
            {
                // mati
                SceneManager.LoadScene(gameOverScene);
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // selesai satu siklus. Ulangi pola GREEN->RED terus selama player masih di zona.
        while (inZone)
        {
            // GREEN
            if (uiText) uiText.text = "GREEN LIGHT";
            yield return new WaitForSeconds(greenDuration);

            // RED
            if (uiText) uiText.text = "RED LIGHT\nJangan bergerak!";
            lastPos = player.position;
            timer = 0f;
            while (timer < redDuration)
            {
                Vector3 now2 = player.position; now2.y = 0f;
                Vector3 lp2 = lastPos; lp2.y = 0f;

                if (Vector3.Distance(now2, lp2) > moveThreshold)
                {
                    SceneManager.LoadScene(gameOverScene);
                    yield break;
                }
                timer += Time.deltaTime;
                yield return null;
            }
        }

        // keluar zona -> matikan UI
        if (uiPanel) uiPanel.SetActive(false);
        if (uiText) uiText.text = "";
        started = false;
    }
}
