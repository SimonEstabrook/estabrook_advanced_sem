using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballManager : MonoBehaviour {

	public PlayerController.Team team;
	public bool dropped = true;

	public ParticleSystem explodeParticles;

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
				KillSnowball();
			}
			else if (other.tag == "Block")
			{
				Debug.Log("Hit Wall");
				other.GetComponent<AudioSource>().Play();
				other.GetComponent<SnowBlockManager>().SpawnSnow();
				other.GetComponent<BoxCollider>().enabled = false;
				other.GetComponent<SnowBlockManager>().destroyCube();
				other.GetComponent<SnowBlockManager>().isDestroyed = true;

				KillSnowball();
			}
			else if (other.tag == "FireHitbox" && team != other.GetComponent<FireManager>().whichTeam)
			{
				other.GetComponent<FireManager>().LoseHealth();
				KillSnowball();
			}
			else if (tag == "Snowman")
			{
				KillSnowball();
			}
			else if (tag != "Pickup")
			{
				Debug.Log("Hit boundry");
				KillSnowball();
			}

		}
	}

	public void KillSnowball()
	{
		Instantiate(explodeParticles, transform.position, explodeParticles.transform.rotation);
		Destroy(this.gameObject);

	}
}
