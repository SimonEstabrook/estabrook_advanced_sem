using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour {

    public List<GameObject> blocks;

    public static BlockManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        blocks.AddRange(GameObject.FindGameObjectsWithTag("Block"));

    }

    // Update is called once per frame
    void Update () {


	}
}
