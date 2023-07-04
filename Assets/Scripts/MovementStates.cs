using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MovementStates : MonoBehaviour
{
    public static bool inWater;
    public static bool isSwimming;

    Rigidbody rb;
    Swimmer swimmer;
    ActionBasedContinuousMoveProvider move;
    //if not in water, walk
    //if in water and not swimming, float
    //if in water and is swimming, swim

    private static LayerMask waterMask;
    
    void SwitchMovement()
    {
        //toggle inWater
        inWater = !inWater;
    }

    void SwimmingOrFloating()
    {
        bool swimCheck = false;
        if (inWater)
        {
            RaycastHit hit;
            if(Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z),
                Vector3.down, out hit, Mathf.Infinity, waterMask))
            {
                if (hit.distance < 0.1f)
                {
                    swimCheck = true;
                }
                else
                {
                    swimCheck = true;
                }
            }
        }

        isSwimming = swimCheck;
        //Debug.Log("isSwimming = " + isSwimming);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        swimmer = GetComponent<Swimmer>();
        move =  GetComponent<ActionBasedContinuousMoveProvider>();
    }
    private void Start()
    {
        inWater = false;
    }

    private void FixedUpdate()
    {
        WalkingOrSwimming();
        SwimmingOrFloating();
    }
    void WalkingOrSwimming()
    {
        if(inWater)
        {
            rb.useGravity = false;
            swimmer.enabled = true;
            move.enabled = false;
        }

        else
        {
            rb.useGravity = true;
            swimmer.enabled = false;
            move.enabled = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        SwitchMovement();
    }

    private void OnTriggerExit(Collider other)
    {
        SwitchMovement();
    }
}
