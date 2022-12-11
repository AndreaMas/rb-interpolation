using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component placed on client gameobjects, for smooth movement.
/// Activates only when game instance is client.
/// Takes as "input" target position and rotation, updates the position to the target one, smoothly.
/// </summary>
public class ClientSmoothMovement : MonoBehaviour
{
    [HideInInspector] public Vector3 targetPosition;
    [HideInInspector] public Quaternion targetRotation;

    private float smoothAmount = 5f;

    void Start()
    {
        targetPosition = transform.position;
        targetRotation = transform.rotation;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothAmount); // TODO: test SmoothDamp
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothAmount);
    }
}

