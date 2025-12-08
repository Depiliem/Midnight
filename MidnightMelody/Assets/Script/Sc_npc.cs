using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement;
using UnityEngine.UI;   


public enum Speaker { NPC, HERO }

[System.Serializable]
public struct DialogueLine
{
    [TextArea(3, 10)]
    public string text;
    public Speaker speaker;
}


public class Sc_npc : MonoBehaviour
{

    public static Action OnTalkStarted;
    public static bool HasTalkedToNpc = false;

    [Header("Dialogue Settings (NPC)")]
    public GameObject d_template_npc;
    public GameObject canva_npc;

    [Header("Dialogue Settings (Hero)")]
    public GameObject d_template_hero;
    public GameObject canva_hero;

    // Font
    [Header("Font Settings")]
    public TMP_FontAsset dialogueFont;

    // Typewriter
    [Header("Typewriter Settings")]
    public float typingSpeed = 0.05f; // Kecepatan ketikan (detik per karakter)

    // Fade In/Out
    [Header("Fade Settings")]
    public CanvasGroup dialogueCanvasGroup;
    public float fadeInDuration = 0.3f;
    public float fadeOutDuration = 0.3f;
    private Coroutine fadeCoroutine; // Untuk mengelola coroutine fade

    private bool player_detection = false;
    private Coroutine typingCoroutine; // Untuk mengelola coroutine typewriter

    [Header("Dialogue Content")]
    public List<DialogueLine> initialDialogue;
    public List<DialogueLine> ongoingDialogue;
    public List<DialogueLine> completeDialogue;

    private int currentDialogueIndex = 0;
    private List<DialogueLine> currentDialogueList;

    void Update()
    {
        
        if (player_detection && Input.GetKeyDown(KeyCode.F) && !Sc_hero.dialogue)
        {
            StartDialogueSequence();
        }

        if (Sc_hero.dialogue && Input.GetMouseButtonDown(0))
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                TextMeshProUGUI currentTextUI = GetCurrentlyActiveTextUI();
                if (currentTextUI != null && currentDialogueList != null && currentDialogueIndex > 0)
                {
                    currentTextUI.text = currentDialogueList[currentDialogueIndex - 1].text;
                    currentTextUI.ForceMeshUpdate();
                }
                typingCoroutine = null; // Reset coroutine
            }
            else
            {
                ShowNextDialogue();
            }
        }
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
                Transform dialogBoxTransform = lastChild.Find("dialogbox");
                if (dialogBoxTransform != null)
                {
                    Transform textTransform = dialogBoxTransform.Find("Text (TMP)");
                    if (textTransform != null)
                    {
                        return textTransform.GetComponent<TextMeshProUGUI>();
                    }
                }
            }
        }
        return null;
    }



    void StartDialogueSequence()
    {
        Sc_hero.dialogue = true;
        currentDialogueIndex = 0;
        ClearAllDialogues(); 
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        // Mulai fade in
        fadeCoroutine = StartCoroutine(FadeCanvas(dialogueCanvasGroup, 0f, 1f, fadeInDuration));

        if (QuestManager.instance.AllNotesCollected())
        {
            currentDialogueList = completeDialogue;
            StartCoroutine(WaitForDialogueEndAndTriggerVictoryScene());
        }
        else if (QuestManager.instance.questActive)
        {
            currentDialogueList = ongoingDialogue;
        }
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
            // Semua dialog selesai
            EndDialogueSequence();
        }
    }

    void EndDialogueSequence()
    {
        Sc_hero.dialogue = false;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        // Mulai fade out
        fadeCoroutine = StartCoroutine(FadeCanvas(dialogueCanvasGroup, 1f, 0f, fadeOutDuration, () => {
            canva_npc.SetActive(false);
            if (canva_hero != null) canva_hero.SetActive(false);
        }));
    }

    
    void ClearAllDialogues()
    {
        foreach (Transform child in canva_npc.transform)
        {
            if (child.name.Contains("(Clone)"))
                Destroy(child.gameObject);
        }
        if (canva_hero != null)
        {
            foreach (Transform child in canva_hero.transform)
            {
                if (child.name.Contains("(Clone)"))
                    Destroy(child.gameObject);
            }
        }
    }

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

        if (templateToUse == null || canvasToUse == null)
        {
            Debug.LogError($"Dialogue template or canvas for {speaker} is not set!");
            return;
        }

        if (canvasToUse == canva_hero)
        {
            Debug.Log("DIALOG CHECK: HERO turn. Setting NPC Canvas to False, Hero Canvas to True.");
            canva_npc.SetActive(false);
            canva_hero.SetActive(true);
        }
        else 
        {
            Debug.Log("DIALOG CHECK: NPC turn. Setting NPC Canvas to True, Hero Canvas to False.");
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
            {
                textUI = textTransform.GetComponent<TextMeshProUGUI>();
            }
        }

        if (textUI != null)
        {
     
            if (dialogueFont != null)
            {
                textUI.font = dialogueFont;
            }

            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeLine(text, textUI));

            if (dialogueFont == null)
            {
                Debug.LogError("Dialogue Font Asset is not assigned in the Inspector! Using default font fallback.");
            }
        }
        else
            Debug.LogError("Gagal menemukan TextMeshProUGUI. Cek apakah objek 'dialogbox' dan 'Text (TMP)' ada di Prefab!");

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

    IEnumerator FadeCanvas(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration, Action onComplete = null)
    {
        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup not assigned for fading!");
            onComplete?.Invoke();
            yield break;
        }

        canvasGroup.gameObject.SetActive(true); // Pastikan CanvasGroup aktif saat fade in
        float timer = 0f;
        while (timer < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = endAlpha; 

        if (endAlpha == 0) {
            canvasGroup.gameObject.SetActive(false);
        }

        onComplete?.Invoke();
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

    IEnumerator WaitForDialogueEndAndTriggerVictoryScene()
    {
        yield return new WaitUntil(() => Sc_hero.dialogue == false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("Victory");
    }

    void ShowAllNotes()
    {
        NoteCollectible[] allNotes = FindObjectsOfType<NoteCollectible>(true);
        foreach (NoteCollectible note in allNotes)
            note.gameObject.SetActive(true);

        Debug.Log($"Notes activated: {allNotes.Length}");
    }
}