using UnityEngine;

public class ButtonSoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    public void PlayHover()
    {
        if (hoverSound != null)
            audioSource.PlayOneShot(hoverSound);
    }

    public void PlayClick()
    {
        if (clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }
}