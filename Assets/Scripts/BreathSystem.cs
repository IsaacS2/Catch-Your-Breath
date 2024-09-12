using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BreathSystem : MonoBehaviour
{
    [SerializeField] private InputActionReference breathIn, breathOut;
    [SerializeField] private float maxBreathlessTime, breathStageBestTime, segmentPercent, speedRevertTime, reducedSpeedTime, minStableBreathTime;
    [SerializeField] private int segments;
    [SerializeField] private Image breathRing;
    [SerializeField] private Image breath;
    [SerializeField] private Color strongBreathIndicator, weakBreathIndicator, inhaleColor, holdColor, exhaleColor;

    public event Action<float> OnBreathComplete = (_breathMultiplier) => { };
    private float[] ringHeights; // heights of rings will be used to determine input accuracy
    private Image[] ringImages; // heights of rings will be used to determine input accuracy
    private Vector2 breathStartSize;
    private float breathlessTimer, finishedBreathTimer, breathStageTimer, breathInMultiplier, holdingMultiplier, breathOutMultiplier;
    private int windedState;
    private BreathStates breathingState;

    private void OnEnable()
    {
        breathIn.action.started += BreathIn;
        breathIn.action.canceled += HoldBreath;
        breathOut.action.started += BreathOut;
        breathOut.action.canceled += StopBreath;
    }

    private void Start()
    {
        breathingState = BreathStates.Idle;  // player starts out not breathing
        RectTransform initialBreathRect = (RectTransform)breathRing.transform;
        breathStartSize = initialBreathRect.sizeDelta;
        ringHeights = new float[segments + 1];
        ringImages = new Image[segments + 1];
        windedState = 0;  // not "winded"

        if (segments < 1) // too few segments
        {
            segments = 1;
        }
        if (segments % 2 == 0) // even amount of ring segments doesn't make sense
        {
            segments++;
        }

        if ((1f / (float)segments) < segmentPercent || segmentPercent <= 0) // segmentPerecent is too large to uniformely size segments (or is negative)
        {
            segmentPercent = 1 / (float)segments;
        }

        Vector2 firstRingSize = new Vector2(breathStartSize.x - ((((float)segments / 2) * segmentPercent) * breathStartSize.x), 
            breathStartSize.y - ((((float)segments / 2) * segmentPercent) * breathStartSize.y));
        Debug.Log(firstRingSize);

        for (int i = 0; i < segments + 1; i++)
        {
            // create gameObject with a new breath ring image that's also a child of the canvas (so it's part of the UI)
            GameObject newBreathRing = Instantiate(breathRing.gameObject, 
                ((RectTransform)breathRing.transform).transform.position,
                breathRing.transform.rotation,
                breathRing.gameObject.transform.root);

            // adjust individual ring size
            initialBreathRect = (RectTransform)newBreathRing.transform;
            initialBreathRect.sizeDelta = firstRingSize + (segmentPercent * i * breathStartSize);
            ringHeights[i] = initialBreathRect.sizeDelta.y;
            ringImages[i] = newBreathRing.GetComponent<Image>();
        }

        int halfSegments = (segments + 1) / 2;

        //
        // adjusting ring color based on correlating segment
        //
        for (int i = 0; i < halfSegments; i++)
        {
            Color newRingColor = Color.Lerp(weakBreathIndicator, strongBreathIndicator, (float)i + 1 / (float)halfSegments);
            ringImages[i].color = newRingColor;
            //ringImages[i].gameObject.SetActive(false);
        }
        for (int i = segments; i >= (segments + 1) / 2; i--)
        {
            Color newRingColor = Color.Lerp(strongBreathIndicator, weakBreathIndicator, (float)(i - halfSegments) / (float)halfSegments);
            ringImages[i].color = newRingColor;
            //ringImages[i].gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        breathlessTimer += Time.deltaTime;
        finishedBreathTimer += Time.deltaTime;
        
        if (breathlessTimer > maxBreathlessTime && windedState < 3)  // couldn't breath properly for too long
        {
            windedState = 3;
        }
        else if (breathlessTimer > reducedSpeedTime && windedState < 2)  // hasn't breathed properly for a while
        {
            Debug.Log("need to breath");
            OnBreathComplete(0.75f);
            windedState = 2;
        }
        else if (breathlessTimer > speedRevertTime && windedState <= 0) // lost directional speed boost
        {
            Debug.Log("lost speed (should probably start breathing again)");
            OnBreathComplete(1);
            windedState = 1;
        }

        if (breathingState != BreathStates.Idle)
        {
            breathStageTimer += Time.deltaTime;
            Vector2 newBreathSize = breathStartSize * (breathStageTimer / breathStageBestTime);
            ((RectTransform)breath.transform).sizeDelta = newBreathSize;

            if (newBreathSize.y > ringHeights[ringHeights.Length - 1])  // too much air inhaled
            {
                breathingState = BreathStates.Idle;
                SetBreathUI(false);
            }
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
            if (breathlessTimer >= minStableBreathTime) {
                breathingState = BreathStates.BreathingIn;
                breath.color = inhaleColor;
                SetBreathUI(true);
            }
            else // trying to start box-breathing again too soon
            {
                Debug.Log("Breathing too fast!");
                OnBreathComplete(0.75f);
            }
        }
    }

    private void HoldBreath(InputAction.CallbackContext obj)
    {
        if (breathingState == BreathStates.BreathingIn)
        {
            float currentBreathHeight = ((RectTransform)breath.transform).sizeDelta.y;

            if (currentBreathHeight < ringHeights[0])
            {
                breathingState = BreathStates.Idle;  // breathed in too quickly (not enough air)
                OnBreathComplete(1);
                SetBreathUI(false);
            }
            else
            {
                breathingState = BreathStates.HoldingIn;
                breath.color = holdColor;
                float breathDivisor = breathStartSize.y - Mathf.Abs(currentBreathHeight - breathStartSize.y);
                breathInMultiplier = (1f / 3f) * (breathDivisor / breathStartSize.y);
            }

            breathStageTimer = 0;
        }
    }

    private void BreathOut(InputAction.CallbackContext obj)
    {
        if (breathingState == BreathStates.HoldingIn)
        {
            float currentBreathHeight = ((RectTransform)breath.transform).sizeDelta.y;

            if (currentBreathHeight < ringHeights[0])
            {
                OnBreathComplete(1);
                breathingState = BreathStates.Idle;  // released breath in too quickly
                SetBreathUI(false);
            }
            else
            {
                breathingState = BreathStates.BreathingOut;
                breath.color = exhaleColor;
                float breathDivisor = breathStartSize.y - Mathf.Abs(currentBreathHeight - breathStartSize.y);
                holdingMultiplier = (1f / 3f) * (breathDivisor / breathStartSize.y);
            }

            breathStageTimer = 0;
        }
    }

    private void StopBreath(InputAction.CallbackContext obj)
    {
        if (breathingState == BreathStates.BreathingOut)
        {
            float currentBreathHeight = ((RectTransform)breath.transform).sizeDelta.y;

            if (currentBreathHeight >= ringHeights[0])
            {
                Debug.Log("Successful breathing");
                float breathDivisor = breathStartSize.y - Mathf.Abs(currentBreathHeight - breathStartSize.y);
                breathOutMultiplier = (1f / 3f) * (breathDivisor / breathStartSize.y);
                breathlessTimer = 0;  // got adequate breathing in
                OnBreathComplete(1.5f + breathInMultiplier + breathOutMultiplier + holdingMultiplier);
            }
            else
            {
                OnBreathComplete(1);
            }

            breathingState = BreathStates.Idle;
            windedState = 0;
            breathStageTimer = 0;
            SetBreathUI(false);
        }
    }

    private void SetBreathUI(bool activate)
    {
        breathRing.gameObject.SetActive(activate);
        breath.gameObject.SetActive(activate);

        foreach (Image ring in ringImages)
        {
            ring.gameObject.SetActive(activate);
        }
    }
}
