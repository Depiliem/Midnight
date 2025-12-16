using UnityEngine;

public class NoteObject : MonoBehaviour
{
    [HideInInspector]
    public float fallSpeed;

    public bool canBePressed = false;

    [Header("Level Editor Settings")]
    [Tooltip("Ketukan (Beat) ke berapa Note ini mulai jatuh (misal: 1, 2.5, 3)")]
    public float startBeat;

    private float startTime;
    private bool isFalling = false;

    // Digunakan untuk menghancurkan note jika melewati batas
    private const float DestroyYPosition = -6f;

    // Dipanggil oleh RhythmManager/LevelManager saat Start
    public void PrepareToStart()
    {
        RhythmManager manager = FindObjectOfType<RhythmManager>();
        if (manager != null)
        {
            float beatInterval = manager.GetBeatInterval();
            // Hitung waktu (detik) Note ini harus mulai jatuh
            startTime = startBeat * beatInterval;
        }
        else
        {
            Debug.LogError("RhythmManager tidak ditemukan! Note jatuh segera.");
            isFalling = true;
        }
    }

    void Update()
    {
        // 1. Logika Mulai Jatuh (Sinkronisasi dengan Waktu BPM)
        if (!isFalling && Time.time >= startTime)
        {
            isFalling = true;
        }

        // 2. Logika Pergerakan
        if (isFalling)
        {
            // Note jatuh ke bawah (sumbu Y negatif)
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
        }

        // 3. Logika Penghancuran (Lewat Batas)
        if (transform.position.y < DestroyYPosition)
        {
            // Tambahkan logika untuk mengurangi skor/nyawa karena MISS (jika diperlukan)
            Destroy(gameObject);
        }
    }
}