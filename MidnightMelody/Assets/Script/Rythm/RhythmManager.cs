using UnityEngine;
using TMPro;
using System.Collections;

public class RhythmManager : MonoBehaviour
{
    public float bpm = 120f;

    [Header("Sync Settings")]
    [Tooltip("Posisi Y tengah dari HitZone Anda (misal: -4.0)")]
    public float hitZoneY = -4f;
    [Tooltip("Posisi Y dari Posisi Awal Note Anda (misal: 7.0)")]
    public float spawnY = 7f;
    [Tooltip("Berapa beat yang dibutuhkan Note untuk mencapai HitZone (misal: 1 atau 2)")]
    public float dropTimeInBeats = 1f;

    [Header("Countdown Settings")]
    public TextMeshProUGUI countdownText;
    public GameObject countdownBackground; // Untuk background hitam pudar
    public float countdownDuration = 3f;

    [Header("Countdown Animation")]
    public float animationDuration = 0.4f; // Durasi animasi geser (detik)
    public float startXOffset = -500f;    // Posisi X awal di luar layar (kiri)
    private RectTransform countdownRectTransform;

    private float beatInterval;
    private float calculatedFallSpeed;

    void Start()
    {
        // 1. Hitung Kecepatan dan Interval Beat
        beatInterval = 60f / bpm;
        float totalDropTime = beatInterval * dropTimeInBeats;
        float distance = spawnY - hitZoneY;
        calculatedFallSpeed = distance / totalDropTime;

        // 2. Dapatkan RectTransform
        if (countdownText != null)
        {
            countdownRectTransform = countdownText.GetComponent<RectTransform>();
        }

        // 3. Mulai Coroutine Hitungan Mundur
        StartCoroutine(StartCountdownRoutine());
    }

    private IEnumerator StartCountdownRoutine()
    {
        // 1. Aktifkan teks dan background
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
        }
        if (countdownBackground != null)
        {
            countdownBackground.SetActive(true);
        }

        // --- Logika Hitungan Mundur (Teks & Animasi) ---
        yield return StartCoroutine(AnimateCountdownText("THREE", 1f));
        yield return StartCoroutine(AnimateCountdownText("TWO", 1f));
        yield return StartCoroutine(AnimateCountdownText("ONE", 1f));

        // --- Logika GO! ---
        if (countdownText != null)
        {
            yield return StartCoroutine(AnimateCountdownText("GO!", 0.5f));
        }

        // Tunggu sebentar setelah "GO!"
        yield return new WaitForSeconds(0.2f);

        // 2. Nonaktifkan teks dan background
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
        if (countdownBackground != null)
        {
            countdownBackground.SetActive(false);
        }

        // 3. Panggil fungsi untuk memulai Note
        PrepareAllNotesInScene();
    }

    private IEnumerator AnimateCountdownText(string textToDisplay, float waitTime)
    {
        if (countdownText == null || countdownRectTransform == null)
        {
            yield return new WaitForSeconds(waitTime);
            yield break;
        }

        // 1. Atur Teks Baru dan Posisi Awal (Di luar kiri)
        countdownText.text = textToDisplay;
        Vector3 startPosition = new Vector3(startXOffset, 0f, 0f);
        Vector3 endPosition = Vector3.zero; // Posisi Tengah (0, 0, 0)
        countdownRectTransform.anchoredPosition3D = startPosition;

        // 2. Animasi Geser dari Kiri ke Tengah
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / animationDuration);

            // Menggunakan Lerp dengan smoothing (Ease-Out)
            float smoothT = 1f - Mathf.Pow(1f - t, 3f);

            countdownRectTransform.anchoredPosition3D = Vector3.Lerp(startPosition, endPosition, smoothT);

            yield return null;
        }

        countdownRectTransform.anchoredPosition3D = endPosition;

        // 3. Tahan Teks di Tengah
        yield return new WaitForSeconds(waitTime - animationDuration);
    }

    void PrepareAllNotesInScene()
    {
        NoteObject[] allNotesInScene = FindObjectsOfType<NoteObject>();

        foreach (NoteObject note in allNotesInScene)
        {
            note.fallSpeed = calculatedFallSpeed;
            note.PrepareToStart();
        }
    }

    public float GetBeatInterval()
    {
        return beatInterval;
    }
}