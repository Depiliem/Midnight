// TalkHint.cs
using UnityEngine;

public class TalkHint : MonoBehaviour
{
    [SerializeField] GameObject hint;   

    void Awake()
    {
        if (!hint) hint = gameObject;
        hint.SetActive(true);
        Sc_npc.OnTalkStarted += HandleTalkStarted;
    }

    void OnDestroy() => Sc_npc.OnTalkStarted -= HandleTalkStarted;

    void HandleTalkStarted() => hint.SetActive(false);
}
