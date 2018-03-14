using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InControl;

public class CharSelectManager : MonoBehaviour {

	public enum player
	{
		P1 = 0,
		P2 = 1,
		P3 = 2,
		P4 = 3
	}

	//Sendable Player Variables
	string name;
	int team;
	public int pNum;
	Color pColor;

	#region PRIVATE VARS
	private Color32 TEAM1COLOR, TEAM2COLOR, highlightColor, readyColor;

	private InputDevice pControl;

	public List<Color32> Team1Colors;
	public List<Color32> Team2Colors;

	private int selectedItem = 0;
	private float delay = .15f;

	private int currentColor;

	private float timer;
	private bool canMove = true;

	#endregion

	public player Player;
	public bool isReady = false;

	#region EDITING VARS
	[SerializeField] public Image CharPanel, background;
	[SerializeField] public Text pName, pTeam, preset;

	[SerializeField] private List<Image> tiles;
	#endregion
	

	private void Awake()
	{
		pControl = InputManager.Devices[(int)Player];
	}

	void Start () {
		name = this.gameObject.name;

		pNum = (int)Player;
		TEAM1COLOR = new Color32(255, 159, 159, 255);
		TEAM2COLOR = new Color32(159, 176, 255, 255);
		readyColor = new Color32(23, 120, 145, 255);
		highlightColor = Color.cyan;

		Team1Colors.Add(new Color32(244, 152, 66, 255));
		Team1Colors.Add(new Color32(252, 61, 40, 255));

		Team2Colors.Add(new Color32(48, 165, 93, 255));
		Team2Colors.Add(new Color32(48, 125, 165, 255));

		HighLight(tiles[selectedItem]);
		
	}
	
	
	void Update () {
		if(team == 0)
		{
			pTeam.text = "Red Team";
			CharPanel.color = Team1Colors[currentColor];
			pColor = Team1Colors[currentColor];
			background.color = TEAM1COLOR;

		}
		else
		{
			pTeam.text = "Blue Team";
			CharPanel.color = Team2Colors[currentColor];
			pColor = Team2Colors[currentColor];
			background.color = TEAM2COLOR;

		}
		preset.text = "Preset " + (currentColor + 1);

		if(!canMove)
		{
			if(timer < delay)
			{
				timer += Time.deltaTime;

			}
			else
			{
				timer = 0;
				canMove = true;
			}
		}
		
		CheckInput();
	}

	void CheckInput()
	{
		if(canMove && !isReady)
		{
			if (pControl.LeftStickY > .5)
			{
				UnHighlight(tiles[selectedItem]);
				selectedItem--;
				if (selectedItem <= 0)
				{
					selectedItem = 0;
				}
				HighLight(tiles[selectedItem]);
				canMove = false;
				Debug.Log(selectedItem);
			}
			else if (pControl.LeftStickY < -.5)
			{
				UnHighlight(tiles[selectedItem]);
				selectedItem++;
				if (selectedItem >= tiles.Count)
				{
					selectedItem = tiles.Count - 1;
				}
				HighLight(tiles[selectedItem]);
				canMove = false;
				Debug.Log(selectedItem);

			}

		}
		if(pControl.Action1.WasPressed)
		{
			switch (selectedItem)
			{
				case 0:
					Debug.Log("Not done");
					break;
				case 1:
					SwitchTeam();
					break;
				case 2:
					SwitchColors();
					break;
				case 3:
					ToggleReady();
					break;
				default:
					Debug.LogError("TOO MANY TILES WHAT?");
					break;
			}

		}
		
	}

	void HighLight(Image i)
	{
		i.color = isReady ? readyColor : highlightColor;
	}

	void UnHighlight(Image i)
	{
		i.color = Color.white;
	}

	public void SwitchTeam()
	{
		if(team == 0)
		{
			team = 1;
		}
		else
		{
			team = 0;
		}
	}

	public void SwitchColors()
	{
		if(team == 0)
		{
			currentColor++;
			if (currentColor >= Team1Colors.Count)
			{
				currentColor = 0;
			}
		}
		else
		{
			currentColor++;
			if (currentColor >= Team2Colors.Count)
			{
				currentColor = 0;
			}

		}
	}

	public void ToggleReady()
	{
		if(isReady)
		{
			isReady = false;
			HighLight(tiles[selectedItem]);
			PlayerVariables.instance.Unready();
		}
		else
		{
			isReady = true;
			HighLight(tiles[selectedItem]);
			UploadInformation();
			PlayerVariables.instance.ReadyUp();

		}
	}

	void UploadInformation()
	{
		PlayerVariables.Player pInstance = new PlayerVariables.Player(name, team, pNum, pColor);

		PlayerVariables.instance.PlayerList[pNum] = pInstance;

	}


}
