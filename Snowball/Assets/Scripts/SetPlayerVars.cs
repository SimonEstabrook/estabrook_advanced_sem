using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPlayerVars : MonoBehaviour {

	private PlayerController PC;
	private MeshRenderer MR;

	public Text PlayerName;


	private void Start()
	{
		PC = GetComponent<PlayerController>();
		MR = GetComponent<MeshRenderer>();

		PC.team = (PlayerController.Team)PlayerVariables.instance.PlayerList[(int)PC.whichPlayer].GetTeam();
		PlayerName.text = PlayerVariables.instance.PlayerList[(int)PC.whichPlayer].GetName();
		MR.material.color = PlayerVariables.instance.PlayerList[(int)PC.whichPlayer].GetPColor();
	}
}
