using UnityEngine;
using System.Collections;

public class TutorialFade : MonoBehaviour
{
    public CanvasGroup group;          // drag your CanvasGroup here (or leave empty)
    public float showSeconds = 5f;     // how long it stays fully visible
    public float fadeSeconds = 1f;     // fade-out duration
    public bool disableAfter = true;   // hide object after fade

    IEnumerator Start()
    {
        if (!group) group = GetComponent<CanvasGroup>();
        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;

        yield return new WaitForSecondsRealtime(showSeconds);

        float t = 0f;
        while (t < fadeSeconds)
        {
            t += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(1f, 0f, t / fadeSeconds);
            yield return null;
        }

        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;

        if (disableAfter) gameObject.SetActive(false);
    }
}
