using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField] private int speed;
    public float movex;
    public float movez;

    public enum player
    {
        P1 = 1,
        P2 = 2,
        P3 = 3,
        P4 = 4
    }

    public player whichPlayer;

    Rigidbody rb;

	public GameObject currentBlock;
	GameObject tempObject;
    SnowBlockManager currentMan;
    private void Start()
    {
        transform.position += (Vector3.up * BlockManager.instance.GiveMinHeight());
		tempObject = null;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(GameManager.instance.GameStart)
        {
            CheckInput();

        }
        //Debug.Log(playerState);
        RaycastHit snow;
		if (Physics.Raycast(transform.position, -Vector3.up, out snow, Mathf.Infinity))
		{
            if (snow.transform.gameObject.tag == "Block")
            {


                currentBlock = snow.transform.gameObject;

                currentBlock.GetComponent<SnowBlockManager>().Highlight();
                if (tempObject != currentBlock && tempObject != null)
                {
                    tempObject.GetComponent<SnowBlockManager>().UnHighlight();
                }
            }
		}
		tempObject = currentBlock;
        currentMan = currentBlock.GetComponent<SnowBlockManager>();
		if(Input.GetButtonDown(whichPlayer + "Action"))
		{
			Debug.Log("Destroy");
			currentMan.destroyCube();
            currentMan.isDestroyed = true;
		}
    }

    void FixedUpdate()
    {


        rb.velocity = new Vector3(movex * -1 * speed, 0, movez * -1 * speed);


        Vector3 nextDir = new Vector3(-movex, 0, -movez);
        if (nextDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(nextDir);

        }

    }


    void CheckInput()
    {
        movex = Input.GetAxis(whichPlayer + "LeftStickX");
        movez = Input.GetAxis(whichPlayer + "LeftStickY");

    }


}
