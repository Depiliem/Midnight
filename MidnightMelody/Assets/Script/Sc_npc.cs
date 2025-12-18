using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

// =========================================================
// STRUKTUR DATA DIALOG
// =========================================================

public enum Speaker { NPC, HERO }

[System.Serializable]
public struct DialogueLine
{
    [TextArea(3, 10)]
    public string text;
    public Speaker speaker;
}

// =========================================================
// SKRIP SC_NPC
// =========================================================

public class Sc_npc : MonoBehaviour
{
    // ===== DIPAKAI OLEH SCRIPT LAIN =====
    public static Action OnTalkStarted;
    public static bool HasTalkedToNpc = false;

    // Prefab Dialog
    [Header("Dialogue Settings (NPC)")]
    public GameObject d_template_npc;
    public GameObject canva_npc;

    [Header("Dialogue Settings (Hero)")]
    public GameObject d_template_hero;
    public GameObject canva_hero;

    // Font & Typewriter
    [Header("Visual Settings")]
    public TMP_FontAsset dialogueFont;
    public float typingSpeed = 0.05f;

    private bool player_detection = false;
    private Coroutine typingCoroutine;

    // VARIABEL KONTROL DIALOG
    [Header("Dialogue Content")]
    public List<DialogueLine> initialDialogue;    // Dialog pertama kali ketemu
    public List<DialogueLine> ongoingDialogue;    // Dialog saat quest jalan tapi belum cukup note
    public List<DialogueLine> collect2NotesDialogue; // BARU: Dialog saat sudah kumpul 2 note
    public List<DialogueLine> completeDialogue;   // Dialog saat semua note terkumpul (Victory)

    private int currentDialogueIndex = 0;
    private List<DialogueLine> currentDialogueList;

    // =========================================================
    // UPDATE - Kontrol Input
    // =========================================================

    void Update()
    {
        // Mulai dialog dengan tombol F
        if (player_detection && Input.GetKeyDown(KeyCode.F) && !Sc_hero.dialogue)
        {
            StartDialogueSequence();
        }

        // Lanjut dialog dengan Klik Kiri
        if (Sc_hero.dialogue && Input.GetMouseButtonDown(0))
        {
            if (typingCoroutine != null) // Jika masih mengetik, skip ke teks penuh
            {
                StopCoroutine(typingCoroutine);
                TextMeshProUGUI currentTextUI = GetCurrentlyActiveTextUI();
                if (currentTextUI != null && currentDialogueList != null && currentDialogueIndex > 0)
                {
                    currentTextUI.text = currentDialogueList[currentDialogueIndex - 1].text;
                    currentTextUI.ForceMeshUpdate();
                }
                typingCoroutine = null;
            }
            else
            {
                ShowNextDialogue();
            }
        }
    }

    // =========================================================
    // LOGIKA SESI DIALOG
    // =========================================================

    void StartDialogueSequence()
    {
        Sc_hero.dialogue = true;
        currentDialogueIndex = 0;
        ClearAllDialogues();

        // 1. CEK KONDISI: Sudah kumpul 2 Note? (Pindah ke RhythmLevel)
        if (QuestManager.instance.notesCollected == 2)
        {
            currentDialogueList = collect2NotesDialogue;
            StartCoroutine(WaitAndSwitchToRhythm());
        }
        // 2. CEK KONDISI: Semua Note kumpul? (Victory Scene)
        else if (QuestManager.instance.AllNotesCollected())
        {
            currentDialogueList = completeDialogue;
            StartCoroutine(WaitForDialogueEndAndTriggerVictoryScene());
        }
        // 3. CEK KONDISI: Quest sedang jalan?
        else if (QuestManager.instance.questActive)
        {
            currentDialogueList = ongoingDialogue;
        }
        // 4. KONDISI DEFAULT: Belum mulai quest
        else
        {
            currentDialogueList = initialDialogue;
            OnTalkStarted?.Invoke();
            HasTalkedToNpc = true;
            ShowAllNotes();
        }

        ShowNextDialogue();
    }

    void ShowNextDialogue()
    {
        ClearAllDialogues();

        if (currentDialogueList == null || currentDialogueList.Count == 0)
        {
            EndDialogueSequence();
            return;
        }

        if (currentDialogueIndex < currentDialogueList.Count)
        {
            DialogueLine line = currentDialogueList[currentDialogueIndex];
            NewDialogue(line.text, line.speaker.ToString());
            currentDialogueIndex++;
        }
        else
        {
            EndDialogueSequence();
        }
    }

