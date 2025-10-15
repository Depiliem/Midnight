using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Sc_npc : MonoBehaviour
{
    public GameObject d_template;   // prefab dialog (background + text)
    public GameObject canva;        // canvas tempat dialog
    bool player_detection = false;

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
            NewDialogue("AndreGblk");
            NewDialogue("Senang bertemu denganmu di dunia Unity!");

            // Reset index dan aktifkan dialog pertama
            Sc_Dialogue dialogueScript = canva.GetComponent<Sc_Dialogue>();
            if (dialogueScript != null)
            {
                dialogueScript.index = 0;
                dialogueScript.ShowNextDialogue(); // aktifkan dialog pertama
            }
        }
    }

    void NewDialogue(string text)
    {
        // Clone prefab ke canvas
        GameObject template_clone = Instantiate(d_template, canva.transform);

        // Set text
        TextMeshProUGUI textUI = template_clone.GetComponentInChildren<TextMeshProUGUI>(true);
        if (textUI != null)
            textUI.text = text;
        else
            Debug.LogError("TextMeshProUGUI not found inside d_template prefab!");

        // Biarkan tetap aktif dulu
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
}
