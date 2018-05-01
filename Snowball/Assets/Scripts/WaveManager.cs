using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveManager : MonoBehaviour {

	private const int MAX_ENEMIES = 30;

	public static WaveManager instance = null;

	public int wave = 0;

	public List<GameObject> currentWave;

	int playerNumber = 1;

	int enemyCount;

	public List<Transform> spawnPoints;

	public GameObject snowman;

	bool firstwave = true;

	public float waveTimer = 15f;

	public GameObject PrepareText;

	public TextMeshProUGUI waveText;

	public  GameObject ResultsScreen;
	public TextMeshProUGUI resultsText;

	public FireManager fire;

	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		currentWave = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		if(currentWave.Count <= 0 && !GameManager.instance.isBuildMode)
		{
			if(firstwave)
			{
				OpenWave(1);
				firstwave = false;
			}
			else
			{
				if(waveTimer > 0)
				{
					waveTimer -= Time.deltaTime;
					PrepareText.SetActive(true);
					PrepareText.transform.GetChild(0).gameObject.SetActive(true);
					PrepareText.transform.GetChild(0).GetComponent<Text>().text = "Prepare for Battle!\n" + ((int)(waveTimer)).ToString();
				}
				else
				{
					PrepareText.SetActive(false);
					waveTimer = 15;
					OpenWave(1);
				}
			}

		}
		for(int i = 0; i < currentWave.Count; i++)
		{
			if(currentWave[i] == null)
			{
				currentWave.Remove(currentWave[i]);
			}
		}

		if(GameManager.instance.T1 <= 0 || fire.FireHealth <= 0)
		{
			GameOver();
			Debug.Log("Game Over");
		}

	}

	IEnumerator WaveTimer()
	{
		yield return new WaitForSeconds(15f);
		OpenWave(1);
	}


	void OpenWave(int pNum)
	{
		wave++;
		waveText.text = "Wave: " + wave;
		for(int i = 0; i < wave*pNum; i++)
		{
			if(currentWave.Count < MAX_ENEMIES)
			{
				GameObject snowmanInst = Instantiate(snowman, spawnPoints[Random.Range(0, spawnPoints.Count)].position, snowman.transform.rotation);
				currentWave.Add(snowmanInst);

			}
		}
	}

	void GameOver()
	{
		Time.timeScale = 0;
		GameManager.instance.GameOver = true;
		//ResultsScreen.SetActive(true);
		resultsText.text = "You lasted " + wave + " round(s).";
	}
}
