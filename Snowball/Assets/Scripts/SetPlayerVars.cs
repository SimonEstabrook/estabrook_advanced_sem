using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPlayerVars : MonoBehaviour {

	private PlayerController PC;
	private MeshRenderer MR;

	public Text PlayerName;
	public Image HUDBackground;

	private void Start()
	{
		PC = GetComponent<PlayerController>();
		MR = GetComponent<MeshRenderer>();
		if(PlayerVariables.instance.PlayerList[(int)PC.whichPlayer].GetActive())
		{
			PC.team = (PlayerController.Team)PlayerVariables.instance.PlayerList[(int)PC.whichPlayer].GetTeam() + 1;
			PlayerName.text = PlayerVariables.instance.PlayerList[(int)PC.whichPlayer].GetName();
			MR.material.color = PlayerVariables.instance.PlayerList[(int)PC.whichPlayer].GetPColor();

		}
		else
		{
			this.gameObject.SetActive(false);
		}
	}
}
