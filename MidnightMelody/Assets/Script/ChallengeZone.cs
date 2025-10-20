using System.Collections;
using UnityEngine;
using TMPro;

public class ChallengeZone : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public PlayerHealth playerHealth;
    public Sc_hero playerMovement;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI stateText;
    public CanvasGroup redFlash;

    [Header("Challenge Settings")]
    public int stopAtNoteCount = 1;       // 🔸 Berhenti setelah berapa note diambil
    public TextMeshProUGUI completeText;  // 🔸 Teks "Challenge Complete!" opsional

    [Header("Timing")]
    public float countdown = 3f;
    public float greenDuration = 3f;
    public float redDuration = 3f;

    [Header("Movement Detection")]
    public float moveThreshold = 0.05f;
    public int redDamage = 25;
    public float gracePeriod = 0.3f;      // 🔸 Waktu aman di awal red light

    private bool challengeStarted = false;
    private bool inRed = false;
    private bool challengeActive = false;
    private Vector3 lastPos;

    void Start()
    {
        if (countdownText) countdownText.gameObject.SetActive(false);
        if (stateText) stateText.gameObject.SetActive(false);
        if (redFlash) redFlash.alpha = 0f;
        if (completeText) completeText.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (challengeStarted) return;
        if (other.transform != player) return;

        // Cegah mulai sebelum bicara dengan NPC
        if (!Sc_npc.HasTalkedToNpc)
        {
            Debug.Log("🚫 Player belum bicara dengan NPC!");
            return;
        }

        StartCoroutine(StartChallenge());
        challengeStarted = true;
    }

    IEnumerator StartChallenge()
    {
        // === 1️⃣ Lock player movement ===
        if (playerMovement) playerMovement.enabled = false;

        if (countdownText)
        {
            countdownText.gameObject.SetActive(true);
            float t = countdown;
            while (t > 0f)
            {
                countdownText.text = $"Game starts in {Mathf.CeilToInt(t)}";
                t -= Time.deltaTime;
                yield return null;
            }
            countdownText.gameObject.SetActive(false);
        }

        // === 2️⃣ Unlock movement, mulai looping ===
        if (playerMovement) playerMovement.enabled = true;
        challengeActive = true;
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        Debug.Log("🎮 Challenge started looping!");

        while (challengeActive)
        {
            // === GREEN LIGHT ===
            if (stateText)
            {
                stateText.text = "<color=#00FF5A>GREEN LIGHT</color>";
                stateText.gameObject.SetActive(true);
            }

            lastPos = player.position;
            yield return new WaitForSeconds(greenDuration);

            // === RED LIGHT ===
            if (stateText)
                stateText.text = "<color=#FF4A4A>RED LIGHT</color>";

            inRed = true;
            float timer = redDuration;
            float grace = gracePeriod;
            while (timer > 0f)
            {
                float moved = (player.position - lastPos).magnitude;
                lastPos = player.position;

                // abaikan pergerakan kecil di awal red
                if (grace > 0)
                {
                    grace -= Time.deltaTime;
                }
                else if (moved > moveThreshold)
                {
                    DamageAndFlash();
                }

                timer -= Time.deltaTime;
                yield return null;

                // 🔹 Berhenti kalau note sudah diambil
                if (QuestManager.instance != null && QuestManager.instance.notesCollected >= stopAtNoteCount)
                {
                    EndChallenge();
                    yield break;
                }
            }

            inRed = false;
        }
    }

    void DamageAndFlash()
    {
        if (playerHealth) playerHealth.TakeDamage(redDamage);
        if (redFlash) StartCoroutine(FlashRed());
    }

    IEnumerator FlashRed()
    {
        float a = 0f;
        while (a < 0.35f)
        {
            a += Time.deltaTime * 4f;
            redFlash.alpha = a;
            yield return null;
        }
        while (a > 0f)
        {
            a -= Time.deltaTime * 2f;
            redFlash.alpha = a;
            yield return null;
        }
        redFlash.alpha = 0f;
    }

    void EndChallenge()
    {
        challengeActive = false;
        inRed = false;
        if (stateText) stateText.gameObject.SetActive(false);

        if (completeText)
        {
            completeText.gameObject.SetActive(true);
            completeText.text = "<color=#00FFAA>Challenge Complete!</color>";
        }

        Debug.Log("🏁 Challenge stopped — note collected.");
    }
}
