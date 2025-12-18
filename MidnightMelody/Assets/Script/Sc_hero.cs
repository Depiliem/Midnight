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
        

    }

    void Update()
    {
        if (dialogue) return;

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
        bool raycastHit = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, groundCheckDistance);

        if (raycastHit)
        {
            isGrounded = true; 

            if (isJumping && rb.velocity.y < -0.1f)
            {
                isJumping = false;
                HeroAniCont.SetBool("isJump", false);
            }
        }
        else
        {
            isGrounded = false;
        }
    }
    
    public void ResetJump()
    {
        isJumping = false;
        HeroAniCont.SetBool("isJump", false);
    }
}