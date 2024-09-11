using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class BreathSystem : MonoBehaviour
{
    [SerializeField] private InputActionReference breathIn, breathOut;
    [SerializeField] private float maxBreathlessTime, segmentPercents, breathAccuracySegments = 3;
    [SerializeField] private Image breathRing;

    public event Action<float> OnBreathComplete = (_breathMultiplier) => { };
    private Vector2 breathStartSize;
    private float breathlessTimer, breathStageTimer;
    private BreathStates breathingState;

    private void OnEnable()
    {
        breathIn.action.started += BreathIn;
        breathIn.action.performed += HoldBreath;
        breathOut.action.started += BreathOut;
        breathOut.action.performed += StopBreath;
    }

    private void Start()
    {
        breathingState = BreathStates.Idle;  // player starts out not breathing
        RectTransform initialBreathRect = (RectTransform)breathRing.transform;
        breathStartSize = initialBreathRect.sizeDelta;
        Debug.Log(breathStartSize);
    }

    private void Update()
    {
        if (breathingState != BreathStates.Idle)
        {
            breathStageTimer += Time.deltaTime;
        }
        else
        {
            breathlessTimer += Time.deltaTime;
        }
    }

    private void BreathIn(InputAction.CallbackContext obj)
    {
        if (breathingState == BreathStates.Idle)
        {
            breathingState = BreathStates.BreathingIn;
        }
    }

    private void HoldBreath(InputAction.CallbackContext obj)
    {
        if (breathingState == BreathStates.BreathingIn)
        {
            breathingState = BreathStates.BreathingIn;

        }
    }

    private void BreathOut(InputAction.CallbackContext obj)
    {

    }

    private void StopBreath(InputAction.CallbackContext obj)
    {

    }
}
