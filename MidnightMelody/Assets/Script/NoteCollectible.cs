using UnityEngine;

public class NoteCollectible : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 50f; 

    [Header("Floating Settings")]
    public float floatAmplitude = 0.25f; 
    public float floatFrequency = 2f;   

    private Vector3 startPos;

    private void Start()
    {

        startPos = transform.position;
        gameObject.SetActive(false);
    }

    private void Update()
    {

        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

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
