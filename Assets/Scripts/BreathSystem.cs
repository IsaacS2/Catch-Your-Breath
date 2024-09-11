using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BreathSystem : MonoBehaviour
{
    [SerializeField] private InputActionReference turning, breathIn, breathOut;
    private int breathingState;

    private void OnEnable()
    {
        breathIn.action.started += BreathIn;
        breathIn.action.performed += HoldBreath;
        breathOut.action.started += BreathOut;
        breathOut.action.performed += StopBreath;
    }

    private void BreathIn(InputAction.CallbackContext obj)
    {

    }

    private void HoldBreath(InputAction.CallbackContext obj)
    {

    }

    private void BreathOut(InputAction.CallbackContext obj)
    {

    }

    private void StopBreath(InputAction.CallbackContext obj)
    {

    }
}
