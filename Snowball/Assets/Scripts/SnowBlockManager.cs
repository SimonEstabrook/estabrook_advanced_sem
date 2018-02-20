using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBlockManager : MonoBehaviour {

    public GameObject zP, zN, xP, xN;

    [SerializeField] private List<GameObject> sensors;

    public GameObject baseCube;

    private Color startcolor;

    public bool isDestroyed = false;
    public bool scrolledOver = false;

    private void Start()
    {
        startcolor = Color.white;
        //GetComponent<MeshRenderer>().material.color = new Color(255, 255, 255, .5f);
    }

    private void Update()
    {
        if(!GameManager.instance.seeNodes)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            GetComponent<MeshRenderer>().enabled = true;
        }

        if (Input.GetMouseButtonDown(0) && scrolledOver)
        {
            if(isDestroyed == false)
            {
                Debug.Log("Destroy" + this.name);
                isDestroyed = true;
                GetComponent<BoxCollider>().enabled = false;
                GetComponent<SphereCollider>().enabled = true;
                destroyCube();

            }
            
        }
        else if(Input.GetMouseButtonDown(1) && scrolledOver)
        {
            if (isDestroyed == true) ;
            {
                Debug.Log("Rebuild" + this.name);
                isDestroyed = false;
                GetComponent<BoxCollider>().enabled = true;
                GetComponent<SphereCollider>().enabled = false;

                BuildSnow();

            }
        }
    }

	public void Highlight()
	{
		GetComponent<MeshRenderer>().material.color = Color.yellow;
		for (int i = 0; i < transform.childCount; i++)
		{
			if (transform.GetChild(i).GetComponent<MeshRenderer>() != null)
			{
				transform.GetChild(i).GetComponent<MeshRenderer>().material.color = Color.yellow;
			}


		}
	}

	public void UnHighlight()
	{
		GetComponent<MeshRenderer>().material.color = Color.white;
		for (int i = 0; i < transform.childCount; i++)
		{
			if (transform.GetChild(i).GetComponent<MeshRenderer>() != null)
			{
				transform.GetChild(i).GetComponent<MeshRenderer>().material.color = startcolor;
			}
		}
	}

    public void destroyCube()
    {
        baseCube.SetActive(false);
        
        for(int i = 0; i < BlockManager.instance.blocks.Count; i++)
        {
            if (sensors[2].transform.position != BlockManager.instance.blocks[i].transform.position || (BlockManager.instance.blocks[i].transform.position == sensors[2].transform.position && BlockManager.instance.blocks[i].GetComponent<SnowBlockManager>().isDestroyed))
            {


                if (BlockManager.instance.blocks[i].transform.position == sensors[2].transform.position)
                {
                    if (BlockManager.instance.blocks[i].GetComponent<SnowBlockManager>().isDestroyed)
                    {
                        BlockManager.instance.blocks[i].GetComponent<SnowBlockManager>().baseCube.SetActive(false);
                        BlockManager.instance.blocks[i].GetComponent<SnowBlockManager>().isDestroyed = true;
                        BlockManager.instance.blocks[i].GetComponent<SnowBlockManager>().zP.SetActive(false);
                        BlockManager.instance.blocks[i].GetComponent<SnowBlockManager>().zN.SetActive(false);
                        BlockManager.instance.blocks[i].GetComponent<SnowBlockManager>().xP.SetActive(false);
                        BlockManager.instance.blocks[i].GetComponent<SnowBlockManager>().xN.SetActive(false);

                    }
                    else
                    {

                    }


                }
                else if (BlockManager.instance.blocks[i].transform.position == sensors[0].transform.position)
                {
                    if (BlockManager.instance.blocks[i].GetComponent<SnowBlockManager>().isDestroyed)
                    {
                        BlockManager.instance.blocks[i].GetComponent<SnowBlockManager>().xN.SetActive(false);
                    }
                    else
                    {
                        xP.SetActive(true);
                    }
                }
                else if (BlockManager.instance.blocks[i].transform.position == sensors[1].transform.position)
                {
                    if (BlockManager.instance.blocks[i].GetComponent<SnowBlockManager>().isDestroyed)
                    {
                        BlockManager.instance.blocks[i].GetComponent<SnowBlockManager>().xP.SetActive(false);
                    }
                    else
                    {
                        xN.SetActive(true);
                    }
                }
                else if (BlockManager.instance.blocks[i].transform.position == sensors[4].transform.position)
                {
                    if (BlockManager.instance.blocks[i].GetComponent<SnowBlockManager>().isDestroyed)
                    {
                        BlockManager.instance.blocks[i].GetComponent<SnowBlockManager>().zN.SetActive(false);
                    }
                    else
                    {
                        zP.SetActive(true);
                    }
                }
                else if (BlockManager.instance.blocks[i].transform.position == sensors[5].transform.position)
                {
                    if (BlockManager.instance.blocks[i].GetComponent<SnowBlockManager>().isDestroyed)
                    {
                        BlockManager.instance.blocks[i].GetComponent<SnowBlockManager>().zP.SetActive(false);
                    }
                    else
                    {
                        zN.SetActive(true);
                    }
                }
            }
        }

        
        //sensors[2].transform.position
    }

    void BuildSnow()
    {
        baseCube.SetActive(true);
        zP.SetActive(false);
        zN.SetActive(false);
        xP.SetActive(false);
        xN.SetActive(false);

        RefreshSlopes();

    }

    public void RefreshSlopes()
    {
        for(int i = 0; i < BlockManager.instance.blocks.Count; i++)
        {
            if(sensors[4].transform.position == BlockManager.instance.blocks[i].transform.position && BlockManager.instance.blocks[i].GetComponent<SnowBlockManager>().isDestroyed)
            {
                BlockManager.instance.blocks[i].GetComponent<SnowBlockManager>().zN.SetActive(true);
            }
            else if (sensors[5].transform.position == BlockManager.instance.blocks[i].transform.position && BlockManager.instance.blocks[i].GetComponent<SnowBlockManager>().isDestroyed)
            {
                BlockManager.instance.blocks[i].GetComponent<SnowBlockManager>().zP.SetActive(true);
            }
            else if (sensors[0].transform.position == BlockManager.instance.blocks[i].transform.position && BlockManager.instance.blocks[i].GetComponent<SnowBlockManager>().isDestroyed)
            {
                BlockManager.instance.blocks[i].GetComponent<SnowBlockManager>().xN.SetActive(true);
            }
            else if (sensors[1].transform.position == BlockManager.instance.blocks[i].transform.position && BlockManager.instance.blocks[i].GetComponent<SnowBlockManager>().isDestroyed)
            {
                BlockManager.instance.blocks[i].GetComponent<SnowBlockManager>().xP.SetActive(true);
            }
        }
    }

    void OnMouseEnter()
    {
        scrolledOver = true;
		Highlight();
        
    }



    void OnMouseExit()
    {
        scrolledOver = false;
		UnHighlight();
    }

    
}
