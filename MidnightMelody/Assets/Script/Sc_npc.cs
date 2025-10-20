using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Pastikan ini ada

public class Sc_npc : MonoBehaviour
{
    // === EVENT: dipanggil saat ngobrol pertama kali ===
    public static Action OnTalkStarted;

    // === FLAG global, dicek oleh ChallengeZone ===
    public static bool HasTalkedToNpc = false;

    [Header("Dialogue Settings")]
    public GameObject d_template;   // prefab dialog (background + text)
    public GameObject canva;        // canvas tempat dialog

    private bool player_detection = false;

    // --- FUNGSI UPDATE DENGAN 3 KONDISI ---
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
                NewDialogue("Thank you for saving our world!");
                NewDialogue("You are our hero. The melody is restored.");
                
                dialogueScript.index = 0;
                dialogueScript.ShowNextDialogue();
                StartCoroutine(WaitForDialogueEndAndTriggerVictoryScene()); 
            }
            // --- KASUS 2: QUEST SEDANG AKTIF (tapi belum selesai) ---
            else if (QuestManager.instance.questActive)
            {
                int notesRemaining = QuestManager.instance.totalNotes - QuestManager.instance.notesCollected;
                NewDialogue("Please hurry, the world is counting on you!");
                NewDialogue($"You still need to find {notesRemaining} more note(s).");

                dialogueScript.index = 0;
                dialogueScript.ShowNextDialogue();
            }
            // --- KASUS 3: QUEST BELUM DIMULAI ---
            else
            {
                NewDialogue("Please help me recover this world melody");
                NewDialogue("Help me find the 2 notes to save this world!");

                dialogueScript.index = 0;
                dialogueScript.ShowNextDialogue();

                OnTalkStarted?.Invoke(); 
                HasTalkedToNpc = true;
                ShowAllNotes();
            }
        }
    }

    // --- Coroutine diubah untuk memuat scene ---
    IEnumerator WaitForDialogueEndAndTriggerVictoryScene()
    {
        // Tunggu sampai dialog ditutup
        yield return new WaitUntil(() => Sc_hero.dialogue == false);

        Debug.Log("Quest complete dialogue finished. Loading Victory scene!");
        
        // --- PERBAIKAN ---
        // Tampilkan dan buka kunci kursor SEBELUM pindah scene
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // -----------------

        SceneManager.LoadScene("Victory");
    }
    
    // --- Fungsi lainnya (tidak berubah) ---

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
    
    void ShowAllNotes()
    {
        NoteCollectible[] allNotes = FindObjectsOfType<NoteCollectible>(true);
        foreach (NoteCollectible note in allNotes)
            note.gameObject.SetActive(true);

        Debug.Log($"✅ {allNotes.Length} notes have been activated!");
    }
}