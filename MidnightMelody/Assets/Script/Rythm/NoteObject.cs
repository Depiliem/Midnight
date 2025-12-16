using UnityEngine;

public class NoteObject : MonoBehaviour
{
    [HideInInspector]
    public float fallSpeed;

    public bool canBePressed = false;

    [Header("Level Editor Settings")]
    public float startBeat;

    private float startTime;
    private bool isFalling = false;

    private const float DestroyYPosition = -6f;

    // Dipanggil oleh RhythmManager setelah countdown selesai
    public void PrepareToStart()
    {
        RhythmManager manager = FindObjectOfType<RhythmManager>();
        if (manager != null)
        {
            float beatInterval = manager.GetBeatInterval();
            // Hitung waktu (detik) Note ini harus mulai jatuh
            // Note akan menunggu waktu (startBeat * beatInterval) setelah game *benar-benar* dimulai (setelah countdown)
            startTime = Time.time + (startBeat * beatInterval);
        }
        else
        {
            isFalling = true;
        }
    }

    void Update()
    {
        // 1. Logika Mulai Jatuh
        if (!isFalling && Time.time >= startTime)
        {
            isFalling = true;
        }

        // 2. Logika Pergerakan
        if (isFalling)
        {
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
        }

        // 3. Logika Penghancuran (Lewat Batas)
        if (transform.position.y < DestroyYPosition)
        {
            Destroy(gameObject);
        }
    }
}