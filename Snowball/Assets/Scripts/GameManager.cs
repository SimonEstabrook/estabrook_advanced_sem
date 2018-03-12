using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    public bool GameStart = false;

	public enum HealthMode
	{
		livesMode = 0,
		coldMeter =1
	}


	[Header("UI")]
	public Text DebugText;
	public HealthMode mode;

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
		
	}
	
	// Update is called once per frame
	void Update () {
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
}
