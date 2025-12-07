using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    [Header("Quest Settings")]
    public int totalNotes = 2;            
    [HideInInspector] public int notesCollected = 0;
    public bool questActive = false;

    [Header("UI References")]
    public TextMeshProUGUI questText;     
    public GameObject questUI;            

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        
        Sc_npc.OnTalkStarted += StartQuest;
        

       
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

    
    private void OnDestroy()
    {
        Sc_npc.OnTalkStarted -= StartQuest;
    }
    

    
    public void StartQuest()
    {
        
        if (questActive) return; 
        
        notesCollected = 0;
        questActive = true;
        UpdateQuestUI();
        Debug.Log("Quest dimulai: kumpulkan semua note!");
    }

    
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

    
    void CompleteQuest()
    {
        questActive = false;
        if (questText != null)
            questText.text = "<color=#00FF77>Quest Complete!\nreturn to NPC</color>";

        Debug.Log("✅ Quest completed! Semua note telah dikumpulkan.");
    }

    
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

    
    public void ResetQuest()
    {
        notesCollected = 0;
        questActive = false;
        if (questText != null)
            questText.text = "";
    }
}