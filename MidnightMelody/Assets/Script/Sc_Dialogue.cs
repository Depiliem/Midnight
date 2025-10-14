using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Dialogue : MonoBehaviour
{
    [HideInInspector]
    public int index = 0; // index child dialog saat ini

    void Update()
    {
        if (!Sc_hero.dialogue)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            ShowNextDialogue();
        }
    }

    public void ShowNextDialogue()
    {
        if (transform.childCount == 0)
            return;

        // Matikan semua child
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);

        // Aktifkan dialog berikutnya
        if (index < transform.childCount)
        {
            transform.GetChild(index).gameObject.SetActive(true);
            index++;
        }
        else
        {
            // selesai semua dialog
            index = 0;
            Sc_hero.dialogue = false;
            gameObject.SetActive(false);
        }
    }
}
