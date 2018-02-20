using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour {

    [HideInInspector] public List<GameObject> blocks;

    public static BlockManager instance;

    [SerializeField] int length, width, height, thiccness;

    [SerializeField] GameObject block;
    GameObject blockParent;

    private GameObject camera;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        thiccness = Mathf.Clamp(thiccness, 1, 10);

        camera = GameObject.FindGameObjectWithTag("MainCamera");
        blockParent = GameObject.Find("Blocks");

        for(int l = 0; l < length; l++)
        {
            for(int h = 0; h < height; h++)
            {
                for(int w = 0; w < width; w++)
                {
                    GameObject BlockInst = Instantiate(block, new Vector3(l*2, h*2, w*2), Quaternion.identity);
                    if(h >= thiccness)
                    {
                        Debug.Log("THICC BOI");
                        BlockInst.GetComponent<SnowBlockManager>().isDestroyed = true;
                        BlockInst.GetComponent<BoxCollider>().enabled = false;
                        BlockInst.GetComponent<SphereCollider>().enabled = false;
                        BlockInst.GetComponent<SnowBlockManager>().destroyCube();
                    }
                    BlockInst.transform.parent = blockParent.transform;
                }
            }
        }
        blocks.AddRange(GameObject.FindGameObjectsWithTag("Block"));

        GameManager.instance.GameStart = true;
        camera.transform.position = new Vector3(length-1, camera.transform.position.y, camera.transform.position.z);
    }

    // Update is called once per frame
    void Update () {


	}

    public int GiveMinHeight()
    {
        return ((height * 2) - thiccness * 2);
    }
}
