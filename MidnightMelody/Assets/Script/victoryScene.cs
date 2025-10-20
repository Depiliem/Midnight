using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    void Start()
    {
        // Paksa kursor untuk muncul dan tidak terkunci
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}