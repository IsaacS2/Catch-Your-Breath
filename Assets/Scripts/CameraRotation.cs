using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private InputActionReference turning;
    [SerializeField] private float maxHorizontalRotation, maxVerticalRotation, rotateSpeed;

    private Vector3 currentRotation, startRotation;
    private Vector2 turnDirection;

    // Start is called before the first frame update
    void Start()
    {
        startRotation = currentRotation = transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        turnDirection = new Vector2(turning.action.ReadValue<Vector2>().y * -maxVerticalRotation, turning.action.ReadValue<Vector2>().x * maxHorizontalRotation) 
            + new Vector2(startRotation.x, startRotation.y);
        currentRotation = Vector3.Lerp(currentRotation, turnDirection, Time.deltaTime * rotateSpeed);
        transform.localEulerAngles = currentRotation;
    }
}
