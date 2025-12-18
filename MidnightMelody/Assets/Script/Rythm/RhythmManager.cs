using UnityEngine;
using TMPro;
using System.Collections;

public class RhythmManager : MonoBehaviour
{
    [Header("Sync Settings")]
    public float bpm = 120f;
    [Tooltip("Posisi Y HitZone (Tempat Note ditekan)")]
    public float hitZoneY = -4f;
    [Tooltip("Posisi Y awal Note saat muncul")]
    public float spawnY = 7f;
    [Tooltip("Berapa beat yang dibutuhkan Note untuk sampai ke HitZone")]
    public float dropTimeInBeats = 1f;

    [Header("Countdown UI")]
    public TextMeshProUGUI countdownText;
    public GameObject countdownBackground;

    [Header("Countdown Animation")]
    public float animationDuration = 0.4f;
    public float startXOffset = -2000f; // Muncul dari kiri jauh
    private RectTransform countdownRectTransform;
    private float targetYPosition; // Mengambil posisi Y yang Anda atur di Inspector

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip countdownSound; // Suara hitungan (3, 2, 1)
    public AudioClip goSound;        // Suara GO!
    public AudioClip gameMusic;      // Lagu Utama

    [Header("Music Cut & Fade Settings")]
    public float musicStartTime = 45f; // Mulai dari 00:45
    public float musicEndTime = 70f;   // Berhenti di 01:10 (70 detik)
    public float fadeDuration = 2f;    // Durasi halus suara muncul/hilang

    private float beatInterval;
    private float calculatedFallSpeed;

    void Start()
    {
        // 1. Kalkulasi Kecepatan Note
        beatInterval = 60f / bpm;
        float totalDropTime = beatInterval * dropTimeInBeats;
        float distance = spawnY - hitZoneY;
        calculatedFallSpeed = distance / totalDropTime;

        // 2. Setup UI dan Posisi Y
        if (countdownText != null)
        {
            countdownRectTransform = countdownText.GetComponent<RectTransform>();
            if (countdownRectTransform != null)
            {
                // Mengambil posisi Y yang sudah Anda geser di Inspector
                targetYPosition = countdownRectTransform.anchoredPosition3D.y;
            }
        }

        // 3. Mulai Proses Countdown
        StartCoroutine(StartCountdownRoutine());
    }

    private IEnumerator StartCountdownRoutine()
    {
        if (countdownText != null) countdownText.gameObject.SetActive(true);
        if (countdownBackground != null) countdownBackground.SetActive(true);

        // --- Proses Hitungan Mundur ---
        yield return StartCoroutine(AnimateCountdownText("THREE", 1f));
        yield return StartCoroutine(AnimateCountdownText("TWO", 1f));
        yield return StartCoroutine(AnimateCountdownText("ONE", 1f));
        yield return StartCoroutine(AnimateCountdownText("GO!", 0.5f));

        yield return new WaitForSeconds(0.2f);

        // Matikan UI Countdown
        if (countdownText != null) countdownText.gameObject.SetActive(false);
        if (countdownBackground != null) countdownBackground.SetActive(false);

        // --- Mulai Memutar Lagu (Dengan Cut & Fade) ---
        if (audioSource != null && gameMusic != null)
        {
            audioSource.clip = gameMusic;
            audioSource.time = musicStartTime; // Lompat ke 00:45
            audioSource.volume = 0f;           // Mulai dari senyap
            audioSource.Play();

            // Efek Fade In
            StartCoroutine(FadeMusic(0f, 1f, fadeDuration));

            // Jadwalkan Fade Out sebelum lagu berakhir
            float totalPlayTime = musicEndTime - musicStartTime;
            Invoke("TriggerFadeOut", totalPlayTime - fadeDuration);
        }

        // Aktifkan Note untuk mulai jatuh
        PrepareAllNotesInScene();
    }

    private IEnumerator AnimateCountdownText(string textToDisplay, float waitTime)
    {
        if (countdownText == null || countdownRectTransform == null)
        {
            yield return new WaitForSeconds(waitTime);
            yield break;
        }

        // Suara
        if (audioSource != null)
        {
            if (textToDisplay == "GO!") audioSource.PlayOneShot(goSound);
            else audioSource.PlayOneShot(countdownSound);
        }

        // Setup Animasi
        countdownText.text = textToDisplay;
        Vector3 startPos = new Vector3(startXOffset, targetYPosition, 0f);
        Vector3 endPos = new Vector3(0f, targetYPosition, 0f);
        countdownRectTransform.anchoredPosition3D = startPos;

        // Gerakan Slide-In
        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);
            float smoothT = 1f - Mathf.Pow(1f - t, 3f); // Ease Out

            countdownRectTransform.anchoredPosition3D = Vector3.Lerp(startPos, endPos, smoothT);
            yield return null;
        }

        countdownRectTransform.anchoredPosition3D = endPos;
        yield return new WaitForSeconds(waitTime - animationDuration);
    }

    // Fungsi Fade Out
    private void TriggerFadeOut()
    {
        StartCoroutine(FadeMusic(audioSource.volume, 0f, fadeDuration));
        Invoke("StopMusicCompletely", fadeDuration);
    }

    private void StopMusicCompletely()
    {
        audioSource.Stop();
        Debug.Log("Lagu Selesai (Limit 01:10 tercapai)");
    }

    // Coroutine Universal untuk Fade Volume
    private IEnumerator FadeMusic(float startVol, float targetVol, float duration)
    {
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVol, targetVol, timer / duration);
            yield return null;
        }
        audioSource.volume = targetVol;
    }

    void PrepareAllNotesInScene()
    {
        NoteObject[] allNotes = FindObjectsOfType<NoteObject>();
        foreach (NoteObject note in allNotes)
        {
            note.fallSpeed = calculatedFallSpeed;
            note.PrepareToStart();
        }
    }

    public float GetBeatInterval() => beatInterval;
}