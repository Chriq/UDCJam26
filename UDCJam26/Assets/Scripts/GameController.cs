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
                return obj;
        }

        // Spawn new object
        GameObject tmp = GameObject.Instantiate(targetObject);
        tmp.SetActive(false);
        pooledObjects.Add(tmp);
        return tmp;
    }
}

public class GameController : MonoBehaviour
{
    public static float bpm = 120f;
    public GameObject block;
    public CircleCollider2D target;
    public Transform spawnPoint;

    private List<GameObject> activeBlocks;
    private GameObject currentBlock;
    private float secondsPerBeat;
    private float timer = 0f;

    [SerializeField] private ObjectPool objectPool_note_ON;
    [SerializeField] private ObjectPool objectPool_note_OFF;
    [SerializeField] private ObjectPool objectPool_bar;

    private void Start()
    {
        secondsPerBeat = bpm / 60f;
        secondsPerBeat = 1 / secondsPerBeat;
        activeBlocks = new List<GameObject>();

        objectPool_note_ON.Init();
        objectPool_note_OFF.Init();
        objectPool_bar.Init();

        Spawn();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= secondsPerBeat)
        {
            Spawn();
            timer = 0f;
        }

        CheckCurrentBlock();
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
        GameObject spawn = Instantiate(block);
        spawn.transform.position = spawnPoint.position;
        activeBlocks.Add(spawn);
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
