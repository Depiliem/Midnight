using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class RhythmManager : MonoBehaviour
{
    [Header("Sync Settings")]
    public float bpm = 128f; // Sesuaikan dengan BPM lagu kamu
    public float hitZoneY = -4f;
    public float spawnY = 54.42f; // Sesuai posisi Y Note di Inspector kamu
    public float dropTimeInBeats = 1f;

    [Header("Countdown UI")]
    public TextMeshProUGUI countdownText;
    public GameObject countdownBackground;

    [Header("Win/Loss Settings")]
    public int minScoreToWin = 250;
    public CanvasGroup loseCanvasGroup;
    public float whiteFadeSpeed = 1f;

    [Header("Countdown Animation")]
    public float animationDuration = 0.4f;
    public float startXOffset = -2000f;
    private RectTransform countdownRectTransform;
    private float targetYPosition;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip countdownSound;
    public AudioClip goSound;
    public AudioClip gameMusic;

    [Header("Music Cut & Fade Settings")]
    public float musicStartTime = 45f;
    public float musicEndTime = 70f;
    public float fadeDuration = 2f;

    private float beatInterval;
    private float calculatedFallSpeed;
    private ScoreManager scoreManager;

    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();

        beatInterval = 60f / bpm;
        float totalDropTime = beatInterval * dropTimeInBeats;
        float distance = spawnY - hitZoneY;
        calculatedFallSpeed = distance / totalDropTime;

        if (countdownText != null)
        {
            countdownRectTransform = countdownText.GetComponent<RectTransform>();
            if (countdownRectTransform != null)
            {
                targetYPosition = countdownRectTransform.anchoredPosition3D.y;
            }
        }

        StartCoroutine(StartCountdownRoutine());
    }

    private IEnumerator StartCountdownRoutine()
    {
        if (countdownText != null) countdownText.gameObject.SetActive(true);
        if (countdownBackground != null) countdownBackground.SetActive(true);

        yield return StartCoroutine(AnimateCountdownText("THREE", 1f));
        yield return StartCoroutine(AnimateCountdownText("TWO", 1f));
        yield return StartCoroutine(AnimateCountdownText("ONE", 1f));
        yield return StartCoroutine(AnimateCountdownText("GO!", 0.5f));

        yield return new WaitForSeconds(0.2f);

        if (countdownText != null) countdownText.gameObject.SetActive(false);
        if (countdownBackground != null) countdownBackground.SetActive(false);

        if (audioSource != null && gameMusic != null)
        {
            audioSource.clip = gameMusic;
            audioSource.time = musicStartTime;
            audioSource.volume = 0f;
            audioSource.Play();
            StartCoroutine(FadeMusic(0f, 1f, fadeDuration));

            float totalPlayTime = musicEndTime - musicStartTime;
            Invoke("TriggerFadeOut", totalPlayTime - fadeDuration);
        }

        PrepareAllNotesInScene();
    }

    private IEnumerator AnimateCountdownText(string textToDisplay, float waitTime)
    {
        if (countdownText == null || countdownRectTransform == null)
        {
            yield return new WaitForSeconds(waitTime);
            yield break;
        }

        if (audioSource != null)
        {
            if (textToDisplay == "GO!") audioSource.PlayOneShot(goSound);
            else audioSource.PlayOneShot(countdownSound);
        }

        countdownText.text = textToDisplay;
        Vector3 startPos = new Vector3(startXOffset, targetYPosition, 0f);
        Vector3 endPos = new Vector3(0f, targetYPosition, 0f);
        countdownRectTransform.anchoredPosition3D = startPos;

        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);
            float smoothT = 1f - Mathf.Pow(1f - t, 3f);
            countdownRectTransform.anchoredPosition3D = Vector3.Lerp(startPos, endPos, smoothT);
            yield return null;
        }

        countdownRectTransform.anchoredPosition3D = endPos;
        yield return new WaitForSeconds(waitTime - animationDuration);
    }

    private void TriggerFadeOut()
    {
        StartCoroutine(FadeMusic(audioSource.volume, 0f, fadeDuration));
        Invoke("StopMusicAndCheckScore", fadeDuration);
    }

    private void StopMusicAndCheckScore()
    {
        audioSource.Stop();

        int finalScore = 0;
        if (scoreManager != null)
        {
            finalScore = scoreManager.GetCurrentScore();
        }

        if (finalScore < minScoreToWin)
        {
            StartCoroutine(LoseSequence());
        }
        else
        {
            // --- LOGIKA MENANG ---
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene("Victory");
        }
    }

    private IEnumerator LoseSequence()
    {
        if (loseCanvasGroup == null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            yield break;
        }

        float timer = 0;
        while (timer < 1f)
        {
            timer += Time.deltaTime * whiteFadeSpeed;
            loseCanvasGroup.alpha = timer;
            yield return null;
        }

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

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