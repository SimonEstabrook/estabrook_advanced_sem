using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour {

	public void Climb()
	{
		RaycastHit block;
		if (Physics.Raycast(transform.position, transform.forward, out block, 2, 1 << 8))
		{
			transform.position = new Vector3(block.transform.position.x, block.transform.position.y + 3.4f, block.transform.position.z);
            GetComponent<PlayerController>().isJumping = true;
		}
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
		if (blockInst != null)
		{
			BlockManager.instance.blocks.Add(blockInst);
		}
	}



	public void DestroyBlock(GameObject currentBlock, SnowBlockManager currentMan)
	{
		currentBlock.GetComponent<BoxCollider>().enabled = false;
		currentMan.destroyCube();
		currentMan.isDestroyed = true;

	}
}
