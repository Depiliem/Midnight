using UnityEngine;
using System.Collections;

public class TutorialFade : MonoBehaviour
{
    public CanvasGroup group;          // Drag CanvasGroup ke sini
    public float showSeconds = 5f;     // Durasi tampil penuh
    public float fadeSeconds = 1f;     // Durasi fade-out
    public bool disableAfter = true;   // Matikan objek setelah fade

    void Start()
    {
        if (!group) group = GetComponent<CanvasGroup>();

        // Inisialisasi awal UI
        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;

        // Menunggu sebelum mulai fade-out (menggunakan Invoke)
        Invoke("StartiTweenFade", showSeconds);
    }

    void StartiTweenFade()
    {
        // Menggunakan iTween ValueTo untuk mengubah nilai alpha secara halus
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", 1f,
            "to", 0f,
            "time", fadeSeconds,
            "onupdate", "UpdateAlpha",        // Fungsi yang dipanggil setiap frame
            "oncomplete", "OnFadeComplete",    // Fungsi yang dipanggil saat selesai
            "ignoretimescale", true            // Sama seperti WaitForSecondsRealtime
        ));
    }

    // Fungsi bantuan iTween untuk mengupdate Alpha
    void UpdateAlpha(float val)
    {
        group.alpha = val;
    }

    // Fungsi bantuan iTween saat animasi selesai
    void OnFadeComplete()
    {
        group.interactable = false;
        group.blocksRaycasts = false;

        if (disableAfter)
        {
            gameObject.SetActive(false);
        }
    }
}