using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Sc_npc : MonoBehaviour
{
    public GameObject d_template;
    public GameObject canva;
    bool player_detection = false;

    void Update()
    {
        if (player_detection && Input.GetKeyDown(KeyCode.F) && !Sc_hero.dialogue)
        {
            canva.SetActive(true);
            Sc_hero.dialogue = true;

            // Hapus dulu dialog lama kalau ada sisa
            foreach (Transform child in canva.transform)
            {
                if (child.name.Contains("(Clone)"))
                    Destroy(child.gameObject);
            }

            // Tambahkan dialog baru
            NewDialogue("Hi!");
            NewDialogue("AndreGblk");
            NewDialogue("Senang bertemu denganmu di dunia Unity!");

            // Aktifkan dialog pertama (child ke-1)
            if (canva.transform.childCount > 1)
                canva.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    void NewDialogue(string text)
    {
        // Clone prefab ke canvas
        GameObject template_clone = Instantiate(d_template, canva.transform);

        // Cari TextMeshProUGUI di semua child, lebih aman daripada GetChild(1)
        TextMeshProUGUI textUI = template_clone.GetComponentInChildren<TextMeshProUGUI>(true);
        if (textUI != null)
        {
            textUI.text = text;
        }
        else
        {
            Debug.LogError("TextMeshProUGUI not found inside d_template prefab!");
        }

        template_clone.SetActive(false); // Disembunyikan dulu
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "hero")
        {
            player_detection = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "hero")
        {
            player_detection = false;
        }
    }
}
