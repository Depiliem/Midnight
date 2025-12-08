using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_camera : MonoBehaviour
{
    Transform posHero;
    Vector3 offset;
    float turnSpeed = 4.0f;
    public float collisionOffset = 0.5f; 
    public float minDistance = 1f;       
    public float smoothSpeed = 10f;      


    [Header("Camera View Limits")]
    public float minYAngle = -20.0f; 
    public float maxYAngle = 70.0f;  


    private Vector3 desiredOffset;

    void Start()
    {
        posHero = GameObject.Find("hero").transform.Find("camerafoc");

        // offset awal
        offset = new Vector3(posHero.localPosition.x, posHero.localPosition.y, posHero.localPosition.z - 3f);
        desiredOffset = offset;
    }

    void LateUpdate()
    {
        if (!Sc_hero.dialogue)
        {

            float mouseX = Input.GetAxis("Mouse X") * turnSpeed;
            desiredOffset = Quaternion.AngleAxis(mouseX, Vector3.up) * desiredOffset;


            float mouseY = Input.GetAxis("Mouse Y") * turnSpeed;

            Vector3 oldOffset = desiredOffset; 

            desiredOffset = Quaternion.AngleAxis(-mouseY, transform.right) * desiredOffset;

            float angle = Vector3.Angle(Vector3.up, desiredOffset);

            float minVerticalAngle = 90.0f - maxYAngle; 
            float maxVerticalAngle = 90.0f - minYAngle;

            if (angle < minVerticalAngle || angle > maxVerticalAngle)
            {

                desiredOffset = oldOffset; 
            }

            Vector3 desiredPos = posHero.position + desiredOffset;

            desiredPos = HandleCollision(posHero.position, desiredPos);

            transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * smoothSpeed);

            transform.LookAt(posHero.position + Vector3.up * 0.5f); 
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    Vector3 HandleCollision(Vector3 origin, Vector3 desiredPos)
    {
        Vector3 dir = (desiredPos - origin).normalized;
        float distance = Vector3.Distance(origin, desiredPos);
        RaycastHit hit;

        if (Physics.Raycast(origin, dir, out hit, distance))
        {
            if (hit.collider.gameObject != posHero.gameObject)
            {

                Vector3 hitPos = hit.point - dir * collisionOffset;

                float minDist = minDistance;
                if (Vector3.Distance(origin, hitPos) < minDist)
                    hitPos = origin + dir * minDist;

                return hitPos;
            }
        }

        return desiredPos;
    }
}