using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    [Header("Quest Settings")]
    public int totalNotes = 2;            // total note yang harus dikumpulkan
    [HideInInspector] public int notesCollected = 0;
    public bool questActive = false;

    [Header("UI References")]
    public TextMeshProUGUI questText;     // teks UI untuk status quest
    public GameObject questUI;            // canvas quest (opsional)

    void Awake()
    {
        // Singleton (agar hanya 1 instance di scene)
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // Kalau belum diassign manual, cari canvas otomatis
        if (questText == null)
        {
            if (questUI == null)
                questUI = GameObject.Find("questUI");

            if (questUI != null)
            {
                questText = questUI.GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        if (questText != null)
            questText.text = "";
        else
            Debug.LogWarning("⚠TextMeshProUGUI questText belum di-assign di Inspector!");
    }

    // Mulai quest
    public void StartQuest()
    {
        notesCollected = 0;
        questActive = true;
        UpdateQuestUI();
        Debug.Log("Quest dimulai: kumpulkan semua note!");
    }

    // Ketika player mengambil note
    public void CollectNote()
    {
        if (!questActive) return;

        notesCollected++;
        UpdateQuestUI();

        Debug.Log($"🎵 Note diambil! ({notesCollected}/{totalNotes})");

        if (notesCollected >= totalNotes)
        {
            CompleteQuest();
        }
    }

    // Quest selesai
    void CompleteQuest()
    {
        questActive = false;
        if (questText != null)
            questText.text = "<color=#00FF77>Quest Complete!</color>";

        Debug.Log("✅ Quest completed! Semua note telah dikumpulkan.");
    }

    // Update tampilan teks di UI
    void UpdateQuestUI()
    {
        if (questText == null) return;

        if (questActive)
            questText.text = $"Collect Notes: {notesCollected}/{totalNotes}";
        else
            questText.text = "";
    }

    public bool AllNotesCollected()
    {
        return notesCollected >= totalNotes;
    }

    // Dipakai ulang kalau mau reset challenge tanpa reload scene
    public void ResetQuest()
    {
        notesCollected = 0;
        questActive = false;
        if (questText != null)
            questText.text = "";
    }
}
