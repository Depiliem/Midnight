using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_camera : MonoBehaviour
{
    Transform posHero;
    Vector3 offset;
    float turnSpeed = 4.0f;

    void Start()
    {
        posHero = GameObject.Find("hero").transform.Find("camerafoc");
        offset = new Vector3(posHero.localPosition.x, posHero.localPosition.y,posHero.localPosition.z-3f);
    }

    void Update()
    {
        if (!Sc_hero.dialogue)
        {
            offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offset;
            this.transform.position = posHero.position + offset;
            this.transform.LookAt(posHero.position);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}