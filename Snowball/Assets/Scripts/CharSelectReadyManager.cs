using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;

public class CharSelectReadyManager : MonoBehaviour {

	public static CharSelectReadyManager instance = null;

	public int isReady = 0;

	public GameObject startPanel;

	public int ActivePlayers;


	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}
	// Update is called once per frame
	void Update () {
		ActivePlayers = GameObject.FindGameObjectsWithTag("Active").Length;

		if (isReady >= ActivePlayers && ActivePlayers > 0 && SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(1))
		{
			startPanel.SetActive(true);
			for (int i = 0; i < InputManager.Devices.Count; i++)
			{
				if (InputManager.Devices[i].Command.WasPressed)
				{
					SceneManager.LoadScene(2);

				}
			}
		}
		else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(1))
		{
			startPanel.SetActive(false);
		}


	}

	public void Unready()
	{
		isReady--;
	}

	public void ReadyUp()
	{
		isReady++;
	}


}
