using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Dialogue : MonoBehaviour
{
    int index = 2;

    void Update()
    {
        // kalau gak di mode dialog, abaikan
        if (!Sc_hero.dialogue)
            return;

        // klik kiri untuk lanjut dialog
        if (Input.GetMouseButtonDown(0) && transform.childCount > 1)
        {
            // matikan dialog sebelumnya (biar cuma satu yang aktif)
            for (int i = 2; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(false);

            // aktifkan dialog sesuai index
            if (index < transform.childCount)
            {
                transform.GetChild(index).gameObject.SetActive(true);
                index++;
            }
            else
            {
                // kalau udah habis semua
                index = 2;
                Sc_hero.dialogue = false;
                gameObject.SetActive(false);
            }
        }
    }
}
