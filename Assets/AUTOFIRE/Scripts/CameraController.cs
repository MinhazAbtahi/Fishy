using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    public Transform target;
    public Vector3 offset;
    public float smoothFactor;
    private bool isTargetFound = false;

    public static CameraController SharedManager()
    {
        return Instance;
    }
    private void Awake()
    {
        if (!Instance)
            Instance = this;
    }
    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }
        if (target && isTargetFound == false)
        {
            isTargetFound = true;
        }

        Vector3 desiredPosition = target.transform.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(desiredPosition, transform.position, smoothFactor * Time.deltaTime);
        transform.position = smoothedPosition;
        //transform.LookAt(target);
    }
}
