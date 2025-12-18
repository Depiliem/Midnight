using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 moveOffset = new Vector3(0, 0, 5f); // jarak gerak maju mundur
    public float speed = 2f;                           // kecepatan gerak
    public bool isLooping = true;                       // agar terus bolak-balik

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool movingToTarget = true;
    
    private Rigidbody rb; 

    private void Start()
    {

        rb = GetComponent<Rigidbody>();

        
        startPos = transform.position; 
        targetPos = startPos + moveOffset;
    }


    private void FixedUpdate()
    {
        if (isLooping)
        {
           
            float step = speed * Time.fixedDeltaTime; 
            

            Vector3 currentTarget = movingToTarget ? targetPos : startPos;

            Vector3 newPos = Vector3.MoveTowards(rb.position, currentTarget, step);
            rb.MovePosition(newPos);

            if (Vector3.Distance(rb.position, currentTarget) < 0.05f)
            {
                movingToTarget = !movingToTarget;
            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}