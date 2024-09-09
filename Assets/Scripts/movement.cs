using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class movement : MonoBehaviour
{
    [SerializeField] private float flightSpeed, turnSpeed, maxHorizontalTurn, maxVerticalTurn, rotateTime, breathValue;
    [SerializeField] private InputActionReference turning, breathing;

    private Vector2 turnDirection;
    private float currentRotation;
    private Rigidbody rb;

    private void OnEnable()
    {
        
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = Quaternion.Slerp(currentRotation, , rotateTime);
        //timeCount = timeCount + Time.deltaTime;

        turnDirection = turning.action.ReadValue<Vector2>() * turnSpeed;
    }

    private void FixedUpdate()
    {
        rb.velocity = (transform.forward * flightSpeed) + new Vector3(turnDirection.x, turnDirection.y, 0);
    }


}
