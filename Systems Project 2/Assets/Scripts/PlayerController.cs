using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class PlayerController : MonoBehaviour {

    public enum Player
    {
        P1 = 0,
        P2 = 1
    }

    public Player whichPlayer;
    InputDevice pControl;

    [SerializeField] private int speed;

    Rigidbody rb;
    private float movex, movez;

    private void Awake()
    {
        pControl = InputManager.Devices[(int)whichPlayer];
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        CheckInput();
    }

    void CheckInput()
    {
        rb.velocity = new Vector3(movex * speed,0, movez * speed);


        Vector3 nextDir = new Vector3(movex, 0, movez);
        if (nextDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(nextDir);

        }

    }




}
