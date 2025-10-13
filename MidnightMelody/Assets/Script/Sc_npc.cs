using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_npc : MonoBehaviour
{
    bool player_detection = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player_detection && Input.GetKeyDown(KeyCode.F))
        {
            print("HI");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "hero")
        {
            player_detection = true;
        }

    }
    private void OnTriggerExit(Collider other)
    {
        player_detection = false;
    }
}
