using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballManager : MonoBehaviour {

	public PlayerController.Team team;
	public bool dropped = true;

    private void Start()
    {
		if(!dropped)
		{
			Destroy(this.gameObject, 2f);

		}
	}

    private void OnTriggerEnter(Collider other)
    {
		if(!dropped)
		{
			if (other.tag == "Player" && team != other.GetComponent<PlayerController>().team && team != PlayerController.Team.None)
			{
				Debug.Log("PlayerHit");
				other.gameObject.GetComponent<PlayerController>().TakeDamage();
				Destroy(this.gameObject);
			}
			else if (other.tag == "Block")
			{
				Debug.Log("Hit Wall");
				other.GetComponent<SnowBlockManager>().SpawnSnow();
				other.GetComponent<BoxCollider>().enabled = false;
				other.GetComponent<SnowBlockManager>().destroyCube();
				other.GetComponent<SnowBlockManager>().isDestroyed = true;


				Destroy(this.gameObject);

			}
			else if(other.tag == "Fire")
			{

			}
			else
			{
				Debug.Log("Hit boundry");
				Destroy(this.gameObject);
			}

		}
	}
}
