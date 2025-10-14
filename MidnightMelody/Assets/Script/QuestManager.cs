using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    public int totalNotes = 3;
    public int notesCollected = 0;

    public bool questActive = false;

    private TextMeshProUGUI questText;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        // Cari TextMeshProUGUI di dalam questUi canvas
        GameObject canvasObj = GameObject.Find("questUI");
        if (canvasObj != null)
        {
            questText = canvasObj.GetComponentInChildren<TextMeshProUGUI>();
            if (questText != null)
                questText.text = "";
            else
                Debug.LogError("TextMeshProUGUI tidak ditemukan di questUi!");
        }
        else
        {
            Debug.LogError("Canvas 'questUi' tidak ditemukan!");
        }
    }

    void UpdateQuestUI()
    {
        if (questText == null) return;

        if (questActive)
            questText.text = "Collect Notes: " + notesCollected + "/" + totalNotes;
        else
            questText.text = "";
    }

    public void StartQuest()
    {
        notesCollected = 0;
        questActive = true;
        UpdateQuestUI();
    }

    public void CollectNote()
    {
        if (!questActive) return;

        notesCollected++;
        UpdateQuestUI();

        if (notesCollected >= totalNotes)
        {
            CompleteQuest();
        }
    }

    void CompleteQuest()
    {
        questActive = false;
        if (questText != null)
            questText.text = "Quest Complete!";
        Debug.Log("Quest completed! All notes collected.");
    }
}
