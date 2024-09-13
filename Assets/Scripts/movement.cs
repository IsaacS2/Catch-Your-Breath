using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] private float flightSpeed, originalTurnSpeed;
    [SerializeField] private InputActionReference turning;
    [SerializeField] private BreathSystem lungs;

    private Vector3 startRotation;
    private Vector2 turnDirection;
    private float currentTurnSpeed;
    private Rigidbody rb;

    private void OnEnable()
    {
        lungs.OnBreathComplete += IncreaseDirectionalSpeed;
        startRotation = transform.eulerAngles;
        currentTurnSpeed = originalTurnSpeed;
    }

    private void OnDisable()
    {
        lungs.OnBreathComplete -= IncreaseDirectionalSpeed;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        turnDirection = turning.action.ReadValue<Vector2>() * currentTurnSpeed;
    }

    private void FixedUpdate()
    {
        //
        // moves player in local xy plane
        //
        rb.velocity = (transform.forward * flightSpeed) 
            + (turnDirection.x * transform.right) 
            + (turnDirection.y * transform.up);
    }

    private void IncreaseDirectionalSpeed(float _breathMultiplier)
    {
        currentTurnSpeed = originalTurnSpeed * _breathMultiplier;

        Debug.Log("Speed change: " + currentTurnSpeed);
    }
}
