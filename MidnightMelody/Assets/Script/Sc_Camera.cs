using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_camera : MonoBehaviour
{
    Transform posHero;
    Vector3 offset;
    float turnSpeed = 4.0f;
    public float collisionOffset = 0.5f; // jarak kamera ke objek yang menabrak
    public float minDistance = 1f;       // jarak minimal kamera ke hero
    public float smoothSpeed = 10f;      // kecepatan smoothing kamera

    private Vector3 desiredOffset;

    void Start()
    {
        posHero = GameObject.Find("hero").transform.Find("camerafoc");

        // offset awal
        offset = new Vector3(posHero.localPosition.x, posHero.localPosition.y, posHero.localPosition.z - 3f);
        desiredOffset = offset;
    }

    void Update()
    {
        if (!Sc_hero.dialogue)
        {
            // Rotasi horizontal offset berdasarkan mouse
            float mouseX = Input.GetAxis("Mouse X") * turnSpeed;
            desiredOffset = Quaternion.AngleAxis(mouseX, Vector3.up) * desiredOffset;

            // Posisi kamera sebelum raytracing
            Vector3 desiredPos = posHero.position + desiredOffset;

            // Raytracing collision
            desiredPos = HandleCollision(posHero.position, desiredPos);

            // Smooth movement
            transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * smoothSpeed);

            // Kamera selalu menghadap hero
            transform.LookAt(posHero.position + Vector3.up * 0.5f); // tambah sedikit height

            // Cursor lock
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Fungsi raytracing untuk deteksi objek di antara hero dan kamera
    Vector3 HandleCollision(Vector3 origin, Vector3 desiredPos)
    {
        Vector3 dir = (desiredPos - origin).normalized;
        float distance = Vector3.Distance(origin, desiredPos);
        RaycastHit hit;

        if (Physics.Raycast(origin, dir, out hit, distance))
        {
            if (hit.collider.gameObject != posHero.gameObject) // jangan tabrakan dengan hero
            {
                // geser kamera sedikit ke arah hero supaya tidak nempel di objek
                Vector3 hitPos = hit.point - dir * collisionOffset;

                // optional: jaga jarak minimal ke hero
                float minDist = minDistance;
                if (Vector3.Distance(origin, hitPos) < minDist)
                    hitPos = origin + dir * minDist;

                return hitPos;
            }
        }

        return desiredPos;
    }
}
