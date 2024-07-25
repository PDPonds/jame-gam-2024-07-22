using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;
    [SerializeField] float smoothSpeed = 0.125f;

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPostion = target.position + offset;
            Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPostion, smoothSpeed);

            transform.position = smoothedPos;
        }
    }

}
