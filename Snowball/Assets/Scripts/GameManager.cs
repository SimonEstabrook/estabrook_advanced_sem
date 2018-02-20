using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    public bool GameStart = false;

    [Header("Debugs")]
    public bool seeNodes = false;
    public bool GodMode = false;

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
	}

    void checkDebug()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("SEE NODES");
            seeNodes = toggle(seeNodes);
        }
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
