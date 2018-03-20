using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

	private bool isOptions = false;

	[SerializeField] private GameObject OptionsMenu, MainMenu;

	private void Start()
	{
		if(SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
		{
			OptionsMenu.SetActive(false);
			MainMenu.SetActive(true);
		}
	}

	public void Quit()
	{
		Debug.Log("Quitting game...");
		Application.Quit();
	}
	public void CharSelect()
	{
		SceneManager.LoadScene(1);
	}
	public void Options()
	{
		if(isOptions)
		{
			OptionsMenu.SetActive(false);
			MainMenu.SetActive(true);
			isOptions = false;
		}
		else
		{
			OptionsMenu.SetActive(true);
			MainMenu.SetActive(false);
			isOptions = true;
		}

	}
	public void MainGame()
	{
		SceneManager.LoadScene(2);
	}
	public void Menu()
	{
		SceneManager.LoadScene(0);
	}

}
