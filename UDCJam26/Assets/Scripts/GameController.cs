using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    public float bpm = 120f;
    [SerializeField] private float EDITOR_TIME_SCALE;

    private float secondsPerBeat;
    private float timer = 0f;
    private bool songStarted = false;

    /* Hit Detection */
    [SerializeField] private GameObject targetObj;
    [SerializeField] private Color targetColor = Color.green;
    [SerializeField] private Color hitColor = Color.white;
    [SerializeField] private Color noHitColor = Color.white;

    /* Spawn Settings */
    private float targetDistance;                           // Target distance
    [SerializeField] private Transform spawnPoint;          // Spawn Location
    [SerializeField] private float targetTime;              // Time to reach target
    [SerializeField] private float objectVelocity;          // Spawned object speed
    [SerializeField] private float objectDespawnDelay;      // Spawned object despawn delay
    private float objectAcceleration;                       // Spawned object acceleration

    /* Object Pools */
    [SerializeField] private ObjectPool objectPool_note_ON;
    [SerializeField] private ObjectPool objectPool_note_OFF;
    [SerializeField] private ObjectPool objectPool_bar;

    /* Sequence */
    private int currentBeat = 0;
    [SerializeField] private SequenceBuilder sequence;

    /* UI */
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private CanvasGroup fadeCanvas;

    /* Audio */
    [SerializeField] private AudioSource audioSource;        // Music source
    [SerializeField] private AudioSource pop;
    [SerializeField] private float audioDelay;              // Music Start Delay

    /* Score */
    [SerializeField] private int gameScore;
    [SerializeField] private int multiplier;
    [SerializeField] private int seriescount;
	[SerializeField] private TextMeshProUGUI scoreUI;

	/* Inversion */
	[SerializeField] private int numInversions;
	List<int> inversionBars = new List<int>();
    [SerializeField] private Color invertedBackground;
    private Color originalBackground;
    private int currentInversion = int.MinValue;


	/* Effects */
	[SerializeField] private bool inverted;

    private void Start()
    {
        Time.timeScale = EDITOR_TIME_SCALE;
        UIManager.Instance.fade.canvas = fadeCanvas;
        UIManager.Instance.fade.FadeOut();

        targetDistance = (transform.position - spawnPoint.transform.position).magnitude;
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

        foreach (SpriteRenderer sr in targetObj.GetComponentsInChildren<SpriteRenderer>())
        {
            sr.color = targetColor;
        }

        originalBackground = Camera.main.backgroundColor;
        PickBarsForInversion();


        Play();
    }

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (timer >= (secondsPerBeat * 4f / sequence.subdivisions))
        {
            Spawn();
            timer -= (secondsPerBeat * 4 / sequence.subdivisions);
        }
    }

    public void Play()
    {
        // Initialize
        gameScore = 0;
        multiplier = 1;
        seriescount = 0;
        inverted = false;

        // Start
        timer = (secondsPerBeat * 4f / sequence.subdivisions);
        Invoke("StartAudio", targetTime - audioDelay);
    }

    void StartAudio()
    {
        songStarted = true;
        audioSource.Play();
    }

    public void Spawn()
    {
		StartCoroutine(CheckForInversion(currentBeat));

		if (sequence.beats.Length > currentBeat)
        {
            GameObject obj;

            /* Spawn Visual Elements */
            obj = objectPool_bar.GetObject();
            obj.GetComponent<BeatMovement>().SetParameters(objectVelocity, objectAcceleration, objectDespawnDelay);
            obj.transform.position = spawnPoint.position;

            /* Spawn Notes */

            RhythmBlockType type = sequence.beats[currentBeat];

            switch (type)
            {
                case RhythmBlockType.HIT:
                    obj = objectPool_note_ON.GetObject();
                    obj.GetComponent<BeatMovement>().SetParameters(objectVelocity, objectAcceleration, objectDespawnDelay);
                    obj.transform.position = spawnPoint.position;
                    obj.GetComponentInChildren<SpriteRenderer>().color = hitColor;
                    break;
                case RhythmBlockType.NO_HIT:
                    obj = objectPool_note_OFF.GetObject();
                    obj.GetComponent<BeatMovement>().SetParameters(objectVelocity, objectAcceleration, objectDespawnDelay);
                    obj.transform.position = spawnPoint.position;
                    obj.GetComponentInChildren<SpriteRenderer>().color = noHitColor;
                    break;
            }

            currentBeat++;
        }
        else if (sequence.beats.Length == currentBeat)
        {
            StartCoroutine(EndLevel());
            currentBeat++;
        }
    }

    void UpdateScore(string tag, bool hit)
    {
        if (
             hit && ((!inverted && tag == "Note_ON") || (inverted && tag == "Note_OFF")) ||
            !hit && ((inverted && tag == "Note_ON") || (!inverted && tag == "Note_OFF"))
            )
        {
            if (seriescount < 0)
            {
                multiplier = 1;
                seriescount = 0;
            }
            gameScore += multiplier;

            if (++seriescount >= (multiplier << 1))
                multiplier <<= 2;
        }
        else if (
            !hit && ((!inverted && tag == "Note_ON") || (inverted && tag == "Note_OFF")) ||
             hit && ((inverted && tag == "Note_ON") || (!inverted && tag == "Note_OFF"))
            )
        {
            if (seriescount > 0)
            {
                multiplier = 1;
                seriescount = 0;
            }
            gameScore -= multiplier;

            if (--seriescount <= -(multiplier << 1))
                multiplier <<= 2;
        }
        else
        {
            Debug.Log("Target object hit is not tagged!");
        }

        scoreUI.text = $"SCORE: {gameScore}";
    }

    void PickBarsForInversion() {

        if(SceneManager.GetActiveScene().name == "Tutorial") {
            inversionBars.Add(16);
        } else {
            for(int i = 0; i < numInversions;) {
	            int rand = UnityEngine.Random.Range(1, sequence.numBars / 4);
	            int barStart = rand * 4 * sequence.subdivisions;
                if(!inversionBars.Contains(barStart) && !inversionBars.Contains(barStart + 4 * sequence.subdivisions)) {
                    inversionBars.Add(barStart);
                    i++;
                }
            
            }
        }

        inversionBars.Sort();
		currentInversion = inversionBars[0];
	}

	private IEnumerator CheckForInversion(int beat) {
        yield return new WaitForSeconds(targetTime - audioDelay);

        int NUM_BARS = 4;
        int BEATS_PER_BAR = sequence.subdivisions;

        if(inversionBars.Contains(beat)) {
			inverted = !inverted;
            currentInversion = beat;
		} else if(beat == currentInversion + NUM_BARS * BEATS_PER_BAR) {
			inverted = !inverted;
		} else if(beat == currentInversion - (2 * BEATS_PER_BAR / 4)) {
            StartCoroutine(BackgroundCoroutine(originalBackground, invertedBackground, secondsPerBeat * 2f));
		} else if(beat == currentInversion + NUM_BARS * BEATS_PER_BAR - (2 * BEATS_PER_BAR / 4)) {
			StartCoroutine(BackgroundCoroutine(invertedBackground, originalBackground, secondsPerBeat * 2f));
		}
	}

	private IEnumerator BackgroundCoroutine(Color start, Color end, float duration) {
		float counter = 0;

		while(counter < duration) {
			counter += Time.deltaTime;
			Camera.main.backgroundColor = Color.Lerp(start, end, counter / duration);
			yield return null;
		}
	}

    private IEnumerator EndLevel()
    {
        yield return new WaitForSeconds(5f);
        UIManager.Instance.fade.canvas = fadeCanvas;
        UIManager.Instance.fade.FadeInWithCallback(delegate
        {
            SceneManager.LoadScene("SongSelection");
        });

        // TODO: save score
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
        if (coll.gameObject.activeInHierarchy)
        {
            UpdateScore(coll.gameObject.tag, false);
        }
    }

    public void MainClickEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (targetObj && !targetObj.CompareTag("Untagged"))
            {
                pop.Play();
                UpdateScore(targetObj.tag, true);
                targetObj.SetActive(false);
            }
        }
    }

    public void NavClickEvent()
    {
        //Nothing
    }

    public void AltClickEvent(InputAction.CallbackContext context)
    {
        if (songStarted && context.performed)
        {
            pauseMenu.TogglePause();
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
            else
            {
                audioSource.Play();
            }
        }
    }
}
