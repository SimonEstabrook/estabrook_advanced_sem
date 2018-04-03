using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockUIScript : MonoBehaviour {

	public GameObject followPos;

	void Update () {
		transform.position = new Vector3(Mathf.Round(followPos.transform.position.x), followPos.transform.position.y, Mathf.Round(followPos.transform.position.z));
		if (transform.position.x % 2 != 0)
		{
			transform.position -= Vector3.right;
		}
		if (transform.position.z % 2 != 0)
		{
			transform.position -= Vector3.forward;
		}

	}
}
