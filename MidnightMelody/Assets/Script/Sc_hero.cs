using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_hero : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 4f;
    public float runSpeed = 6.5f;
    public float rotationSpeed = 10f;

    [Header("Jump Settings")]
    public float jumpForce = 7f;
    [SerializeField] private float fallMultiplier = 3.5f;
    [SerializeField] private float groundCheckDistance = 0.25f;

    public static bool dialogue = false;

    private Animator HeroAniCont;
    private Rigidbody rb;
    private bool isJumping = false;
    private bool isGrounded = true;

    void Start()
    {
        HeroAniCont = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        
        // PENTING: Pastikan juga Rigidbody di Inspector punya:
        // 1. Drag = 0
        // 2. Interpolate = Interpolate (untuk fix stutter kamera)
        // 3. Freeze Rotation di X dan Z
    }

    void Update()
    {
        if (dialogue) return;

        // Logika "else" ini penting untuk mencegah konflik
        // antara Lompat() dan CekTanah() di frame yang sama.
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Lompat();
        }
        else
        {
            CekTanah();
        }
    }

    void FixedUpdate()
    {
        // Selalu terapkan fallMultiplier jika sedang jatuh
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }

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
            // Menggunakan Time.deltaTime di sini oke, tapi lebih baik
            // memindahkan rotasi ke FixedUpdate dan menggunakan rb.MoveRotation()
            // Tapi untuk saat ini, ini tidak masalah.
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
        
        if (isMoving)
        {
            float currentSpeed = isRunning ? runSpeed : speed;
            Vector3 moveDir = (Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up) * v) +
                              (Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up) * h);

            Vector3 newPos = rb.position + moveDir.normalized * currentSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPos);
        }
        
        HeroAniCont.SetBool("isWalk", isMoving && !isRunning && isGrounded);
        HeroAniCont.SetBool("isRun", isRunning && isGrounded);
    }

    void Lompat()
    {
        isJumping = true;
        isGrounded = false;
        HeroAniCont.SetBool("isJump", true);
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void CekTanah()
    {
        // 1. Lakukan Raycast
        bool raycastHit = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, groundCheckDistance);

        if (raycastHit)
        {
            // 2. Jika Raycast kena, kita PASTI di tanah.
            //    Ini memperbaiki bug "stuck".
            isGrounded = true; 

            // 3. Cek untuk mendaratkan animasi
            //    Kita cek 'isJumping' (flag dari Lompat())
            if (isJumping && rb.velocity.y < -0.1f)
            {
                isJumping = false;
                HeroAniCont.SetBool("isJump", false);
            }
        }
        else
        {
            // 4. Jika Raycast tidak kena, kita PASTI di udara.
            isGrounded = false;
        }
    }
    
    public void ResetJump()
    {
        isJumping = false;
        HeroAniCont.SetBool("isJump", false);
    }
}