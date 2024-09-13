using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private GameObject player;
    private Vector3 currentCheckpointPosition;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
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
        Debug.Log("moving player at " + currentCheckpointPosition);
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            player.transform.position = currentCheckpointPosition != Vector3.zero ? currentCheckpointPosition : player.transform.position;
            Debug.Log("player's new location: " + player.transform.position);
        }
    }

    public void setCheckpoint(Vector3 _newCheckpoint)
    {
        currentCheckpointPosition = _newCheckpoint;
    }
}
