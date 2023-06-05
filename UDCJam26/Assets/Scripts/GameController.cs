using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class ObjectPool
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private int targetAmount;

    [SerializeField] private List<GameObject> pooledObjects;

    public void Init()
    {
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < targetAmount; i++)
        {
            GameObject tmp = GameObject.Instantiate(targetObject);
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
        }
    }
    public GameObject GetObject()
    {
        // Return inactive object
        foreach (GameObject obj in pooledObjects)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // Spawn new object
        Debug.Log("Object Pool Full - Instantiating new prefab");
        GameObject tmp = GameObject.Instantiate(targetObject);
        pooledObjects.Add(tmp);
        return tmp;
    }
}

public class GameController : MonoBehaviour
{
    public static float bpm = 120f;
    public Transform spawnPoint;

    private float secondsPerBeat;
    private float timer = 0f;

    /* Hit Detection */
    [SerializeField] private GameObject targetObj;

    /* Spawn Settings */
    [SerializeField] private float targetDistance;          // Target distance
    [SerializeField] private float targetTime;              // Time to reach target
    [SerializeField] private float objectVelocity;          // Spawned object speed
    private float objectAcceleration;                       // Spawned object acceleration
    [SerializeField] private float objectDespawnDelay;      // Spawned object despawn delay

    /* Object Pools */
    [SerializeField] private ObjectPool objectPool_note_ON;
    [SerializeField] private ObjectPool objectPool_note_OFF;
    [SerializeField] private ObjectPool objectPool_bar;

    /* Sequence */
    private int currentBeat = 0;
    [SerializeField] private SequenceBuilder sequence;

    /* Audio */
    [SerializeField] private AudioSource audioSource;        // Music source

    private void Start()
    {
        targetDistance = (target.transform.position - spawnPoint.transform.position).magnitude;
        objectAcceleration = Time.fixedDeltaTime * (targetDistance - objectVelocity * targetTime) / targetTime / targetTime;
        // TODO: Warn in editor if peak is within sreen bounds
        // TODO: Check this calculation
        float peak = -objectVelocity * objectVelocity * Time.fixedDeltaTime / 4 / objectAcceleration * 2;

        secondsPerBeat = 60f / bpm;

        objectPool_note_ON.Init();
        objectPool_note_OFF.Init();
        objectPool_bar.Init();

        if (audioSource == null)
        {
            audioSource = this.gameObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("Audio Source component not found in GameController!!");
            }
        }
        Play();
    }

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (timer >= secondsPerBeat)
        {
            Spawn();
            timer -= secondsPerBeat;
        }

        // TODO: Read Button Press
        // if (trigger)
        //     if (button)
        //         activate note
    }

    public void Play()
    {
        Spawn();
        Invoke("StartAudio", targetTime - 0.2f);
    }

    void StartAudio()
    {
        audioSource.Play();
    }

    public void Spawn()
    {
        GameObject obj;

        /* Spawn Visual Elements */
        obj = objectPool_bar.GetObject();
        obj.GetComponent<BeatMovement>().SetParameters(objectVelocity, objectAcceleration, objectDespawnDelay);
        obj.transform.position = spawnPoint.position;

        /* Spawn Notes */

        RhythmBlockType type = RhythmBlockType.EMPTY;
        if (sequence.beats.Length > currentBeat)
        {
            type = sequence.beats[currentBeat];
        }

        switch (type)
        {
            case RhythmBlockType.HIT:
                obj = objectPool_note_ON.GetObject();
                obj.GetComponent<BeatMovement>().SetParameters(objectVelocity, objectAcceleration, objectDespawnDelay);
                obj.transform.position = spawnPoint.position;
                break;
            case RhythmBlockType.NO_HIT:
                obj = objectPool_note_OFF.GetObject();
                obj.GetComponent<BeatMovement>().SetParameters(objectVelocity, objectAcceleration, objectDespawnDelay);
                obj.transform.position = spawnPoint.position;
                break;
        }

        currentBeat++;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        targetObj = coll.gameObject;
    }
    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject == targetObj)
        {
            targetObj = null;
        }

        // TODO: Note Miss
    }

    public void MainClickEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (targetObj)
            {
                targetObj.SetActive(false);
            }
        }
    }

    public void NavClickEvent()
    {
        //Nothing
    }

    public void AltClickEvent()
    {
        // TODO: Pause
    }
}
