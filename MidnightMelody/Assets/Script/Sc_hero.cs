using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_hero : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float runSpeed = 7.5f;
    public float rotationSpeed = 10f;

    [Header("Jump Settings")]
    public float jumpForce = 7f;
    [SerializeField] private float fallMultiplier = 3.5f; // makin besar makin cepat jatuh

    public static bool dialogue = false;

    private Animator HeroAniCont;
    private Rigidbody rb;
    private bool isJumping = false;

    void Start()
    {
        HeroAniCont = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (dialogue) return;

        // Tekan spasi untuk lompat (hanya jika tidak sedang melompat)
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            Lompat();
        }
    }

    void FixedUpdate()
    {
        // Tambahan gaya jatuh biar lebih realistis
        if (isJumping && rb.velocity.y < 0)
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
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        float currentSpeed = isRunning ? runSpeed : speed;
        Vector3 moveDir = (Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up) * v) +
                          (Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up) * h);

        Vector3 newPos = rb.position + moveDir.normalized * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);

        HeroAniCont.SetBool("isWalk", isMoving && !isRunning);
        HeroAniCont.SetBool("isRun", isRunning);
    }

    void Lompat()
    {
        isJumping = true;
        HeroAniCont.SetBool("isJump", true);

        // Reset velocity biar loncatan konsisten
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    // 🔹 Auto deteksi "mendarat" tanpa tag
    private void OnCollisionEnter(Collision collision)
    {
        // Cek arah benturan: kalau normal-nya mengarah ke atas (tanah di bawah karakter)
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
            {
                isJumping = false;
                HeroAniCont.SetBool("isJump", false);
                break;
            }
        }
    }

    // 🔹 Optional: Animation Event di akhir animasi lompat
    public void ResetJump()
    {
        isJumping = false;
        HeroAniCont.SetBool("isJump", false);
    }
}
