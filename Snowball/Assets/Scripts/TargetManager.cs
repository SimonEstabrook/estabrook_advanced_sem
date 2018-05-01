using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.UI;


[RequireComponent(typeof(AIDestinationSetter))]
[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(AIPath))]



public class TargetManager : MonoBehaviour
{

	//ai state machine used to control what each ai agent is doing 

	Camera cam;

    [SerializeField] public float radius = 20;
    IAstarAI ai;
    [SerializeField] public State state;
	public Transform target;

    Rigidbody rb1;

	public GameObject fire;

	Collider[] players;

	public bool hasTarget = false;

	public GameObject snowPile;

	public int health = 5;

	float normalSpeed;

	public Slider timer;

	Slider thisSlider;

	public bool isClimbing = false;
	float climbTimer = 3f;

	public GameObject block;

    void Start()
    {
		fire = GameObject.Find("FirePlace");
		cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();


		thisSlider = Instantiate(timer.gameObject, GameObject.Find("Canvas").transform).GetComponent<Slider>();
		thisSlider.maxValue = climbTimer;

		thisSlider.gameObject.SetActive(false);
		ai = GetComponent<IAstarAI>();
		StartCoroutine(FSM());
		normalSpeed = GetComponent<AIPath>().maxSpeed;
    }
    private void Update()
    {
		if(transform.position.y < -1)
		{
			transform.position = new Vector3(transform.position.x, -1, transform.position.z);
		}

		thisSlider.gameObject.transform.position = cam.WorldToScreenPoint(transform.position + (Vector3.up * 2));
		//IdleWandering();
		if (state != State.CHASE)
		{
			players = Physics.OverlapSphere(transform.position, 8f);
			for (int i = 0; i < players.Length; i++)
			{
				if (players[i].GetComponent<PlayerController>())
				{
					state = State.CHASE;
					target = players[i].gameObject.transform;
				}
			}

		}
		else
		{
			players = Physics.OverlapSphere(transform.position, 8f);
			for (int i = 0; i < players.Length; i++)
			{
				if (players[i].GetComponent<FireManager>())
				{
					state = State.ATTACK;
					target = players[i].gameObject.transform;
				}
			}


		}
		ai.destination = target.position;

		if(isClimbing)
		{

			thisSlider.gameObject.SetActive(true);

			if(climbTimer > 0)
			{
				climbTimer -= Time.deltaTime;
				thisSlider.value = climbTimer;
			}
			else
			{
				climbTimer = 3f;
				isClimbing = false;
				transform.position = new Vector3(block.transform.position.x, transform.position.y + 6, block.transform.position.z);

				GetComponent<AIPath>().maxSpeed = normalSpeed;
				thisSlider.gameObject.SetActive(false);

			}
		}

	}

	Vector3 PickRandomPoint()
    {
        var point = Random.insideUnitSphere * radius;
        point.y = 0;
        point += ai.position;
        return point;
    }
    public enum State
    {
        WANDER,
        CHASE,
        ATTACK
        

    }

    IEnumerator FSM()
    {
        
            switch (state)
            {
					 case State.WANDER:
                    {
                        IdleWandering();
                        break;
                    }
					case State.ATTACK:
					{
						Attack();
						break;
					}
            }
            yield return null;
       
    }

	void Attack()
	{
		target = fire.gameObject.transform;
	}


    void IdleWandering()
    {
        // Update the destination of the AI if
        // the AI is not already calculating a path and
        // the ai has reached the end of the path or it has no path at all
        ai.maxSpeed = normalSpeed;
        if (!ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath))
        {
            ai.destination = PickRandomPoint();
            ai.SearchPath();
        }
		//StartCoroutine(FSM());
    }


    void Evade()
    {
        
        FleePath fp = FleePath.Construct(transform.position, target.position, 1);

        // Get the Seeker component which must be attached to this GameObject
        Seeker seeker = GetComponent<Seeker>();
        // Start the path and return the result to MyCompleteFunction (which is a function you have to define, the name can of course be changed)
        seeker.StartPath(fp);

        state = TargetManager.State.CHASE;

    }

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Block")
		{
			GetComponent<AIPath>().maxSpeed = 0;

			isClimbing = true;

			block = other.gameObject;
		}
		else if(other.tag == "FireHitbox")
		{
			other.GetComponent<FireManager>().LoseHealth();
			HitSnowman();

		}
		else if(other.tag == "Player")
		{
			other.GetComponent<PlayerController>().TakeDamage();
			StartCoroutine(stun());
			Vector3 dir = other.GetComponent<Rigidbody>().velocity.normalized * 50;
			GetComponent<Rigidbody>().AddForce(dir + (Vector3.up * 50 / 2));

			//KillSnowman();
		}

		if (other.gameObject.GetComponent<SnowballManager>())
		{
			if (other.GetComponent<SnowballManager>().team != PlayerController.Team.None)
			{
				HitSnowman();
				other.GetComponent<SnowballManager>().KillSnowball();
			}
		}

	}


	IEnumerator stun()
	{

		Debug.Log("Stun");
		GetComponent<AIPath>().maxSpeed = 0;
		yield return new WaitForSeconds(1.5f);
		GetComponent<AIPath>().maxSpeed = normalSpeed;

	}

	void HitSnowman()
	{
			health--;
			if (health <= 0)
			{
				KillSnowman();
			}

		
	}

	void KillSnowman()
	{
		Instantiate(snowPile, transform.position, snowPile.transform.rotation);
		Destroy(this.gameObject);

	}
}
