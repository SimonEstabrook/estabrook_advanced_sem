using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackManager : MonoBehaviour {

	public List<GameObject> SnowBallStack;

	public int count = 0;

	void Start () {
		
	}
	
	
	void Update () {
		RefreshStack();

	}

	public void AddSnowBall()
	{
		if(count < SnowBallStack.Count)
		{
			count++;

		}
	}

	public void RemoveSnowBall()
	{
		if (count > 0)
		{
			count--;

		}
	}

	public void RefreshStack()
	{
		for (int i = 0; i < SnowBallStack.Count; i++)
		{
			if (i < count)
			{
				SnowBallStack[i].SetActive(true);
			}
			else
			{
				SnowBallStack[i].SetActive(false);
			}
		}

	}
}
