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
		SnowBrick = 3,
		Sled = 4
    }

    #region CONSTANTS
    private const float PICKUP_TIMER = 1.5f;
    private const float SNOWBALL_TIMER = 2f;
    private const float WALL_TIMER = 2.5f;

    private const int MAX_AMMO = 3;
	private const float FREEZE_MAX_TIME = 5f;
    #endregion

    #region Private Variables

    //non viewable variables
    private InputDevice pControl;
    private PlayerAbilities PA;
	Camera cam;
	[HideInInspector] public float health;
	private float MaxHealth;
	private int snowBallDamage;

    //snowblock variables
    private GameObject currentBlock;
    [HideInInspector] public GameObject blockPlace;
    GameObject tempObject;
    SnowBlockManager currentMan;

	//freezing variables
	float freezeTimer = 0;

    //movement
    private float movex;
    private float movey;
    private float movez;
    Rigidbody rb;
    private bool isGrounded = true;
    private int v = 0;
    private float TempSpeed = 0;

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
	public bool isAiming = false;
	public bool onStack = false;
	private bool isRunning = false;

    #endregion

    #region Public Variables

    [Header("Movement")] 
    [SerializeField] private float speed;
    public bool isJumping = false;
    public player whichPlayer;
	public Team team;
	public float launchNumber;

	[Header("Attributes")]
	public int ColdTotal = 2000;
	public int wallsAround = 0;
	public bool[] walls;
	bool isFreezing = false;
	bool isNearFire = false;
	public ParticleSystem freezeParticles;

    [Header("Inventory")]
    public int ammo;
    public List<GameObject> snowballPoints;
    public items currentItem = items.nothing;
    public List<GameObject> itemPrefabs;
    [SerializeField] GameObject currentObject;
    public GameObject heldPoint;
	[SerializeField] private GameObject BlockUI;
	[SerializeField] private GameObject ShootUI;
	[SerializeField] private GameObject currentUI;
	public int stackCount;


	[Header("UI")]
    [SerializeField] private Slider timeSlider;
	[SerializeField] private Slider healthSlider;
	[SerializeField] private Slider ColdSlider;

    [Header("Prefabs")]
    [SerializeField] private GameObject block;
    #endregion

    private void Awake()
    {
        pControl = InputManager.Devices[(int)whichPlayer];

    }

    private void Start()
    {
		walls = new bool[] { false, false, false, false };

        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        PA = GetComponent<PlayerAbilities>();
        TempSpeed = speed;
        blockPlace = transform.Find("PlaceObject").gameObject;
        transform.position += (Vector3.up * BlockManager.instance.GiveMinHeight());
		tempObject = null;
        rb = GetComponent<Rigidbody>();

		
		health = 5;
		snowBallDamage = 1;
		

		MaxHealth = healthSlider.maxValue = health;
		ColdSlider.maxValue = ColdTotal;
	}


	private void Update()
    {

		if(currentItem == items.Sled)
		{
			onStack = false;
		}else if(pickableItem != null && pickableItem.name.Contains("Stack"))
		{
			onStack = true;
		}

		wallsAround = 0;
		walls[0] = (Physics.Raycast(transform.position, Vector3.forward, 2, 1 << 8));
		walls[1] = (Physics.Raycast(transform.position, Vector3.right, 2, 1 << 8));
		walls[2] = (Physics.Raycast(transform.position, Vector3.back, 2, 1 << 8));
		walls[3] = (Physics.Raycast(transform.position, Vector3.left, 2, 1 << 8));

		for(int i = 0; i < walls.Length; i++)
		{
			if(walls[i] == true)
			{
				wallsAround++;
			}
		}

		if(health <= 0)
		{
			GameManager.instance.PlayerGone(team);
			this.gameObject.SetActive(false);
		}

		ColdSlider.value = ColdTotal;

		if (!isNearFire)
		{
			ColdTotal -= wallsAround;


			isFreezing = (ColdTotal <= 0);

			if (isFreezing)
			{
				if (freezeTimer < FREEZE_MAX_TIME)
				{
					freezeTimer += Time.deltaTime;
				}
				else
				{
					freezeTimer = 0;
					TakeDamage();
					freezeParticles.Play();
				}
			}

		}
		else
		{
			if(ColdTotal < ColdSlider.maxValue)
			{
				ColdTotal += 4;
			}
		}

		healthSlider.value = health;
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
		if(pControl.Action4.WasPressed && wait)
		{
			wait = false;
			isPickingUp = false;
			isCreating = false;
			isClimbing = false;
			isPlacing = false;
			timer = 0;
		}
        else if (((pControl.Action3.WasPressed && !wait) || isPickingUp) && currentItem == items.nothing)
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
        else if (((pControl.Action2.WasPressed && !wait) || isCreating) && currentItem == items.SnowPile)
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
				currentItem = PlayerController.items.Snowball;
				GiveObject((int)currentItem);
				wait = false;
                timer = 0;

            }
        }
        else if (((pControl.Action3.WasPressed && !wait) || isPlacing) && currentItem == items.SnowPile)
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
				currentItem = items.SnowBrick;
				GiveObject((int)currentItem);
                wait = false;
                timer = 0;

            }
        }
        else if(pControl.Action1.WasPressed && !wait && onStack)
        {
			if(pickableItem.GetComponent<StackManager>().count > 0)
			{
				pickableItem.GetComponent<StackManager>().RemoveSnowBall();
				currentItem = items.Snowball;
				GiveObject((int)currentItem);
			}
            
        }
		else if (pControl.Action4.IsPressed && isAiming)
		{
			isAiming = false;
		}
		else if (pControl.Action4.WasPressed && !wait && !isAiming)
        {
            PA.Drop();
        }
		else if((pControl.Action2.IsPressed) && currentItem == items.Snowball && !wait)
		{
			isAiming = true;
		}
        else if((pControl.Action2.WasReleased) && currentItem == items.Snowball && !wait && isAiming)
        {
            Debug.Log("Throw");
            PA.Throw();
			isAiming = false;
        }else if((pControl.Action3.WasPressed && !wait) && currentItem == items.SnowBrick)
		{
			PA.PlaceBlock(block, currentUI);

		}else if((pControl.Action1.IsPressed))
		{
			//speed = speed * 1.5f;
			isRunning = true;
		}else if((pControl.Action1.WasReleased))
		{
			//speed = TempSpeed;
			isRunning = false;
		}
		#endregion

		#region DEBUG_COMMANDS
		if (GameManager.instance.PlayerCheats)
		{


			if (pControl.DPadUp.WasPressed)
			{
				currentItem = items.Snowball;
				GiveObject((int)currentItem);
			}
			if (pControl.DPadDown.WasPressed)
			{
				currentItem = items.SnowPile;
				GiveObject((int)currentItem);
			}
			if (pControl.DPadLeft.WasPressed || pControl.DPadRight.WasPressed)
			{
				currentItem = items.nothing;
				GiveObject(0);
			}
		}
		#endregion


        isGrounded = (Physics.Raycast(transform.position, -Vector3.up, 1.1f));

        UpdateHUD();
        
		
    }
    
    public void GiveObject(int n)
    {
		if (currentUI != null)
		{
			//Destroy(currentUI.gameObject);
			currentUI.gameObject.SetActive(false);
		}
		ShootUI.SetActive(false);
		if (currentObject != null)
        {
            Destroy(currentObject.gameObject);
        }    
        if(n == 0)
        {
			if(currentItem == items.Snowball)
			{
				ammo--;
			}
            Destroy(currentObject.gameObject);
        }
        else
        {
            switch(n)
            {
                case 1://snow pile
                    currentObject = Instantiate(itemPrefabs[n], heldPoint.transform.position, itemPrefabs[n].transform.rotation);
                    break;
                case 2://snow ball
					currentObject = Instantiate(itemPrefabs[n], heldPoint.transform.position, Quaternion.identity);
					currentObject.GetComponent<SnowballManager>().enabled = false;
                    currentObject.GetComponent<Rigidbody>().isKinematic = true;
                    currentObject.GetComponent<TrailRenderer>().enabled = false;
                    ammo++;

					ShootUI.SetActive(true);
					break;
				case 3://snow block
					currentObject = Instantiate(itemPrefabs[n], heldPoint.transform.position, Quaternion.identity);
					currentObject.GetComponent<BoxCollider>().enabled = false;
					currentObject.GetComponent<Rigidbody>().isKinematic = true;

					currentUI = Instantiate(BlockUI, heldPoint.transform.position, Quaternion.identity);
					blockPlace.transform.parent = currentUI.transform;
					currentUI.GetComponent<BlockUIScript>().followPos = heldPoint.gameObject;
					break;
				case 4://snowball stack
					currentObject = Instantiate(itemPrefabs[n], heldPoint.transform.position, itemPrefabs[n].transform.rotation);
					currentObject.GetComponent<StackManager>().count = stackCount;
					//currentObject.transform.localScale = Vector3.one * 1f;
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
		if(!wait && !isAiming)
		{
			rb.velocity = new Vector3(movex * speed, -(movey * v), movez * speed);

		}
		else
		{
			rb.velocity = Vector3.zero;
		}

		if (!isGrounded)
		{
			if (!isJumping && !isRunning)
			{
				if (movey == 0)
				{
					movey = 1;
				}
				v = 25;
				speed = 5;

			}
			else if (!isRunning)
			{
				movey = 1;
				v = 15;
			}
			else
			{
			
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

	public void TakeDamage()
	{
		health -= snowBallDamage;
	}

    private void OnTriggerEnter(Collider other)
    {
		if(other.gameObject.name.Contains("Snowball"))
		{
			if(other.GetComponent<SnowballManager>().team != team && other.GetComponent<SnowballManager>().team != Team.None)
			{
				Vector3 dir = other.GetComponent<Rigidbody>().velocity.normalized * launchNumber;
				rb.AddForce(dir + (Vector3.up * launchNumber / 2));

			}

		}
		if (other.tag == "Pickup")
		{
			pickableItem = other.gameObject;
			if(pickableItem.name.Contains("Stack") && currentItem != items.Sled)
			{
				onStack = true;
			}
		}
		if(other.tag == "Block")
		{
			transform.position = other.transform.position + Vector3.up*1;
		}
		if(other.tag == "Fire")
		{
			isNearFire = true;
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
			onStack = false;
        }
		if (other.tag == "Fire")
		{
			isNearFire = false;
		}

	}






}
