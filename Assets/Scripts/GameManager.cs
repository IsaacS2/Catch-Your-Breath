using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int numLevelSegments;

    public static GameManager Instance;

    private GameObject player;
    private Vector3 currentCheckpointPosition;
    private int currentCheckpointValue = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else {
            Instance = this;

            DontDestroyOnLoad(Instance.gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            player.transform.position = currentCheckpointPosition != Vector3.zero ? currentCheckpointPosition : player.transform.position;
        }
    }

    private void Update()
    {
        var emitter = GetComponent<StudioEventEmitter>();
        if (emitter != null) {
            emitter.SetParameter(name: "Checkpoint", Mathf.Min(currentCheckpointValue, numLevelSegments - 1));
        }
    }

    public void setCheckpoint(Vector3 _newCheckpoint)
    {
        if (_newCheckpoint != currentCheckpointPosition)  // new checkpoint found, increment value for fmod
        {
            currentCheckpointValue++;
        }

        currentCheckpointPosition = _newCheckpoint;
    }

    public void ResetValues()
    {
        currentCheckpointPosition = Vector3.zero;
        currentCheckpointValue = 0;
    }
}
