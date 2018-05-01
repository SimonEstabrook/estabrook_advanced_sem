﻿using System.Collections;
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
	private float iTimer = 0;

    //snowblock variables
    public GameObject currentBlock;
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
	private bool isHit = false;

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
	public float wallsAround = 0;
	public bool[] walls;
	bool isFreezing = false;
	public bool isNearFire = false;
	public bool onGround = false;
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

	public AudioSource grunt;

	[Header("UI")]
    [SerializeField] private Slider timeSlider;
	[SerializeField] private Slider healthSlider;
	[SerializeField] private Slider ColdSlider;

    [Header("Prefabs")]
    [SerializeField] private GameObject block;
    #endregion

    private void Awake()
    {
		if (InputManager.Devices.Count < (int)whichPlayer+ 1)
		{
			this.gameObject.SetActive(false);
			healthSlider.gameObject.transform.parent.gameObject.SetActive(false);
		}
		else
		{
			pControl = InputManager.Devices[(int)whichPlayer];

		}

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

		
		health = 100;
		snowBallDamage = 15;
		

		MaxHealth = healthSlider.maxValue = health;
		ColdSlider.maxValue = health;
		healthSlider.gameObject.SetActive(false);
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
				wallsAround += .03f;
			}
		}

		if(health <= 0)
		{
			GameManager.instance.PlayerGone(team);
			this.gameObject.SetActive(false);
		}

		ColdSlider.value = health;

		if (!isNearFire)
		{
			health -= wallsAround;


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
			if(health < ColdSlider.maxValue)
			{
				health += .2f;
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

                //currentBlock.GetComponent<SnowBlockManager>().Highlight();
                if (tempObject != currentBlock && tempObject != null)
                {
                    //tempObject.GetComponent<SnowBlockManager>().UnHighlight();
                }
            }

			if (snow.transform.gameObject.tag == "Ground")
			{
				onGround = true;

			}
			else
			{
				onGround = false;
			}
		}

        #region ACTIONS
        tempObject = currentBlock;
        currentMan = currentBlock.GetComponent<SnowBlockManager>();
		if((pControl.Action4.WasPressed || pControl.Action2.WasPressed) && wait)
		{
			wait = false;
			isPickingUp = false;
			isCreating = false;
			isClimbing = false;
			isPlacing = false;
			timer = 0;
		}
        else if ((((pControl.Action1.WasPressed || pControl.Action3.WasPressed) && !wait) || isPickingUp) && currentItem == items.nothing && !onGround)
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
        else if ((((pControl.Action3.WasPressed || pControl.RightTrigger.WasPressed)&& !wait) || isCreating) && currentItem == items.SnowPile)
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
        else if ((((pControl.LeftTrigger.WasPressed || pControl.Action1.WasPressed)&& !wait) || isPlacing) && currentItem == items.SnowPile)
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
        else if(pControl.RightBumper.WasPressed && !wait && onStack)
        {
			if(pickableItem.GetComponent<StackManager>().count > 0)
			{
				pickableItem.GetComponent<StackManager>().RemoveSnowBall();
				currentItem = items.Snowball;
				GiveObject((int)currentItem);
			}
            
        }
		else if(pControl.LeftBumper.WasPressed && !wait && onStack && currentItem == items.Snowball)
		{
			PA.Drop();
		}
		else if ((pControl.Action4.IsPressed || pControl.Action2.IsPressed) && isAiming)
		{
			isAiming = false;
		}
		else if ((pControl.Action4.WasPressed || pControl.Action2.WasPressed) && !wait && !isAiming)
        {
            PA.Drop();
        }
		else if((pControl.RightTrigger.IsPressed || pControl.Action3.IsPressed) && currentItem == items.Snowball && !wait && !GameManager.instance.isBuildMode)
		{
			isAiming = true;
		}
        else if((pControl.RightTrigger.WasReleased || pControl.Action3.WasReleased) && currentItem == items.Snowball && !wait && isAiming && !GameManager.instance.isBuildMode)
        {
            Debug.Log("Throw");
            PA.Throw();
			isAiming = false;
        }else if(((pControl.LeftTrigger.WasPressed || pControl.Action1.WasPressed) && !wait) && currentItem == items.SnowBrick)
		{
			PA.PlaceBlock(block, currentUI);

		}else if((pControl.LeftStick.IsPressed))
		{
			//speed = speed * 1.5f;
			isRunning = true;
		}else if((pControl.LeftStick.WasReleased))
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
        
		if(isHit)
		{
			if(iTimer <= 1)
			{
				iTimer += Time.deltaTime;
			}
			else
			{
				isHit = false;
			}
		}
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
					//currentObject.GetComponent<ParticleSystem>().Stop();
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
			if (!isJumping)
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

	public void TakeDamage()
	{
		if(!isHit)
		{
			health -= snowBallDamage;
			grunt.Play();
		}
		isHit = true;
	}

    private void OnTriggerEnter(Collider other)
    {
		if(other.gameObject.name.Contains("Snowball"))
		{
			if((other.GetComponent<SnowballManager>().team != team && other.GetComponent<SnowballManager>().team != Team.None)) 
			{
				Debug.Log("knockback");
				Vector3 dir = (transform.position - other.transform.position).normalized * launchNumber;
				rb.AddForce(dir);
				rb.AddForce(Vector3.up * launchNumber / 2);

			}

		}
		if(other.gameObject.tag == "Snowman")
		{
			//launchNumber *= 2;
			Debug.Log("knockback");
			Vector3 dir = (transform.position - other.transform.position).normalized * launchNumber;
			rb.AddForce(dir);
			rb.AddForce(Vector3.up * launchNumber / 2);
			//launchNumber /= 2;

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
			Debug.Log("Near Fire");
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
