using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameScreenManager : MonoBehaviour {

	public GameObject NameTypeScreen;
	public CharSelectManager SelectManager;

	void Start () {
		SelectManager = transform.parent.gameObject.GetComponent<CharSelectManager>();
	}
	
	
	void Update () {
		
	}
}
