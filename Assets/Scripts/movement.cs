using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] private float flightSpeed, turnSpeed, maxHorizontalTurn, maxVerticalTurn, rotateTime;
    [SerializeField] private InputActionReference turning;

    private Vector3 startRotation, currentRotation;
    private Vector2 turnDirection;
    private float breathMultiple;
    private Rigidbody rb;

    private void OnEnable()
    {
        startRotation = transform.eulerAngles;
    }

    void Start()
    {
        breathMultiple = 1;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        turnDirection = turning.action.ReadValue<Vector2>() * turnSpeed;

        //Vector3 maxRotation = new Vector3(maxHorizontalTurn * turnDirection.x, maxVerticalTurn * turnDirection.y, 0);
        //Vector3 newRotation = Vector3.Lerp(currentRotation, maxRotation, Time.deltaTime);
    }

    private void FixedUpdate()
    {
        rb.velocity = (transform.forward * flightSpeed) 
            + (turnDirection.x * transform.right * breathMultiple) 
            + (turnDirection.y * transform.up * breathMultiple);
    }
}
