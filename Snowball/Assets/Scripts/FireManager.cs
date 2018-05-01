using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireManager : MonoBehaviour {

	public int FireHealth = 7;
	public Slider HealthSlider;
	public PlayerController.Team whichTeam;

	public bool isDead = false;

	// Use this for initialization
	void Start () {
		if(GameManager.instance.gMode != GameManager.GameMode.PvE)
		{
			FireHealth *= 2;

		}
		HealthSlider.maxValue = FireHealth;
	}
	
	// Update is called once per frame
	void Update () {
		HealthSlider.value = FireHealth;
		if(FireHealth <= 0 && !isDead)
		{
			isDead = true;
			for(int i = 0; i < transform.childCount; i++)
			{
				if(transform.GetChild(i).gameObject.GetComponent<ParticleSystem>() || transform.GetChild(i).gameObject.GetComponent<Light>())
				{
					transform.GetChild(i).gameObject.SetActive(false);
				}
				if(transform.GetChild(i).gameObject.tag == "Fire")
				{
					transform.GetChild(i).gameObject.SetActive(false);
				}
			}
			HealthSlider.gameObject.SetActive(false);
		}
	}

	public void LoseHealth()
	{
		FireHealth--;
		GetComponent<AudioSource>().Play();
	}
}
