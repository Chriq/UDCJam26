using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public static float bpm = 120f;
	public GameObject block;
	public CircleCollider2D target;
	public Transform spawnPoint;

	private List<GameObject> activeBlocks;
	private GameObject currentBlock;
	private float secondsPerBeat;
	private float timer = 0f;

	private void Start() {
		secondsPerBeat = bpm / 60f;
		secondsPerBeat = 1 / secondsPerBeat;
		activeBlocks = new List<GameObject>();
		Spawn();
	}

	private void Update() {
		timer += Time.deltaTime;
		if(timer >= secondsPerBeat) {
			Spawn();
			timer = 0f;
		}

		CheckCurrentBlock();
	}

	private void CheckCurrentBlock() {
		currentBlock = activeBlocks[0];
		if(currentBlock) {
			if(currentBlock.GetComponent<CircleCollider2D>().IsTouching(target)) {
				float dist = (currentBlock.transform.position - target.transform.position).magnitude;
			}
		}
	}

	public void Spawn() {
		GameObject spawn = Instantiate(block);
		spawn.transform.position = spawnPoint.position;
		activeBlocks.Add(spawn);
	}

	private void OnTriggerExit2D(Collider2D collision) {
		RhythmBlock rhythmBlock;
		if(collision.gameObject.TryGetComponent<RhythmBlock>(out rhythmBlock)) {
			activeBlocks.Remove(rhythmBlock.gameObject);
			rhythmBlock.Despawn();
		}
	}
}
