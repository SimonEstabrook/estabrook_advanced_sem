using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InControl;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour {

	public enum HealthMode
	{
		livesMode = 0,
		coldMeter = 1
	}

	public enum GameMode
	{
		Co_op = 0,
		PvP = 1
	}


	public static GameManager instance = null;

	public GameMode gMode;
    public bool GameStart = false;
	public bool GameOver = false;
	public bool isPaused = false;

	[Header("Player Info")]
	public List<GameObject> Players;
	public List<GameObject> Team1, Team2, Team3, Team4;
	public int T1, T2, T3, T4;

	[Header("UI")]
	public Text DebugText;
	public HealthMode mode;

	[Header("Prefabs")]
	public GameObject PauseMenu;

    [Header("Debugs")]
    public bool seeNodes = false;
    public bool GodMode = false;
	public bool PlayerCheats = false;

	private float timer = 0;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }


    // Use this for initialization
    void Start () {
		Players.AddRange(GameObject.FindGameObjectsWithTag("Player"));
		for(int i = 0; i < Players.Count; i++)
		{
			switch(Players[i].GetComponent<PlayerController>().team)
			{
				case PlayerController.Team.Team1:
					Team1.Add(Players[i]);
					break;
				case PlayerController.Team.Team2:
					Team2.Add(Players[i]);
					break;
				case PlayerController.Team.Team3:
					Team3.Add(Players[i]);
					break;
				case PlayerController.Team.Team4:
					Team4.Add(Players[i]);
					break;
				default:
					Debug.LogError("Invalid Team");
					break;
			}
		}

		T1 = Team1.Count;
		T2 = Team2.Count;
		T3 = Team3.Count;
		T4 = Team4.Count;

		if (T1 > 0 && T2 > 0)
		{
			gMode = GameMode.PvP;
		}
		else
		{
			gMode = GameMode.Co_op;
		}
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < InputManager.Devices.Count;i++)
		{
			if(InputManager.Devices[i].Command.WasPressed)
			{
				isPaused = !isPaused;
			}
			if(isPaused && InputManager.Devices[i].RightTrigger.IsPressed && InputManager.Devices[i].LeftTrigger.IsPressed && InputManager.Devices[i].Action1.IsPressed)
			{
				SceneManager.LoadScene(0);
			}
		}

		if(isPaused)
		{
			Time.timeScale = 0;
			PauseMenu.SetActive(true);
			
		}
		else
		{
			Time.timeScale = 1;
			PauseMenu.SetActive(false);
		}

        checkDebug();
		if (DebugText.enabled == true)
		{
			if(timer <= 5)
			{
				timer += Time.deltaTime;

			}
			else
			{
				DebugText.enabled = false;
				timer = 0;
			}

		}
		if(gMode == GameMode.PvP && (Team1.Count <= 0 || Team2.Count <= 0))
		{
			GameOver = true;
		}
	}

    void checkDebug()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("SEE NODES");
            seeNodes = toggle(seeNodes);
			DisplayDebugText("SEE NODES", seeNodes);
        }
		if(Input.GetKeyDown(KeyCode.C))
		{
			Debug.Log("TOGGLE CHEATS");
			PlayerCheats = toggle(PlayerCheats);
			DisplayDebugText("TOGGLE CHEATS", PlayerCheats);
		}
		if(Input.GetKeyDown(KeyCode.G))
		{
			Debug.Log("TOGGLE GODMODE");
			GodMode = toggle(GodMode);
			DisplayDebugText("TOGGLE GODMODE", GodMode);
		}
    }

	void DisplayDebugText(string s, bool b)
	{
		DebugText.enabled = true;
		DebugText.text = (s + ": " + b.ToString());

	}


    bool toggle(bool b)
    {
        if(b == true)
        {
            b = false;
        }
        else
        {
            b = true;
        }
        return b;
    }

	public void PlayerGone(PlayerController.Team t)
	{
		if(t == PlayerController.Team.Team1)
		{
			T1--;
		}
		else
		{
			T2--;
		}
	}
}
