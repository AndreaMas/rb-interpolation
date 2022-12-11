using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for Player.
/// Handles interaction with small cubes.
/// </summary>
public class PlayerCollision : MonoBehaviour
{
    PlayerMovement pm = null;
    float flyColliderRadius = 1f;
    const float flyForceOnSmallCubes = 20f;
    const float absorbForceOnSmallCubes = 5f;

    private void Awake()
    {
        //if (GetComponent<PlayerMovement>())
            pm = GetComponent<PlayerMovement>();
        
        //if (GetComponent<SphereCollider>())
            flyColliderRadius = GetComponent<SphereCollider>().radius;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Change smallcube color
        if (collision.gameObject.CompareTag("SmallCube"))
        {
            collision.gameObject.GetComponent<SmallCube>().Touched();
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // Change smallcube color
        if (collision.gameObject.CompareTag("SmallCube"))
        {
            collision.gameObject.GetComponent<SmallCube>().Touched();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (pm.isFlying)
        {
            // flying blows cubes away
            if (other.gameObject.CompareTag("SmallCube"))
            {
                Vector3 forceDir = -(transform.position - other.transform.position);
                float force = (flyColliderRadius - forceDir.magnitude) / flyColliderRadius; // 0 when far, 1 when near
                float forceClamped = Mathf.Clamp(force, 0.001f, 0.5f); // you never know ...
                Vector3 forceVector = forceDir.normalized * forceClamped * flyForceOnSmallCubes;
                other.attachedRigidbody.AddForce(forceVector);

                // change color if force significant
                if (forceClamped > 0.3f)
                    other.gameObject.GetComponent<SmallCube>().Touched();
            }
        }

        if (pm.isAbsorbing)
        {
            // absorbing attracts cubes to player
            if (other.gameObject.CompareTag("SmallCube"))
            {
                Vector3 forceDir = -(transform.position - other.transform.position);
                float force = (flyColliderRadius - forceDir.magnitude) / flyColliderRadius; // 0 when far, 1 when near
                float forceClamped = Mathf.Clamp(force, 0.001f, 0.5f); // you never know ...
                Vector3 forceVector = forceDir.normalized * forceClamped * absorbForceOnSmallCubes;
                other.attachedRigidbody.AddForce(-forceVector);

                // change color if force significant
                if (forceClamped > 0.3f)
                    other.gameObject.GetComponent<SmallCube>().Touched();
            }
        }


    }
}
