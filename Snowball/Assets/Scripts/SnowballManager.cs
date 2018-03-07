using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballManager : MonoBehaviour {

	public PlayerController.Team team;

    private void Start()
    {
        Destroy(this.gameObject, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && team != other.GetComponent<PlayerController>().team && team != PlayerController.Team.None)
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
