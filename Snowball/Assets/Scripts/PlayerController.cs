using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class PlayerController : MonoBehaviour {
    public enum player
    {
        P1 = 0,
        P2 = 1,
        P3 = 2,
        P4 = 3
    }

    #region Private Variables

    //non viewable variables
    private InputDevice pControl;

    //snowblock variables
    private GameObject currentBlock;
    GameObject blockPlace;
    GameObject tempObject;
    SnowBlockManager currentMan;

    //movement
    private float movex;
    private float movey;
    private float movez;
    Rigidbody rb;
    private bool isGrounded = true;
    private int v = 0;
    private int TempSpeed = 0;
    #endregion


    [Header("Movement")] 
    [SerializeField] private int speed;
    public bool isJumping = false;
    public player whichPlayer;

    [Header("Prefabs")]
    [SerializeField] private GameObject block;

    private void Awake()
    {
        pControl = InputManager.Devices[(int)whichPlayer];

    }

    private void Start()
    {
        TempSpeed = speed;
        blockPlace = transform.Find("PlaceObject").gameObject;
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
		if(pControl.Action3.WasPressed)
		{
			Debug.Log("Destroy");
			GetComponent<PlayerAbilities>().DestroyBlock(currentBlock, currentMan);
        }
        else if(pControl.Action2.WasPressed)
        {
            Debug.Log("Place");
            GetComponent<PlayerAbilities>().PlaceBlock(block, blockPlace);
        }
        else if(pControl.Action1.WasPressed)
        {
            Debug.Log("Climb");
            GetComponent<PlayerAbilities>().Climb();
        }
        if(pControl.Command.IsPressed)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }

        isGrounded = (Physics.Raycast(transform.position, -Vector3.up, 1.1f));
        

        
    }

    void FixedUpdate()
    {


        rb.velocity = new Vector3(movex * speed, -(movey * v), movez *  speed);
        
        if(!isGrounded)
        {
            if(!isJumping)
            {
                if (movey == 0)
                {
                    movey = 1;
                }
                v = 25;
                speed = 5;

            }
            else
            {
                movey = 1;
                v = 15;
            }
        }
        else
        {
            speed = TempSpeed;
            v = 0;
        }


        Vector3 nextDir = new Vector3(movex, 0, movez);
        if (nextDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(nextDir);

        }

    }


    void CheckInput()
    {
        movex = pControl.LeftStickX;
        movez = pControl.LeftStickY;

    }

    

   


}
