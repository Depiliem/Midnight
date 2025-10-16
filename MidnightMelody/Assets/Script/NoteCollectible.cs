using UnityEngine;

public class NoteCollectible : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 50f; // kecepatan rotasi (derajat per detik)

    private void Update()
    {
        // 🔹 Muter di tempat pada sumbu Y
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
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
