using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cloud;
    [SerializeField] private bool horizontalVariety;
    [SerializeField] private bool verticalVariety;
    [SerializeField] private float minSizeMultiplier, maxSizeMultiplier, minSpeed, maxSpeed, minXPosition, maxXPosition,
        minYPosition, maxYPosition, minZPosition, maxZPosition, spawnTime;
    [SerializeField] private Texture[] textureOptions;

    private Vector3 sideVector, upVector;
    private float spawnTimer;

    private void Start()
    {
        spawnTimer = spawnTime;

        if (horizontalVariety)
        {
            sideVector = transform.right;
        }
        if (verticalVariety)
        {
            upVector = transform.up;
        }
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (cloud != null && spawnTimer > spawnTime)
        {  // gameobject provided to duplicate
            GameObject newCloud = Instantiate(cloud, gameObject.transform.position
                + (sideVector * Random.Range(minXPosition, maxXPosition))
                + (transform.forward * Random.Range(minZPosition, maxZPosition))
                + (upVector * Random.Range(minYPosition, maxYPosition)), Quaternion.identity);

            newCloud.transform.localScale *= Random.Range(minSizeMultiplier, maxSizeMultiplier);

            Cloud cloudScript = newCloud.GetComponent<Cloud>();

            if (cloudScript != null)
            {  // object has a cloud script
                cloudScript.SetSpeed(Random.Range(minSpeed, maxSpeed));

                // cloud's material can be altered
                Material cloudMaterial = newCloud.GetComponent<Material>();
                if (textureOptions.Length > 0 && cloudMaterial)
                {
                    Debug.Log("Changing texture");
                    int textureIndex = Random.Range(0, textureOptions.Length);
                    cloudMaterial.mainTexture = textureOptions[textureIndex];
                }
            }

            spawnTimer = 0;
        }
    }
}
