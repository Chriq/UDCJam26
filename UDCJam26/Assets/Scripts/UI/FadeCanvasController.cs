using System;
using System.Collections;
using UnityEngine;


public class FadeCanvasController : MonoBehaviour {
	public CanvasGroup canvas;

	private void Awake() {
		canvas = GetComponent<CanvasGroup>();
	}

	public void FadeOut() {
		StartCoroutine(FadeCoroutine(1f, 0f, 1f));
	}

	public void FadeIn() {
		StartCoroutine(FadeCoroutine(0f, 1f, 1f));
	}

	public void FadeOut(float duration) {
		StartCoroutine(FadeCoroutine(1f, 0f, duration));
	}

	public void FadeIn(float duration) {
		StartCoroutine(FadeCoroutine(0f, 1f, duration));
	}

	public void FadeOutWithCallback(Action action) {
		StartCoroutine(FadeCoroutineWithCallback(1f, 0f, action, 1f));
	}

	public void FadeInWithCallback(Action action) {
		StartCoroutine(FadeCoroutineWithCallback(0f, 1f, action, 1f));
	}

	public void FadeOutWithCallback(Action action, float duration) {
		StartCoroutine(FadeCoroutineWithCallback(1f, 0f, action, duration));
	}

	public void FadeInWithCallback(Action action, float duration) {
		StartCoroutine(FadeCoroutineWithCallback(0f, 1f, action, duration));
	}

	private IEnumerator FadeCoroutine(float start, float end, float duration) {
		float counter = 0;

		while(counter < duration) {
			counter += Time.deltaTime;
			canvas.alpha = Mathf.Lerp(start, end, counter / duration);
			yield return null;
		}

		ToggleCanvasEnabled();
	}

	private IEnumerator FadeCoroutineWithCallback(float start, float end, Action action, float duration) {
		float counter = 0;

		while(counter < duration) {
			counter += Time.deltaTime;
			canvas.alpha = Mathf.Lerp(start, end, counter / duration);
			yield return null;
		}

		ToggleCanvasEnabled();
		action.Invoke();
	}

	private void ToggleCanvasEnabled() {
		canvas.blocksRaycasts = !canvas.blocksRaycasts;
		canvas.interactable= !canvas.interactable;
	}
}
