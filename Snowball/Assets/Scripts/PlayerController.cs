using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    public enum player
    {
        P1 = 0,
        P2 = 1,
        P3 = 2,
        P4 = 3
    }

	public enum Team
	{
		None = 0,
		Team1 = 1,
		Team2 = 2,
		Team3 = 3,
		Team4 = 4
	}


    public enum items
    {
        nothing = 0,
        SnowPile = 1,
        Snowball = 2,
    }

    #region CONSTANTS
    private const float PICKUP_TIMER = 1.5f;
    private const float SNOWBALL_TIMER = 2f;
    private const float WALL_TIMER = 2.5f;
    private const float CLIMB_TIMER = .5f;

    private const int MAX_AMMO = 3;
    #endregion

    #region Private Variables

    //non viewable variables
    private InputDevice pControl;
    private PlayerAbilities PA;
    Camera cam;

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

    //items
    [HideInInspector] public GameObject pickableItem;

    //actions
    float timer = 0.0f;
    private bool wait = false;
    private bool isPickingUp = false;
    private bool isCreating = false;
    private bool isPlacing = false;
    private bool isClimbing = false;
    private bool isWalking = false;

    #endregion

    #region Public Variables

    [Header("Movement")] 
    [SerializeField] private int speed;
    public bool isJumping = false;
    public player whichPlayer;
	public Team team;

    [Header("Inventory")]
    public int ammo;
    public List<GameObject> snowballPoints;
    public items currentItem = items.nothing;
    public List<GameObject> itemPrefabs;
    [SerializeField] GameObject currentObject;
    public GameObject heldPoint;

    [Header("UI")]
    [SerializeField] private Slider timeSlider;

    [Header("Prefabs")]
    [SerializeField] private GameObject block;
    #endregion

    private void Awake()
    {
        pControl = InputManager.Devices[(int)whichPlayer];

    }

    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        PA = GetComponent<PlayerAbilities>();
        TempSpeed = speed;
        blockPlace = transform.Find("PlaceObject").gameObject;
        transform.position += (Vector3.up * BlockManager.instance.GiveMinHeight());
		tempObject = null;
        rb = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        isWalking = (GetComponent<Rigidbody>().velocity.x != 0 || GetComponent<Rigidbody>().velocity.z != 0);

        if (isWalking && !GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().Play();
        }
        if(!isWalking && GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().Stop();
        }

        timeSlider.gameObject.transform.position = cam.WorldToScreenPoint(transform.position + (Vector3.up*2));
        if(GameManager.instance.GameStart && !wait)
        {
            CheckInput();

        }

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

        #region ACTIONS
        tempObject = currentBlock;
        currentMan = currentBlock.GetComponent<SnowBlockManager>();
        if (((pControl.Action3.WasPressed && !wait) || isPickingUp) && currentItem == items.nothing)
        {
            if (timer < PICKUP_TIMER)
            {
                isPickingUp = true;
                speed = 0;
                rb.velocity = Vector3.zero;
                timer += Time.deltaTime;
                wait = true;
            }
            else
            {
                isPickingUp = false;
                speed = TempSpeed;
                Debug.Log("Destroy");
                PA.DestroyBlock(currentBlock, currentMan);
                wait = false;
                timer = 0;

            }
        }
        else if (((pControl.Action3.WasPressed && !wait) || isCreating) && currentItem == items.SnowPile)
        {
            if (timer < SNOWBALL_TIMER)
            {
                isCreating = true;
                rb.velocity = Vector3.zero;
                speed = 0;
                timer += Time.deltaTime;
                wait = true;
            }
            else
            {
                isCreating = false;
                speed = TempSpeed;
                Debug.Log("Create");
                PA.Craft();
                wait = false;
                timer = 0;

            }
        }
        else if (((pControl.Action2.WasPressed && !wait) || isPlacing) && currentItem == items.SnowPile)
        {
            if (timer < WALL_TIMER)
            {
                isPlacing = true;
                rb.velocity = Vector3.zero;
                speed = 0;
                timer += Time.deltaTime;
                wait = true;
            }
            else
            {
                isPlacing = false;
                speed = TempSpeed;
                Debug.Log("Place");
                PA.PlaceBlock(block, blockPlace);
                wait = false;
                timer = 0;

            }
        }
        else if(pControl.Action1.WasPressed && !wait)
        {
            Debug.Log("Climb");
            PA.Climb();
            
        }
        else if (pControl.Action4.WasPressed && !wait)
        {
            PA.Drop();
        }
        else if((pControl.RightBumper.WasPressed || pControl.LeftBumper.WasPressed) && currentItem == items.Snowball && !wait)
        {
            Debug.Log("Throw");
            PA.Throw();
        }
		#endregion

		#region DEBUG_COMMANDS
		if(pControl.DPadUp.WasPressed)
		{
			currentItem = items.Snowball;
			GiveObject((int)currentItem);
		}
		if(pControl.DPadDown.WasPressed)
		{
			currentItem = items.SnowPile;
			GiveObject((int)currentItem);
		}
		if(pControl.DPadLeft.WasPressed || pControl.DPadRight.WasPressed)
		{
			currentItem = items.nothing;
			GiveObject(0);
			ammo--;
		}
		#endregion

		if (pControl.Command.IsPressed)
        {
            Application.Quit();
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }

        isGrounded = (Physics.Raycast(transform.position, -Vector3.up, 1.1f));

        UpdateHUD();
        
    }
    
    public void GiveObject(int n)
    {
        if(currentObject != null)
        {
            Destroy(currentObject.gameObject);
        }    
        if(n == 0)
        {
            Destroy(currentObject.gameObject);
        }
        else
        {
            switch(n)
            {
                case 1:
                    currentObject = Instantiate(itemPrefabs[n], heldPoint.transform.position, itemPrefabs[n].transform.rotation);
                    break;
                case 2:
                    currentObject = Instantiate(itemPrefabs[n], snowballPoints[ammo].transform.position, Quaternion.identity);
                    currentObject.GetComponent<Rigidbody>().isKinematic = true;
                    currentObject.GetComponent<TrailRenderer>().enabled = false;
                    ammo++;
                    break;
                default:
                    Debug.LogError("Incorrect item id");
                    break;

            }
            currentObject.transform.parent = this.gameObject.transform;
            if (currentObject.GetComponent<Rigidbody>() == true)
            {
                currentObject.GetComponent<Rigidbody>().isKinematic = true;

            }

        }
    }

    void UpdateHUD()
    {
        timeSlider.value = timer;
        if (isPickingUp)
        {
            timeSlider.gameObject.SetActive(true);
            timeSlider.maxValue = PICKUP_TIMER;
        }
        else if(isPlacing)
        {
            timeSlider.gameObject.SetActive(true);
            timeSlider.maxValue = WALL_TIMER;
        }
        else if(isCreating)
        {
            timeSlider.gameObject.SetActive(true);
            timeSlider.maxValue = SNOWBALL_TIMER;
        }
        else
        {
            timeSlider.gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
		if(!wait)
		{
			rb.velocity = new Vector3(movex * speed, -(movey * v), movez * speed);

		}
		else
		{
			rb.velocity = Vector3.zero;
		}

		if (!isGrounded)
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

    private void OnTriggerEnter(Collider other)
    {
		if (other.tag == "Pickup")
		{
			pickableItem = other.gameObject;
		}
		if(other.tag == "Block")
		{

			transform.position = other.transform.position + Vector3.up*1;
		}
    }
	private void OnTriggerStay(Collider other)
	{
		if(other.tag == "Block")
		{
			transform.position = other.transform.position + Vector3.up * 1;

		}
	}
	private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Pickup")
        {
            pickableItem = null;
        }
    }






}
