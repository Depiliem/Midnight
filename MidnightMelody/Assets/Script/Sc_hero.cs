using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_hero : MonoBehaviour
{
    public float speed = 5f;
    public float runSpeed = 7.5f;
    public float rotationSpeed = 10f;
    public static bool dialogue = false; // biar bisa diakses dari mana pun

    Animator HeroAniCont;

    void Start()
    {
        HeroAniCont = GetComponent<Animator>();
    }

    void Update()
    {
        if (!dialogue)
        {
            GerakanType3();
        }
    }

    void GerakanType3()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 targetDirection = new Vector3(h, 0f, v);
        targetDirection = Camera.main.transform.TransformDirection(targetDirection);
        targetDirection.y = 0f;

        bool isMoving = targetDirection.magnitude > 0.1f;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && isMoving;

        if (isMoving)
        {
            Quaternion targetRot = Quaternion.LookRotation(targetDirection, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        float currentSpeed = isRunning ? runSpeed : speed;

        Vector3 moveDir = (Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up) * v) +
                          (Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up) * h);
        transform.position += moveDir.normalized * currentSpeed * Time.deltaTime;

        HeroAniCont.SetBool("isWalk", isMoving && !isRunning);
        HeroAniCont.SetBool("isRun", isRunning);
    }
}
