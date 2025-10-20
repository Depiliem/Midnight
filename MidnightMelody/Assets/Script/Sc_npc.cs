using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Sc_npc : MonoBehaviour
{
    public GameObject d_template;   // prefab dialog (background + text)
    public GameObject canva;        // canvas tempat dialog
    bool player_detection = false;

    private bool hasTriggeredNotes = false; // 🔹 Tambahan supaya note hanya muncul sekali

    void Update()
    {
        if (player_detection && Input.GetKeyDown(KeyCode.F) && !Sc_hero.dialogue)
        {
            canva.SetActive(true);
            Sc_hero.dialogue = true;

            // Hapus dialog lama jika ada
            foreach (Transform child in canva.transform)
            {
                if (child.name.Contains("(Clone)"))
                    Destroy(child.gameObject);
            }

            // Tambahkan dialog baru
            NewDialogue("Please help me recover this world melody");
            NewDialogue("Help me find the 3 notes to save this world!");

            // Reset index dan aktifkan dialog pertama
            Sc_Dialogue dialogueScript = canva.GetComponent<Sc_Dialogue>();
            if (dialogueScript != null)
            {
                dialogueScript.index = 0;
                dialogueScript.ShowNextDialogue();
            }

            // 🔹 Setelah dialog pertama kali aktif, munculkan note
            if (!hasTriggeredNotes)
            {
                ShowAllNotes();
                hasTriggeredNotes = true;
            }
        }
    }

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

    // 🔹 Tambahan fungsi untuk menampilkan semua note
    void ShowAllNotes()
    {
        NoteCollectible[] allNotes = FindObjectsOfType<NoteCollectible>(true); // cari semua, termasuk yang nonaktif
        foreach (NoteCollectible note in allNotes)
        {
            note.gameObject.SetActive(true);
        }

        Debug.Log($"✅ {allNotes.Length} notes have been activated!");
    }
}
