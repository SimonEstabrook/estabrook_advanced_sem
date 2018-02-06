using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBlockManager : MonoBehaviour {

    public GameObject zP, zN, xP, xN;

    [SerializeField] private List<GameObject> sensors;

    [SerializeField] private GameObject baseCube;

    private Color startcolor;

    public bool isDestroyed = false;
    public bool scrolledOver = false;

    private void Start()
    {
        startcolor = Color.white;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && scrolledOver)
        {
            Debug.Log("Destroy" + this.name);
            isDestroyed = true;
            destroyCube();
        }
    }

    void destroyCube()
    {
        baseCube.SetActive(false);
        
        for(int i = 0; i < BlockManager.instance.blocks.Count; i++)
        {
            if(BlockManager.instance.blocks[i].transform.position == sensors[2].transform.position)
            {
                BlockManager.instance.blocks[i].SetActive(false);
            }
            else if (BlockManager.instance.blocks[i].transform.position == sensors[0].transform.position)
            {
                if(BlockManager.instance.blocks[i].GetComponent<SnowBlockManager>().isDestroyed)
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

        
        //sensors[2].transform.position
    }

    void OnMouseEnter()
    {
        scrolledOver = true;
        for(int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).GetComponent<MeshRenderer>() != null)
            {
                transform.GetChild(i).GetComponent<MeshRenderer>().material.color = Color.yellow;
            }

            
        }
        
    }



    void OnMouseExit()
    {
        scrolledOver = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<MeshRenderer>() != null)
            {
                transform.GetChild(i).GetComponent<MeshRenderer>().material.color = startcolor;
            }
        }
    }

    
}
