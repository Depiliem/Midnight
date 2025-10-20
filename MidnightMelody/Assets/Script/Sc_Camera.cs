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

    // --- BARU: Variabel untuk batas atas/bawah ---
    [Header("Camera View Limits")]
    public float minYAngle = -20.0f; // Batas melihat ke bawah (negatif)
    public float maxYAngle = 70.0f;  // Batas melihat ke atas (positif)
    // ---------------------------------------------

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
            // --- MODIFIKASI: Input Horizontal & Vertikal ---

            // 1. Rotasi horizontal (Kiri/Kanan) - Ini sudah ada
            float mouseX = Input.GetAxis("Mouse X") * turnSpeed;
            desiredOffset = Quaternion.AngleAxis(mouseX, Vector3.up) * desiredOffset;

            // 2. Rotasi vertikal (Atas/Bawah)
            float mouseY = Input.GetAxis("Mouse Y") * turnSpeed;
            
            // Simpan offset lama jika rotasi baru gagal (karena clamp)
            Vector3 oldOffset = desiredOffset; 

            // Terapkan rotasi vertikal. Kita pakai 'transform.right' (sumbu kanan lokal kamera)
            // Tanda negatif (-) agar mouse ke atas = kamera ke atas (standard, bukan inverted)
            desiredOffset = Quaternion.AngleAxis(-mouseY, transform.right) * desiredOffset;

            // 3. Clamping (Batasan)
            // Kita cek sudut baru dari 'desiredOffset' relatif terhadap 'Vector3.up' (sumbu Y dunia)
            // Sudut 90 = horizontal. 0 = lurus ke atas. 180 = lurus ke bawah.
            float angle = Vector3.Angle(Vector3.up, desiredOffset);

            // Kita ubah minYAngle dan maxYAngle menjadi rentang 0-180
            // Contoh: maxYAngle 70 (ke atas) -> 90 - 70 = 20 derajat dari sumbu Y
            // Contoh: minYAngle -20 (ke bawah) -> 90 - (-20) = 110 derajat dari sumbu Y
            float minVerticalAngle = 90.0f - maxYAngle; 
            float maxVerticalAngle = 90.0f - minYAngle;

            // Jika sudut baru di luar batas, batalkan rotasi vertikal
            if (angle < minVerticalAngle || angle > maxVerticalAngle)
            {
                // Batal, kembali ke offset sebelum rotasi vertikal
                desiredOffset = oldOffset; 
            }
            
            // ---------------------------------------------------

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