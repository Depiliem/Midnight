using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition instance;
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1.5f;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    IEnumerator FadeAndLoad(string sceneName)
    {
        // 1. Fade In (Layar jadi Hitam)
        float timer = 0;
        fadeCanvasGroup.blocksRaycasts = true; // Agar user tidak klik apa-apa saat transisi
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            yield return null;
        }

        // 2. Pindah Scene
        SceneManager.LoadScene(sceneName);
    }
}