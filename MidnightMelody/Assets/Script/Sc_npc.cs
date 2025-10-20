using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class Sc_npc : MonoBehaviour
{
    // === EVENT: dipanggil saat ngobrol pertama kali ===
    public static Action OnTalkStarted;

    // === FLAG global, dicek oleh ChallengeZone ===
    public static bool HasTalkedToNpc = false;

    [Header("Dialogue Settings")]
    public GameObject d_template;   // prefab dialog (background + text)
    public GameObject canva;        // canvas tempat dialog

    [Header("Game Over")]
    public GameObject gameOverCanvas; // 👈 Assign UI Game Over kamu di sini

    private bool player_detection = false;
    // --- 'hasTriggeredNotes' dan 'hasNotifiedTalk' dihapus ---
    // --- Kita akan menggunakan QuestManager.instance.questActive ---

    void Start()
    {
        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(false);
    }

    // --- FUNGSI UPDATE YANG DIPERBARUI DENGAN 3 KONDISI ---
    void Update()
    {
        // Tekan F untuk mulai bicara
        if (player_detection && Input.GetKeyDown(KeyCode.F) && !Sc_hero.dialogue)
        {
            canva.SetActive(true);
            Sc_hero.dialogue = true;

            // Hapus dialog lama jika ada (yang masih clone)
            foreach (Transform child in canva.transform)
            {
                if (child.name.Contains("(Clone)"))
                    Destroy(child.gameObject);
            }

            // Dapatkan skrip dialog
            Sc_Dialogue dialogueScript = canva.GetComponent<Sc_Dialogue>();
            if (dialogueScript == null)
            {
                Debug.LogError("Sc_Dialogue component not found on canvas!");
                Sc_hero.dialogue = false;
                canva.SetActive(false);
                return;
            }

            // Cek jika QuestManager ada
            if (QuestManager.instance == null)
            {
                Debug.LogError("QuestManager.instance not found!");
                Sc_hero.dialogue = false;
                canva.SetActive(false);
                return;
            }

            // --- LOGIKA BARU DENGAN 3 KONDISI ---

            // --- KASUS 1: QUEST SUDAH SELESAI ---
            if (QuestManager.instance.AllNotesCollected())
            {
                // Tampilkan dialog "Terima Kasih"
                NewDialogue("Thank you for saving our world!");
                NewDialogue("You are our hero. The melody is restored.");
                
                dialogueScript.index = 0;
                dialogueScript.ShowNextDialogue();

                // Mulai coroutine untuk menunggu dialog selesai & trigger Game Over
                StartCoroutine(WaitForDialogueEndAndTriggerGameOver());
            }
            // --- KASUS 2: QUEST SEDANG AKTIF (tapi belum selesai) ---
            else if (QuestManager.instance.questActive)
            {
                // Tampilkan dialog "Pengingat"
                // Ini FIX untuk bug "reset" karena tidak memanggil OnTalkStarted
                int notesRemaining = QuestManager.instance.totalNotes - QuestManager.instance.notesCollected;
                NewDialogue("Please hurry, the world is counting on you!");
                NewDialogue($"You still need to find {notesRemaining} more note(s).");

                dialogueScript.index = 0;
                dialogueScript.ShowNextDialogue();
            }
            // --- KASUS 3: QUEST BELUM DIMULAI ---
            else
            {
                // Tampilkan dialog quest "Tolong bantu saya"
                NewDialogue("Please help me recover this world melody");
                NewDialogue("Help me find the 2 notes to save this world!");

                dialogueScript.index = 0;
                dialogueScript.ShowNextDialogue();

                // 🔔 Panggil event OnTalkStarted HANYA DI SINI.
                // Ini akan memicu QuestManager.StartQuest() (yang me-reset notes = 0)
                // HANYA pada percakapan pertama kali.
                OnTalkStarted?.Invoke(); 
                HasTalkedToNpc = true;   // dipakai ChallengeZone
                
                // 🎵 Aktifkan semua note collectible
                ShowAllNotes();
            }
        }
    }
    // -------------------------------------------------

    // --- Coroutine untuk menunggu dialog selesai ---
    IEnumerator WaitForDialogueEndAndTriggerGameOver()
    {
        // Tunggu sampai dialog ditutup (Sc_hero.dialogue di-set false oleh Sc_Dialogue)
        yield return new WaitUntil(() => Sc_hero.dialogue == false);

        Debug.Log("Quest complete dialogue finished. Triggering Game Over!");
        
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true); // Tampilkan UI Game Over
            
            // Opsional: Jeda game dan tampilkan kursor
            // Time.timeScale = 0f;
            // Cursor.lockState = CursorLockMode.None;
            // Cursor.visible = true;
        }
    }
    
    // --- Fungsi lainnya (tidak berubah) ---

    // Membuat dialog baru dari template
    void NewDialogue(string text)
    {
        GameObject template_clone = Instantiate(d_template, canva.transform);

        TextMeshProUGUI textUI = template_clone.GetComponentInChildren<TextMeshProUGUI>(true);
        if (textUI != null)
            textUI.text = text;
        else
            Debug.LogError("TextMeshProUGUI not found inside d_template prefab!");

        template_clone.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "hero")
            player_detection = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "hero")
            player_detection = false;
    }

    // Mengaktifkan semua NoteCollectible (termasuk yang nonaktif)
    void ShowAllNotes()
    {
        // Kita tidak lagi butuh 'hasTriggeredNotes' karena fungsi ini
        // hanya akan dipanggil satu kali dari KASUS 3 di Update()
        NoteCollectible[] allNotes = FindObjectsOfType<NoteCollectible>(true);
        foreach (NoteCollectible note in allNotes)
            note.gameObject.SetActive(true);

        Debug.Log($"✅ {allNotes.Length} notes have been activated!");
    }
}