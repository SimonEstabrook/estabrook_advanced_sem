using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballManager : MonoBehaviour {

    private void Start()
    {
        Destroy(this.gameObject, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Debug.Log("PlayerHit");
            Destroy(this.gameObject);
        }
        else if(other.tag == "Block")
        {
            Destroy(this.gameObject);
        }
    }
}
