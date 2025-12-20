using UnityEngine;

public class NoteCollectible : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 50f;

    [Header("Floating Settings")]
    public float floatAmplitude = 0.25f;
    public float floatFrequency = 2f;

    [Header("Audio Settings")]
    public AudioSource ambientSource; // Tarik AudioSource (suara glowing) ke sini
    public AudioClip collectSound;    // Tarik file suara (collect) ke sini

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;

        // JANGAN panggil SetActive(false) di sini. 
        // Mematikan objek dilakukan di Inspector (Hierarchy).

        if (ambientSource != null)
        {
            ambientSource.loop = true;
            ambientSource.Play();
        }
    }

    private void Update()
    {
        // Efek Rotasi
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

        // Efek Melayang (Sin Wave)
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Pastikan nama player kamu tepat "hero"
        if (other.name == "hero")
        {
            // Mainkan suara collect sebelum hancur
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }

            QuestManager.instance.CollectNote();
            Destroy(gameObject);
        }
    }
}