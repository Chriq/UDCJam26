using System;
using System.Collections.Generic;
using UnityEngine;

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
        GameObject tmp = GameObject.Instantiate(targetObject);
        pooledObjects.Add(tmp);
        return tmp;
    }
}

public class GameController : MonoBehaviour
{
    public static float bpm = 120f;
    public CircleCollider2D target;
    public Transform spawnPoint;

    private List<GameObject> activeBlocks;
    private GameObject currentBlock;
    private float secondsPerBeat;
    private float timer = 0f;

    /* Object Pools */
    [SerializeField] private ObjectPool objectPool_note_ON;
    [SerializeField] private ObjectPool objectPool_note_OFF;
    [SerializeField] private ObjectPool objectPool_bar;

    /* Spawn Settings */
    [SerializeField] private float targetDistance;          // Target distance
    [SerializeField] private float targetTime;              // Time to reach target
    [SerializeField] private float objectVelocity;          // Spawned object speed
    private float objectAcceleration;                       // Spawned object acceleration

    /* Audio */
    [SerializeField] private AudioSource audioSource;        // Music source

    private void Start()
    {
        objectAcceleration = Time.fixedDeltaTime * (targetDistance - objectVelocity * targetTime) / targetTime / targetTime;
        // TODO: Warn in editor if peak is within sreen bounds
        // TODO: Check this calculation
        float peak = -objectVelocity * objectVelocity * Time.fixedDeltaTime / 4 / objectAcceleration * 2;

        secondsPerBeat = 60f / bpm;
        activeBlocks = new List<GameObject>();

        objectPool_note_ON.Init();
        objectPool_note_OFF.Init();
        objectPool_bar.Init();
        Play();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= secondsPerBeat)
        {
            Spawn();
            timer -= secondsPerBeat;
        }

        CheckCurrentBlock();
    }

    public void Play()
    {
        //Invoke("StartSequence", songDelay);
        Invoke("StartAudio", 5);
    }

    void StartAudio()
    {
        audioSource.Play();
    }

    private void CheckCurrentBlock()
    {
        currentBlock = activeBlocks[0];
        if (currentBlock)
        {
            if (currentBlock.GetComponent<CircleCollider2D>().IsTouching(target))
            {
                float dist = (currentBlock.transform.position - target.transform.position).magnitude;
            }
        }
    }

    public void Spawn()
    {
        GameObject obj;

        /* Spawn Visual Elements */
        obj = objectPool_bar.GetObject();
        obj.GetComponent<BeatMovement>().SetParameters(objectVelocity, objectAcceleration);
        obj.transform.position = spawnPoint.position;


        /* Spawn Notes */
        // TODO: Read Sequence for next note
        obj = objectPool_note_ON.GetObject();
        obj.GetComponent<BeatMovement>().SetParameters(objectVelocity, objectAcceleration);
        obj.transform.position = spawnPoint.position;
        activeBlocks.Add(obj);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        RhythmBlock rhythmBlock;
        if (collision.gameObject.TryGetComponent<RhythmBlock>(out rhythmBlock))
        {
            activeBlocks.Remove(rhythmBlock.gameObject);
            rhythmBlock.Despawn();
        }
    }
}
