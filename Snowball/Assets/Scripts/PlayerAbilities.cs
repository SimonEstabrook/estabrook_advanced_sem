using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour {

    [SerializeField] private GameObject crunchSound;

    PlayerController PC;
    private void Start()
    {
        PC = GetComponent<PlayerController>();
    }

    public void Climb()
	{
		RaycastHit block;
		if (Physics.Raycast(transform.position, transform.forward, out block, 2, 1 << 8))
		{
			transform.position = new Vector3(block.transform.position.x, block.transform.position.y + 3.4f, block.transform.position.z);
            PC.isJumping = true;
            crunchSound.GetComponent<AudioSource>().Play();
		}
	}

    public void Drop()
    {
        if(PC.currentItem != PlayerController.items.nothing)
        {
			if (PC.currentItem == PlayerController.items.Snowball && PC.onStack)
			{
				if (PC.pickableItem.GetComponent<StackManager>().count < 10)
				{
					PC.pickableItem.GetComponent<StackManager>().AddSnowBall();
					PC.ammo--;
					PC.currentItem = PlayerController.items.nothing;
					PC.blockPlace.transform.parent = transform;
					PC.GiveObject(0);
				}
			}
			else
			{
				GameObject droppedItem = Instantiate(PC.itemPrefabs[(int)PC.currentItem], transform.position, PC.itemPrefabs[(int)PC.currentItem].transform.rotation);
				if (PC.currentItem == PlayerController.items.Snowball)
				{
					PC.ammo--;
					droppedItem.GetComponent<SnowballManager>().enabled = false;
				}
				else if (PC.currentItem == PlayerController.items.Sled)
				{
					droppedItem.GetComponent<StackManager>().count = PC.stackCount;
				}

				droppedItem.GetComponent<Rigidbody>().AddForce(Vector3.up * 1000);
				PC.currentItem = PlayerController.items.nothing;
				PC.GiveObject(0);
				crunchSound.GetComponent<AudioSource>().Play();
			}
        }
        else
        {
            if(PC.pickableItem != null)
            {
                if(PC.pickableItem.name.Contains("SnowPile"))
                {
                    PC.currentItem = PlayerController.items.SnowPile;
                }else if(PC.pickableItem.name.Contains("Snowball"))
                {
                    PC.currentItem = PlayerController.items.Snowball;
                }else if(PC.pickableItem.name.Contains("HeldBlock"))
				{
					Debug.Log("BLOCK CHECK");
					PC.currentItem = PlayerController.items.SnowBrick;
				}
				else if(PC.pickableItem.name.Contains("Stack"))
				{
					PC.currentItem = PlayerController.items.Sled;
					PC.stackCount = PC.pickableItem.GetComponent<StackManager>().count;
				}
                PC.GiveObject((int)PC.currentItem);
                Destroy(PC.pickableItem.gameObject);
                crunchSound.GetComponent<AudioSource>().Play();

            }
        }
    }


    public void Throw()
    {

        GameObject snowballInstance = Instantiate(PC.itemPrefabs[(int)PC.currentItem], PC.heldPoint.transform.position, transform.rotation);
		snowballInstance.GetComponent<SnowballManager>().dropped = false;
		snowballInstance.GetComponent<SnowballManager>().team = PC.team;
		snowballInstance.GetComponent<ParticleSystem>().Play();
		snowballInstance.GetComponent<AudioSource>().Play();
		Rigidbody instRB = snowballInstance.GetComponent<Rigidbody>();
        instRB.useGravity = false;
        instRB.AddForce(transform.forward * 1500);
        //instRB.AddForce(Vector3.up * 150);
        PC.currentItem = PlayerController.items.nothing;
        PC.GiveObject((int)PC.currentItem);
		PC.ammo--;

    }


    public void PlaceBlock(GameObject block, GameObject blockPlace)
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


		if (Physics.Raycast(blockInst.transform.position, -Vector3.up, out blockRay, Mathf.Infinity, 1 << 8))
		{
			blockInst.transform.position = blockRay.transform.gameObject.GetComponent<SnowBlockManager>().sensors[2].transform.position;
		}
		else
		{
			blockInst.transform.position = new Vector3(blockInst.transform.position.x, 0, blockInst.transform.position.z);
		}

        PC.currentItem = PlayerController.items.nothing;


        //check for duplicates
        for (int i = 0; i < BlockManager.instance.blocks.Count; i++)
		{

			if (blockInst.transform.position == BlockManager.instance.blocks[i].transform.position)
			{
				if(BlockManager.instance.blocks[i].GetComponent<SnowBlockManager>().isDestroyed == false)
				{
					Debug.Log("Destroyed");
					Destroy(blockInst);
					PC.currentItem = PlayerController.items.SnowPile;
					break;

				}
				else
				{
					BlockManager.instance.blocks[i].gameObject.SetActive(false) ;
				}
			}

		}
		if (blockInst != null)
		{
			BlockManager.instance.blocks.Add(blockInst);

        }
        crunchSound.GetComponent<AudioSource>().Play();
        PC.GiveObject((int)PC.currentItem);
		if(PC.currentItem == PlayerController.items.nothing)
		{
			PC.blockPlace.transform.parent = transform;
		}
    }


    public void DestroyBlock(GameObject currentBlock, SnowBlockManager currentMan)
	{
        currentBlock.GetComponent<BoxCollider>().enabled = false;
        PC.currentItem = PlayerController.items.SnowPile;
        currentMan.destroyCube();
        currentMan.isDestroyed = true;
        crunchSound.GetComponent<AudioSource>().Play();
        PC.GiveObject((int)PC.currentItem);

    }


}
