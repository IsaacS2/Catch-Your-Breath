using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BreathSystem : MonoBehaviour
{
    [SerializeField] private InputActionReference breathIn, breathOut;
    [SerializeField] private float maxBreathlessTime, breathStageBestTime, segmentPercent;
    [SerializeField] private int segments;
    [SerializeField] private Image breathRing;
    [SerializeField] private Image breath;
    [SerializeField] private Color strongBreathIndicator, weakBreathIndicator, inhaleColor, holdColor, exhaleColor;

    public event Action<float> OnBreathComplete = (_breathMultiplier) => { };
    private float[] ringHeights; // heights of rings will be used to determine input accuracy
    private Image[] ringImages; // heights of rings will be used to determine input accuracy
    private Vector2 breathStartSize;
    private float breathlessTimer, breathStageTimer, breathInMultiplier, holdingMultiplier, breathOutMultiplier;
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
        ringHeights = new float[segments + 1];
        ringImages = new Image[segments + 1];

        if (segments < 1) // too few segments
        {
            segments = 1;
        }
        if (segments % 2 == 0) // even amount of ring segments doesn't make sense
        {
            segments++;
        }

        if ((1 / (float)segments) < segmentPercent || segmentPercent <= 0) // segmentPerecent is too large to uniformely size segments (or is negative)
        {
            segmentPercent = 1 / (float)segments;
        }

        Vector2 firstRingSize = new Vector2(breathStartSize.x - (((segments / 2) * segmentPercent) * breathStartSize.x), 
            breathStartSize.y - (((segments / 2) * segmentPercent) * breathStartSize.y));
        Debug.Log(firstRingSize);

        Color startingColor = Color.white;  // color for the largest and smallest rings, indicating least-adequate breathing times
        for (int i = 0; i < segments + 1; i++)
        {
            // create gameObject with a new breath ring image that's also a child of the canvas (so it's part of the UI)
            GameObject newBreathRing = Instantiate(breathRing.gameObject, 
                ((RectTransform)breathRing.transform).transform.position,
                breathRing.transform.rotation,
                breathRing.gameObject.transform.root);

            initialBreathRect = (RectTransform)newBreathRing.transform;
            initialBreathRect.sizeDelta = firstRingSize + (segmentPercent * i * breathStartSize);  // adjust individual ring sizes
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

        if (breathlessTimer < 12)
        {
            Debug.Log("Woken up!");
        }

        if (breathingState != BreathStates.Idle)
        {
            breathStageTimer += Time.deltaTime;
            Vector2 newBreathSize = breathStartSize * (breathStageTimer / breathStageBestTime);
            ((RectTransform)breath.transform).sizeDelta = newBreathSize;

            if (newBreathSize.y > ringHeights[ringHeights.Length - 1])  // too much air inhaled
            {
                breathingState = BreathStates.Idle;
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
            breathingState = BreathStates.BreathingIn;
            breath.color = inhaleColor;
            SetBreathUI(true);
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
                breath.color = holdColor;
                SetBreathUI(false);
            }
            else
            {
                float breathDivisor = breathStartSize.y - Mathf.Abs(currentBreathHeight - breathStartSize.y);
                breathInMultiplier = (1 / 3) * (breathDivisor / breathStartSize.y);
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
                breathingState = BreathStates.Idle;  // released breath in too quickly
                SetBreathUI(false);
                breath.color = exhaleColor;
            }
            else
            {
                float breathDivisor = breathStartSize.y - Mathf.Abs(currentBreathHeight - breathStartSize.y);
                holdingMultiplier = (1 / 3) * (breathDivisor / breathStartSize.y);
            }
            breathStageTimer = 0;
        }
    }

    private void StopBreath(InputAction.CallbackContext obj)
    {
        if (breathingState == BreathStates.BreathingOut)
        {
            float currentBreathHeight = ((RectTransform)breath.transform).sizeDelta.y;
            if (currentBreathHeight < ringHeights[0])
            {
                breathingState = BreathStates.Idle;  // exhaled breath too quickly
            }
            else
            {
                float breathDivisor = breathStartSize.y - Mathf.Abs(currentBreathHeight - breathStartSize.y);
                breathOutMultiplier = (1 / 3) * (breathDivisor / breathStartSize.y);
                breathlessTimer = 0;  // got adequate breathing in

                OnBreathComplete(1 + breathInMultiplier + breathOutMultiplier + holdingMultiplier);
            }
            breathStageTimer = 0;
            SetBreathUI(true);
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
