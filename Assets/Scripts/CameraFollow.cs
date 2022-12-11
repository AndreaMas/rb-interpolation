using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for Camera.
/// Camera follows target smoothly.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform playerPosition;
    [SerializeField] private float smoothTime = 0.3f;
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 offset; // desired camera offset from player

    void Start()
    {
        offset = playerPosition.position - transform.position;
    }

    void LateUpdate()
    {
        // smoothly move camera towards the target+offset
        Vector3 targetPosition = playerPosition.position - offset;
        Vector3 currentPosition = transform.position;
        transform.position = Vector3.SmoothDamp(currentPosition, targetPosition, ref currentVelocity, smoothTime);
    }
}


