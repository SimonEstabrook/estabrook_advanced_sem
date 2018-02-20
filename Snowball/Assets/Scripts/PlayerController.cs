using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public enum player
    {
        P1 = 1,
        P2 = 2,
        P3 = 3,
        P4 = 4
    }
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
		if(Input.GetButtonDown(whichPlayer + "Action"))
		{
			Debug.Log("Destroy");
            currentBlock.GetComponent<BoxCollider>().enabled = false;
			currentMan.destroyCube();
            currentMan.isDestroyed = true;
        }
        else if(Input.GetButtonDown(whichPlayer + "Place"))
        {
            Debug.Log("Place");
            PlaceBlock();
        }
        else if(Input.GetButtonDown(whichPlayer + "Climb"))
        {
            Debug.Log("Climb");
            Climb();
        }

        isGrounded = (Physics.Raycast(transform.position, -Vector3.up, 1.1f));
        

        
    }

    void FixedUpdate()
    {


        rb.velocity = new Vector3(movex * -1 * speed, v, movez * -1 * speed);

        if(!isGrounded)
        {
            v = -8;
        }


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

    void PlaceBlock()
    {
        GameObject blockInst;
        RaycastHit blockRay;
        blockInst = Instantiate(block, blockPlace.transform.position, block.transform.rotation);
        blockInst.transform.position = new Vector3(Mathf.Round(blockInst.transform.position.x), transform.position.y, Mathf.Round(blockInst.transform.position.z));
        if (blockInst.transform.position.x % 2 != 0)
        {
            blockInst.transform.position -= Vector3.right;
        }
        if (blockInst.transform.position.z % 2 != 0)
        {
            blockInst.transform.position -= Vector3.forward;
        }


        if(Physics.Raycast(blockInst.transform.position, -Vector3.up, out blockRay, Mathf.Infinity, 1 << 8))
        {
            blockInst.transform.position = blockRay.transform.gameObject.GetComponent<SnowBlockManager>().sensors[2].transform.position;
        }
        else
        {
            blockInst.transform.position = new Vector3(blockInst.transform.position.x, 0, blockInst.transform.position.z);
        }
        

        //check for dublicates
        for (int i = 0; i < BlockManager.instance.blocks.Count; i++)
        {

            if (blockInst.transform.position == BlockManager.instance.blocks[i].transform.position && BlockManager.instance.blocks[i].GetComponent<SnowBlockManager>().isDestroyed == false)
            {
                Debug.Log("Destroyed");
                Destroy(blockInst);
                break;
            }
            
        }
        if(blockInst != null)
        {
            BlockManager.instance.blocks.Add(blockInst);
        }
    }

    void Climb()
    {
        RaycastHit block;
        if(Physics.Raycast(transform.position, transform.forward, out block, 2, 1 << 8))
        {
            transform.position = new Vector3(block.transform.position.x, block.transform.position.y + 3, block.transform.position.z);
        }
    }


}
