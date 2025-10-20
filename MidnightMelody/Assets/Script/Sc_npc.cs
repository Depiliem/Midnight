using System;                      // untuk Action event
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

    private bool player_detection = false;
    private bool hasTriggeredNotes = false;  // agar ShowAllNotes hanya sekali
    private bool hasNotifiedTalk = false;    // agar event hanya terpanggil sekali

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

            // Tambahkan dua baris dialog
            NewDialogue("Please help me recover this world melody");
            NewDialogue("Help me find the 3 notes to save this world!");

            // Reset index dan tampilkan dialog pertama
            Sc_Dialogue dialogueScript = canva.GetComponent<Sc_Dialogue>();
            if (dialogueScript != null)
            {
                dialogueScript.index = 0;
                dialogueScript.ShowNextDialogue();
            }

            // 🔔 Beritahu sekali bahwa player sudah mulai bicara
            if (!hasNotifiedTalk)
            {
                hasNotifiedTalk = true;
                HasTalkedToNpc = true;   // dipakai ChallengeZone
                OnTalkStarted?.Invoke(); // trigger event (misal sembunyikan UI “Talk To NPC”)
            }

            // 🎵 Aktifkan semua note collectible sekali saja
            if (!hasTriggeredNotes)
            {
                ShowAllNotes();
                hasTriggeredNotes = true;
            }
        }
    }

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
        NoteCollectible[] allNotes = FindObjectsOfType<NoteCollectible>(true);
        foreach (NoteCollectible note in allNotes)
            note.gameObject.SetActive(true);

        Debug.Log($"✅ {allNotes.Length} notes have been activated!");
    }
}
