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
    public Color pressedColor = new Color(1f, 1f, 1f, 1f);
    private Color defaultColor;

    public GameObject outlineObject;

    [Header("Hit Effects")]
    public GameObject hitEffectPrefab;
    // ------------------------------------

    private List<NoteObject> notesInZone = new List<NoteObject>();
    private ScoreManager scoreManager;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            defaultColor = spriteRenderer.color;
        }

        scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager tidak ditemukan di Scene!");
        }

        // Pastikan Outline nonaktif di awal
        if (outlineObject != null)
        {
            outlineObject.SetActive(false);
        }
    }

    void Update()
    {
        // 1. Tombol Ditekan (Logika HIT & Visual Feedback ON)
        if (Input.GetKeyDown(inputKey))
        {
            // Visual Feedback ON: Warna & Outline
            if (spriteRenderer != null)
            {
                spriteRenderer.color = pressedColor;
            }
            if (outlineObject != null)
            {
                outlineObject.SetActive(true);
            }

            if (notesInZone.Count > 0)
            {
                NoteObject noteToHit = notesInZone[0];

                if (noteToHit != null)
                {
                    // TAMBAH SKOR
                    if (scoreManager != null)
                    {
                        scoreManager.AddScore();
                    }

                    // INSTANSIASI EFEK PARTIKEL
                    if (hitEffectPrefab != null)
                    {
                        GameObject hitEffect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
                        Destroy(hitEffect, 1f); // Hancurkan partikel setelah 1 detik
                    }

                    Destroy(noteToHit.gameObject);
                    notesInZone.RemoveAt(0);
                }
                else
                {
                    notesInZone.RemoveAt(0);
                }
            }
        }

        // 2. Tombol Dilepas (Visual Feedback OFF)
        if (Input.GetKeyUp(inputKey))
        {
            // Visual Feedback OFF: Warna & Outline
            if (spriteRenderer != null)
            {
                spriteRenderer.color = defaultColor;
            }
            if (outlineObject != null)
            {
                outlineObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Note"))
        {
            NoteObject note = other.GetComponent<NoteObject>();

            if (note != null)
            {
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

            if (note != null && notesInZone.Contains(note))
            {
                notesInZone.Remove(note);
                note.canBePressed = false;
                // Logika MISS otomatis di sini
            }
        }
    }
}