using UnityEngine;
using System.Collections.Generic;

public class LaneController : MonoBehaviour
{
    // VARIABEL INI HARUS DIATUR UNIK di Inspector (D, F, J, K, dll.)
    public KeyCode inputKey;

    // VARIABEL UNTUK VISUAL FEEDBACK
    // ------------------------------------
    private SpriteRenderer spriteRenderer;

    [Header("Visual Feedback")]
    [Tooltip("Warna yang akan ditampilkan saat tombol ditekan (misal: Putih Penuh)")]
    public Color pressedColor = new Color(1f, 1f, 1f, 1f);
    private Color defaultColor; // Warna default (saat tidak ditekan)
    // ------------------------------------

    private List<NoteObject> notesInZone = new List<NoteObject>();

    void Start()
    {
        // Mendapatkan komponen SpriteRenderer dari objek HitZone ini
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            // Simpan warna default HitZone saat game dimulai
            defaultColor = spriteRenderer.color;
        }

        // Debug untuk memastikan script mengambil Key yang benar dari Inspector
        // Debug.Log($"[LC DEBUG] {gameObject.name} siap dengan Key: {inputKey}");
    }

    void Update()
    {
        // 1. Tombol Ditekan (Logika HIT & Visual Feedback ON)
        if (Input.GetKeyDown(inputKey))
        {
            // Mengubah warna HitZone saat tombol ditekan
            if (spriteRenderer != null)
            {
                spriteRenderer.color = pressedColor;
            }

            // Debug.Log($"[INPUT CONFIRM] **{gameObject.name}** Memicu! Key: {inputKey}");

            if (notesInZone.Count > 0)
            {
                NoteObject noteToHit = notesInZone[0];

                // PENCEGAHAN NULL #1: Cek apakah note itu masih ada
                if (noteToHit != null)
                {
                    // Hancurkan note (Hit!)
                    Destroy(noteToHit.gameObject);
                    notesInZone.RemoveAt(0);

                    // Panggil fungsi Score/Feedback di sini!
                }
                else
                {
                    // PENCEGAHAN NULL #2: Bersihkan list jika note yang ada di indeks 0 sudah null
                    notesInZone.RemoveAt(0);
                }
            }
            else
            {
                // Logika MISS
            }
        }

        // 2. Tombol Dilepas (Visual Feedback OFF)
        if (Input.GetKeyUp(inputKey))
        {
            // Mengembalikan warna HitZone ke default
            if (spriteRenderer != null)
            {
                spriteRenderer.color = defaultColor;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // HANYA proses jika objek yang masuk memiliki Tag "Note"
        if (other.CompareTag("Note"))
        {
            NoteObject note = other.GetComponent<NoteObject>();

            // PENCEGAHAN NULL #3: Pastikan komponen NoteObject ada
            if (note != null)
            {
                // Tambahkan ke list (jika belum ada) dan atur status bisa ditekan
                if (!notesInZone.Contains(note))
                {
                    notesInZone.Add(note);
                    note.canBePressed = true;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Note"))
        {
            NoteObject note = other.GetComponent<NoteObject>();

            // PENCEGAHAN NULL #4: Cek note tidak null dan ada di dalam list sebelum menghapusnya
            if (note != null && notesInZone.Contains(note))
            {
                notesInZone.Remove(note);
                note.canBePressed = false;

                // Panggil fungsi untuk Miss
            }
        }
    }
}