using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using InControl;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour {


	public List<TextMeshProUGUI> Buttons;

	int index = 0;

	private Color32 HighLightColor;

	private float delay = .15f;


	private float timer;
	public bool canMove = true;

	private void Start()
	{
		Time.timeScale = 1;
		HighLightColor = new Color32(66, 244, 229, 255);
		Debug.Log(InputManager.Devices.Count);
	}

	void Update () {
		if(index < 0)
		{
			index = 0;
		}
		if(index > Buttons.Count - 1)
		{
			index = Buttons.Count - 1;
		}

		for(int i = 0; i < Buttons.Count; i++)
		{
			if(i == index)
			{
				Buttons[i].color = HighLightColor;

			}
			else
			{
				Buttons[i].color = Color.black;
			}
		}
		
		for (int i = 0; i < InputManager.Devices.Count; i++)
		{
			if((InputManager.Devices[i].LeftStickY > .5  || InputManager.Devices[i].DPadUp.WasPressed) && canMove)
			{
				index--;
				canMove = false;
			}
			if((InputManager.Devices[i].LeftStickY < -.5 || InputManager.Devices[i].DPadDown.WasPressed)&& canMove)
			{
				index++;
				canMove = false;
			}

			if(InputManager.Devices[i].Action1.WasPressed)
			{
				Buttons[index].transform.parent.GetComponent<Button>().onClick.Invoke();
			}
		}

		if (!canMove)
		{
			if (timer < delay)
			{
				timer += Time.deltaTime;

			}
			else
			{
				timer = 0;
				canMove = true;
			}
		}


	}
}
