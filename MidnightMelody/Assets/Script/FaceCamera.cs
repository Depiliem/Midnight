using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera _mainCamera;
    private Vector3 _offsetFromParent;

    private void Start()
    {
        _mainCamera = Camera.main;
        _offsetFromParent = transform.localPosition;
    }

    private void LateUpdate()
    {

        transform.position = transform.parent.position + _offsetFromParent;

        
        Vector3 directionToCamera = _mainCamera.transform.position - transform.position;
        directionToCamera.y = 0;  

        
        if (directionToCamera == Vector3.zero) return;
        Quaternion targetRotation = Quaternion.LookRotation(-directionToCamera, Vector3.up);
        transform.rotation = targetRotation;
    }
}