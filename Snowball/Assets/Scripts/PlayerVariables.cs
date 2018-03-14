using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InControl;
using UnityEngine.SceneManagement;

public class PlayerVariables : MonoBehaviour {

	public static PlayerVariables instance = null;

	public class Player
	{
		string name;
		int team;
		int controller;
		Color pColor;

		public Player()
		{
			name = "";
			team = 0;
			controller = 0;
			pColor = Color.white;
		}

		public Player(string n, int t, int c, Color pC)
		{
			name = n;
			team = t;
			controller = c;
			pColor = pC;
		}

		#region Getter/Setter
		public void SetName(string n)
		{
			Debug.Log("Name " + n);
			name = n;
		}
		public string GetName()
		{
			return name;
		}
		public void SetTeam(int t)
		{
			Debug.Log("Team " + t.ToString());
			team = t;
		}
		public int GetTeam()
		{
			return team;
		}
		public void SetControl(int c)
		{
			Debug.Log("Controller " + c.ToString());
			controller = c;
		}
		public int GetController()
		{
			return controller;
		}
		public void SetPColor(Color c)
		{
			Debug.Log("Color " + c.r.ToString() + c.g.ToString() + c.b.ToString());
			pColor = c;
		}
		public Color GetPColor()
		{
			return pColor;
		}
		#endregion

	}

	public Player[] PlayerList;

	public int isReady = 0;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}

		DontDestroyOnLoad(this.gameObject);
	}
	void Start () {
		PlayerList = new Player[4];
		for(int i = 0; i < PlayerList.Length;i++)
		{
			PlayerList[i] = new Player();
		}
	}

	private void Update()
	{
		if (isReady >= 2 && SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(1))
		{
			SceneManager.LoadScene(2);
		}
	}

	public void ReadyUp()
	{
		isReady++;
	}
	public void Unready()
	{
		isReady--;
	}
	
	
	
}
