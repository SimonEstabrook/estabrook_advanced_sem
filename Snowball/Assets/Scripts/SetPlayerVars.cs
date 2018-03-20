using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPlayerVars : MonoBehaviour {

	private PlayerController PC;
	private MeshRenderer MR;

	public Text PlayerName;
	public Image HUDBackground;
	public GameObject HUD;

	private void Start()
	{
		PC = GetComponent<PlayerController>();
		MR = GetComponent<MeshRenderer>();
		if(PlayerVariables.instance.PlayerList[(int)PC.whichPlayer].GetActive())
		{
			//Set Team
			PC.team = (PlayerController.Team)PlayerVariables.instance.PlayerList[(int)PC.whichPlayer].GetTeam() + 1;
			//Change Name
			PlayerName.text = PlayerVariables.instance.PlayerList[(int)PC.whichPlayer].GetName();
			//Change Player Color
			MR.material.color = PlayerVariables.instance.PlayerList[(int)PC.whichPlayer].GetPColor();
			//Change HUD background color
			HUDBackground.color = PlayerVariables.instance.PlayerList[(int)PC.whichPlayer].GetPColor();
			//Add player to game manager
			GameManager.instance.AddPlayer(this.gameObject, PC.team);
		}
		else
		{
			this.gameObject.SetActive(false);
			HUD.SetActive(false);
		}
	}
}
