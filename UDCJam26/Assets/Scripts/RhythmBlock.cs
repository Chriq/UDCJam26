using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmBlock : MonoBehaviour {
	public float speedMutliplier = 1f;
	private float speed;

	private void Start() {
		speed = GameController.bpm / 60f;
	}

	private void Update() {
		transform.Translate(Vector3.left * speed * speedMutliplier * Time.deltaTime);
	}

	public void Despawn() {
		StartCoroutine(StartDespawnTimer());
	}

	private IEnumerator StartDespawnTimer() {
		yield return new WaitForSeconds(1f);
		Destroy(gameObject);
	}
}
