using UnityEngine;

public class NoteCollectible : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 50f; // Kecepatan rotasi (derajat per detik)

    [Header("Floating Settings")]
    public float floatAmplitude = 0.25f; // Seberapa tinggi naik-turun
    public float floatFrequency = 2f;    // Seberapa cepat gerak naik-turun

    private Vector3 startPos;

    private void Start()
    {
        // Simpan posisi awal agar gerakan naik-turun relatif terhadap titik awal
        startPos = transform.position;
    }

    private void Update()
    {
        // 🔹 Muter di tempat pada sumbu Y
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

        // 🔹 Naik-turun lembut menggunakan sinus
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "hero")
        {
            QuestManager.instance.CollectNote();
            Destroy(gameObject);
        }
    }
}
