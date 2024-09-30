using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Cloud : MonoBehaviour
{
    [SerializeField] private float maxTravelDistance;

    private Renderer rend;
    private float travelDistance, speed = 5;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
        travelDistance += Time.deltaTime;

        if (travelDistance > maxTravelDistance)  // destroy cloud after it's gone too far
        {
            Destroy(gameObject);
        }
    }

    public void SetSpeed(float _speed) {
        speed = _speed;
    }
}
