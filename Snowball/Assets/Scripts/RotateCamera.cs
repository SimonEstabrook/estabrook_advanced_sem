using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour {

	float tempx, tempy, tempz;

	// Use this for initialization
	void Start () {
		tempx = transform.rotation.x;
		tempy = transform.rotation.y;
		tempz = transform.rotation.z;

	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.D))
        {

			transform.Rotate(0, 1f, 0, Space.World);
        }
        if(Input.GetKey(KeyCode.A))
        {
			transform.Rotate(0, -1f, 0, Space.World);

        }
		if (Input.GetKey (KeyCode.W)) {
			transform.Rotate (1f, 0, 0);
		}
		if (Input.GetKey (KeyCode.S)) {
			transform.Rotate (-1f, 0, 0);
		}
		if (Input.GetKeyDown (KeyCode.X)) {
            resetCameraRot();
		}
	}

    public void resetCameraRot()
    {
        transform.rotation = Quaternion.identity;
    }
}