    void EndDialogueSequence()
    {
        Sc_hero.dialogue = false;

        // Langsung nonaktifkan canvas (instan)
        if (canva_npc != null) canva_npc.SetActive(false);
        if (canva_hero != null) canva_hero.SetActive(false);
    }

    // =========================================================
    // TRANSISI SCENE
    // =========================================================

    IEnumerator WaitAndSwitchToRhythm()
    {
        // Tunggu sampai dialog selesai ditutup pemain
        yield return new WaitUntil(() => Sc_hero.dialogue == false);

        // Panggil SceneTransition (Layar Hitam) jika ada, jika tidak langsung pindah
        if (SceneTransition.instance != null)
        {
            SceneTransition.instance.TransitionToScene("RythmLevel");
        }
        else
        {
            SceneManager.LoadScene("RythmLevel");
        }
    }

    IEnumerator WaitForDialogueEndAndTriggerVictoryScene()
    {
        yield return new WaitUntil(() => Sc_hero.dialogue == false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Victory");
    }

    // =========================================================
    // FUNGSI UTILITY (UI & TYPEWRITER)
    // =========================================================

    public void NewDialogue(string text, string speaker)
    {
        GameObject templateToUse;
        GameObject canvasToUse;

        if (speaker.ToUpper() == "HERO")
        {
            templateToUse = d_template_hero;
            canvasToUse = canva_hero;
        }
        else
        {
            templateToUse = d_template_npc;
            canvasToUse = canva_npc;
        }

        if (templateToUse == null || canvasToUse == null) return;

        // Atur Aktivasi Canvas
        if (canvasToUse == canva_hero)
        {
            canva_npc.SetActive(false);
            canva_hero.SetActive(true);
        }
        else
        {
            canva_npc.SetActive(true);
            if (canva_hero != null) canva_hero.SetActive(false);
        }

        GameObject clone = Instantiate(templateToUse, canvasToUse.transform);
        Transform dialogBoxTransform = clone.transform.Find("dialogbox");
        TextMeshProUGUI textUI = null;

        if (dialogBoxTransform != null)
        {
            Transform textTransform = dialogBoxTransform.Find("Text (TMP)");
            if (textTransform != null)
                textUI = textTransform.GetComponent<TextMeshProUGUI>();
        }

        if (textUI != null)
        {
            if (dialogueFont != null) textUI.font = dialogueFont;
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeLine(text, textUI));
        }

        clone.SetActive(true);
    }

    IEnumerator TypeLine(string fullText, TextMeshProUGUI textComponent)
    {
        textComponent.text = "";
        foreach (char c in fullText.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        typingCoroutine = null;
    }

    private TextMeshProUGUI GetCurrentlyActiveTextUI()
    {
        Transform activeCanvasTransform = null;
        if (canva_hero != null && canva_hero.activeInHierarchy) activeCanvasTransform = canva_hero.transform;
        else if (canva_npc != null && canva_npc.activeInHierarchy) activeCanvasTransform = canva_npc.transform;

        if (activeCanvasTransform != null && activeCanvasTransform.childCount > 0)
        {
            Transform lastChild = activeCanvasTransform.GetChild(activeCanvasTransform.childCount - 1);
            if (lastChild != null)
            {
                Transform dBox = lastChild.Find("dialogbox");
                if (dBox != null)
                {
                    Transform txt = dBox.Find("Text (TMP)");
                    if (txt != null) return txt.GetComponent<TextMeshProUGUI>();
                }
            }
        }
        return null;
    }

    void ClearAllDialogues()
    {
        foreach (Transform child in canva_npc.transform)
        {
            if (child.name.Contains("(Clone)")) Destroy(child.gameObject);
        }
        if (canva_hero != null)
        {
            foreach (Transform child in canva_hero.transform)
            {
                if (child.name.Contains("(Clone)")) Destroy(child.gameObject);
            }
        }
    }

    // =========================================================
    // TRIGGERS
    // =========================================================

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "hero") player_detection = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "hero") player_detection = false;
    }

    void ShowAllNotes()
    {
        NoteCollectible[] allNotes = FindObjectsOfType<NoteCollectible>(true);
        foreach (NoteCollectible note in allNotes)
            note.gameObject.SetActive(true);
    }
}