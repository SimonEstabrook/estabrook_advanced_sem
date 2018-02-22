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

    private InputDevice pControl;

    //non viewable variables
    Rigidbody rb;
    GameObject tempObject;
    SnowBlockManager currentMan;

    GameObject blockPlace;


    [Header("Movement")] 
    [SerializeField] private int speed;
    public float movex;
    public float movez;
    public player whichPlayer;


    [Header("Debugs")]
    public GameObject currentBlock;
    [SerializeField] private bool isGrounded = true;

    [Header("Prefabs")]
    [SerializeField] private GameObject block;

    private void Awake()
    {
        pControl = InputManager.Devices[(int)whichPlayer];

    }

    private void Start()
    {
        blockPlace = transform.Find("PlaceObject").gameObject;
        transform.position += (Vector3.up * BlockManager.instance.GiveMinHeight());
		tempObject = null;
        rb = GetComponent<Rigidbody>();
    }

    int v = 0;

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
		if(pControl.Action3)
		{
			Debug.Log("Destroy");
			GetComponent<PlayerAbilities>().DestroyBlock(currentBlock, currentMan);
        }
        else if(pControl.Action2)
        {
            Debug.Log("Place");
            GetComponent<PlayerAbilities>().PlaceBlock(block, blockPlace);
        }
        else if(pControl.Action1)
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


        rb.velocity = new Vector3(movex * speed, v, movez *  speed);

        if(!isGrounded)
        {
            v = -8;
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
