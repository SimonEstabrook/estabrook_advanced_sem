using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlockManager : MonoBehaviour {

    [HideInInspector] public List<GameObject> blocks;

    public static BlockManager instance;

    [SerializeField] int length, width, height;

    [SerializeField] GameObject block;
    GameObject blockParent;

    private GameObject camera;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        blockParent = GameObject.Find("Blocks");

        for(int l = 0; l < length; l++)
        {
            for(int h = 0; h < height; h++)
            {
                for(int w = 0; w < width; w++)
                {
                    GameObject BlockInst = Instantiate(block, new Vector3(l * 2, h * 2, w * 2), Quaternion.identity);
                    
                    BlockInst.transform.parent = blockParent.transform;
                }
            }
        }
        blocks.AddRange(GameObject.FindGameObjectsWithTag("Block"));

        GameManager.instance.GameStart = true;
        camera.transform.position = new Vector3(length-1, camera.transform.position.y, camera.transform.position.z);
    }

    public int GiveMinHeight()
    {
        return (height * 2);
    }
}
