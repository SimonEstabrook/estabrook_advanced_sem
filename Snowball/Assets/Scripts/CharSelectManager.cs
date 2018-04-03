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

	public enum Screen
	{
		BASE_MENU = 0,
		NAME_SELECT = 1,
		NAME_TYPE = 2
	}


	//Sendable Player Variables
	public string name;
	int team;
	public int pNum;
	Color pColor;

	public Screen whichScreen;
	public GameObject NameScreen;
	public GameObject typeScreen;

	#region PRIVATE VARS
	private Color32 TEAM1COLOR, TEAM2COLOR, highlightColor, readyColor;

	private InputDevice pControl;

	public List<Color32> Team1Colors;
	public List<Color32> Team2Colors;

	public  int selectedItem = 0;
	public int nameSelectCount = 0;
	public int nameTypeCount = 0;
	private float delay = .15f;

	private int currentColor;

	private float timer;
	public bool canMove = true;

	#endregion

	public player Player;
	public bool isReady = false;
	public bool isActivated = false;

	#region EDITING VARS
	[SerializeField] public Image CharPanel, background;
	[SerializeField] public Text pName, pTeam, preset;
	[SerializeField] private GameObject ActivatePanel;

	[SerializeField] private List<Image> tiles;
	[SerializeField] private List<Image> nameSelectTiles;
	[SerializeField] private List<Image> nameTypeTiles;
	#endregion
	

	private void Awake()
	{
		pControl = InputManager.Devices[(int)Player];
		Time.timeScale = 1;
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
		HighLight(nameSelectTiles[nameSelectCount]);
		HighLight(nameTypeTiles[nameTypeCount]);
		
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
		
		if(isActivated)
		{
			ActivatePanel.SetActive(false);
			this.gameObject.tag = "Active";
		}
		else
		{
			ActivatePanel.SetActive(true);
			this.gameObject.tag = "Inactive";
		}
		
		CheckInput();
		if(whichScreen == Screen.NAME_SELECT)
		{
			NameScreen.SetActive(true);
			typeScreen.SetActive(false);
		}
		else if(whichScreen == Screen.BASE_MENU)
		{
			NameScreen.SetActive(false);
		}
		else
		{
			typeScreen.SetActive(true);
		}
		
	}

	void CheckInput()
	{
		if (isActivated)
		{ 
			if(pControl.Action2.WasPressed)
			{
				if(isActivated)
				{
					switch (whichScreen)
					{
						case Screen.BASE_MENU:
							isActivated = false;
							break;
						case Screen.NAME_SELECT:
							whichScreen = Screen.BASE_MENU;
							break;
						case Screen.NAME_TYPE:
							whichScreen = Screen.NAME_SELECT;
							break;
					}

				}
				

			}
			switch (whichScreen)
			{
				case Screen.BASE_MENU:
					MainSelectScreen();
					break;
				case Screen.NAME_SELECT:
					NameSelectScreen();
					break;
				case Screen.NAME_TYPE:
					NameTypeScreen();
					break;
			}


		}
		
		else
		{
			if (pControl.Action1.WasPressed)
			{
				isActivated = true;
			}
			if(pControl.Action2.WasPressed)
			{
				transform.parent.gameObject.GetComponent<MenuManager>().Menu();
				
			}
		}
	}

	void NameTypeScreen()
	{
		if (canMove && !isReady)
		{
			if (pControl.LeftStickX < -.5)
			{

				Debug.Log("Move Up");
				UnHighlight(nameTypeTiles[nameTypeCount]);
				nameTypeCount--;
				if (nameTypeCount <= 0)
				{
					nameTypeCount = 0;
				}
				HighLight(nameTypeTiles[nameTypeCount]);
				canMove = false;
				Debug.Log(nameTypeCount);
			}
			else if (pControl.LeftStickX > .5)
			{
				Debug.Log("Move Down");
				UnHighlight(nameTypeTiles[nameTypeCount]);
				nameTypeCount++;
				if (nameTypeCount >= nameTypeTiles.Count)
				{
					nameTypeCount = nameTypeTiles.Count - 1;
				}
				HighLight(nameTypeTiles[nameTypeCount]);
				canMove = false;
				Debug.Log(nameTypeCount);

			}
			

		}
		if (pControl.Action1.WasPressed)
		{
			switch (nameSelectCount)
			{
				case 0:
					Debug.Log("Not done");
					break;
				case 1:
					whichScreen = Screen.NAME_TYPE;
					typeScreen.SetActive(true);
					break;
				case 2:
					whichScreen = Screen.BASE_MENU;
					break;
				default:
					Debug.LogError("TOO MANY TILES WHAT?");
					break;
			}

		}
	}

	void NameSelectScreen()
	{
		if (canMove && !isReady)
		{
			if (pControl.LeftStickY > .5)
			{
				Debug.Log("Move Up");
				UnHighlight(nameSelectTiles[nameSelectCount]);
				nameSelectCount--;
				if (nameSelectCount <= 0)
				{
					nameSelectCount = 0;
				}
				HighLight(nameSelectTiles[nameSelectCount]);
				canMove = false;
				Debug.Log(nameSelectCount);
			}
			else if (pControl.LeftStickY < -.5)
			{
				Debug.Log("Move Down");
				UnHighlight(nameSelectTiles[nameSelectCount]);
				nameSelectCount++;
				if (nameSelectCount >= nameSelectTiles.Count)
				{
					nameSelectCount = nameSelectTiles.Count - 1;
				}
				HighLight(nameSelectTiles[nameSelectCount]);
				canMove = false;
				Debug.Log(nameSelectCount);

			}
			if (pControl.LeftStickX > .5)
			{
				if (nameSelectCount == 0)
				{
					Debug.Log("Move Right");
					UnHighlight(nameSelectTiles[nameSelectCount]);
					nameSelectCount = 1;
					HighLight(nameSelectTiles[nameSelectCount]);
					canMove = false;

				}
			}
			if(pControl.LeftStickX < -.5)
			{
				if(nameSelectCount != 0)
				{
					Debug.Log("Move Left");
					UnHighlight(nameSelectTiles[nameSelectCount]);
					nameSelectCount = 0;
					HighLight(nameSelectTiles[nameSelectCount]);
					canMove = false;

				}
			}

		}
		if (pControl.Action1.WasPressed)
		{
			switch (nameSelectCount)
			{
				case 0:
					Debug.Log("Not done");
					break;
				case 1:
					whichScreen = Screen.NAME_TYPE;
					typeScreen.SetActive(true);
					break;
				case 2:
					whichScreen = Screen.BASE_MENU;
					break;
				default:
					Debug.LogError("TOO MANY TILES WHAT?");
					break;
			}

		}
	}

	void MainSelectScreen()
	{
		if (canMove && !isReady)
		{
			if (pControl.LeftStickY > .5)
			{
				Debug.Log("Move Up");
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
				Debug.Log("Move Down");
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
		if (pControl.Action1.WasPressed)
		{
			switch (selectedItem)
			{
				case 0:
					Debug.Log("Not done");
					//whichScreen = Screen.NAME_SELECT;
					//NameScreen.SetActive(true);
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
		PlayerVariables.Player pInstance = new PlayerVariables.Player(name, team, pNum, pColor, isActivated);

		PlayerVariables.instance.PlayerList[pNum] = pInstance;

	}



}
