using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float movementSpeed = 2f;
    
    public float jumpPower = 2f;

    private Vector3 _movement;

    private bool jump;
    
    private void Update()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            _movement = Vector3.left;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            _movement = Vector3.right;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }
        
    }

    private void FixedUpdate()
    {
        transform.position += _movement * movementSpeed;
        if (jump)
        {
            transform.position += Vector3.up * jumpPower;
            jump = false;
        }
    }
}
