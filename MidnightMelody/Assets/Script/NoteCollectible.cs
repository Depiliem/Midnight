using UnityEngine;

public class NoteCollectible : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "hero")
        {
            QuestManager.instance.CollectNote();
            Destroy(gameObject);
        }
    }
}
