using UnityEngine;

public class RhythmManager : MonoBehaviour
{
    public float bpm = 120f;

    [Header("Sync Settings")]
    [Tooltip("Posisi Y tengah dari HitZone Anda (misal: -4.0)")]
    public float hitZoneY = -4f;

    [Tooltip("Posisi Y dari Posisi Awal Note Anda (misal: 7.0)")]
    public float spawnY = 7f;

    [Tooltip("Berapa beat yang dibutuhkan Note untuk mencapai HitZone (misal: 1 atau 2)")]
    public float dropTimeInBeats = 1f;

    private float beatInterval;
    private float calculatedFallSpeed;

    void Start()
    {
        // 1. Hitung Interval Ketukan (detik per ketukan)
        beatInterval = 60f / bpm;

        // 2. Hitung Waktu Jatuh Ideal (Drop Time)
        float totalDropTime = beatInterval * dropTimeInBeats;

        // 3. Hitung Jarak Tempuh Vertikal
        float distance = spawnY - hitZoneY;

        // 4. Hitung Kecepatan Jatuh yang Tepat (Speed = Distance / Time)
        calculatedFallSpeed = distance / totalDropTime;

        Debug.Log($"BPM: {bpm}. Drop Time: {totalDropTime}s. FallSpeed: {calculatedFallSpeed}");

        // 5. Kumpulkan dan Siapkan semua Note di Scene yang diletakkan secara manual
        PrepareAllNotesInScene();
    }

    void PrepareAllNotesInScene()
    {
        NoteObject[] allNotesInScene = FindObjectsOfType<NoteObject>();

        foreach (NoteObject note in allNotesInScene)
        {
            // Berikan kecepatan jatuh yang sudah dihitung
            note.fallSpeed = calculatedFallSpeed;

            // Beri tahu Note kapan harus mulai jatuh
            note.PrepareToStart();
        }
    }

    // Digunakan NoteObject untuk mendapatkan interval waktu antar ketukan
    public float GetBeatInterval()
    {
        return beatInterval;
    }
}