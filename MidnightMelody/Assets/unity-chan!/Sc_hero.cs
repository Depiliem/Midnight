using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_hero : MonoBehaviour
{
    public float speed = 5f;
    public float runSpeed = 7.5f;
    public float rotationSpeed = 10f;

    Animator HeroAniCont; // gak perlu di-set manual, akan otomatis diambil

    void Start()
    {
        // otomatis ambil komponen Animator dari gameobject ini
        HeroAniCont = GetComponent<Animator>();
    }

    void Update()
    {
        GerakanType3();
    }

    void GerakanType3()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // arah input
        Vector3 targetDirection = new Vector3(h, 0f, v);
        targetDirection = Camera.main.transform.TransformDirection(targetDirection);
        targetDirection.y = 0f;

        // cek apakah sedang berjalan atau berlari
        bool isMoving = targetDirection.magnitude > 0.1f;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && isMoving;

        // rotasi hero mengikuti arah gerak
        if (isMoving)
        {
            Quaternion targetRot = Quaternion.LookRotation(targetDirection, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // atur kecepatan tergantung apakah sedang lari
        float currentSpeed = isRunning ? runSpeed : speed;

        // posisi hero bergerak sesuai arah kamera
        Vector3 moveDir = (Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up) * v) +
                          (Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up) * h);
        transform.position += moveDir.normalized * currentSpeed * Time.deltaTime;

        // update animator parameter
        HeroAniCont.SetBool("isWalk", isMoving && !isRunning);
        HeroAniCont.SetBool("isRun", isRunning);
    }
}
