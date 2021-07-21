using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCopter : MonoBehaviour
{
    public Transform helicopter;
    public float SmoothSpeed = 0.125f;
    public Vector3 offset;


    void FixedUpdate()
    {
        Vector3 desiredPosition = helicopter.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, SmoothSpeed);
        transform.position = smoothedPosition;
    }
}
