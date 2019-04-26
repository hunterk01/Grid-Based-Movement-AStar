using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float cameraHeight;
	
	void Update ()
	{
        MoveCamera();
	}

    public void MoveCamera()
    {
        transform.position = new Vector3(target.position.x, cameraHeight, target.position.z);
        transform.LookAt(target);
    }
}
