using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour {
	public string text;
	public float fadeSpeed = 0.5f;
	public float textSpeed = 0.03f;
	public float displaySeconds = 2f;

	public TextMeshProUGUI dialog;
	private FadeCanvasController fade;

	private void Start() {
		fade = UIManager.Instance.fade;
	}

	public void DisplayTextUI() {
		StopAllCoroutines();
		dialog.text = "";
		if(text != "") {
			fade.FadeInWithCallback(delegate {
				StartCoroutine("DisplayText");
			}, fadeSpeed);
		}
	}

	public void DisplayTextUIWithCallback(Action action) {
		StopAllCoroutines();
		dialog.text = "";
		if(text != "") {
			fade.FadeInWithCallback(delegate {
				StartCoroutine(DisplayTextWithCallback(action));
			}, fadeSpeed);
		}
	}

	public void DisplayTextSequenceUI(List<TextLine> list) {
		StopAllCoroutines();
		dialog.text = "";
		fade.FadeInWithCallback(delegate {
			StartCoroutine(DisplayTextSequence(list));
		});
	}

	public void DisplayTextSequenceUIWithCallback(List<TextLine> list, Action action) {
		StopAllCoroutines();
		dialog.text = "";
		fade.FadeInWithCallback(delegate {
			StartCoroutine(DisplayTextSequenceWithCallback(list, action));
		});
	}

	IEnumerator DisplayText() {
		for(int i = 0; i < text.Length; i++) {
			dialog.text = dialog.text + text[i];
			yield return new WaitForSeconds(textSpeed);
		}

		yield return new WaitForSeconds(displaySeconds);

		fade.FadeOut(fadeSpeed);
	}

	IEnumerator DisplayTextWithCallback(Action action) {
		for(int i = 0; i < text.Length; i++) {
			dialog.text = dialog.text + text[i];
			yield return new WaitForSeconds(textSpeed);
		}

		yield return new WaitForSeconds(displaySeconds);

		fade.FadeOutWithCallback(action, fadeSpeed);
	}

	IEnumerator DisplayTextSequence(List<TextLine> list) {
		foreach(TextLine line in list) {
			dialog.text = "";
			dialog.color = line.color;

			for(int i = 0; i < line.text.Length; i++) {
				dialog.text = dialog.text + line.text[i];
				yield return new WaitForSeconds(textSpeed);
			}

			yield return new WaitForSeconds(displaySeconds);
		}

		fade.FadeOut(fadeSpeed);
	}

	IEnumerator DisplayTextSequenceWithCallback(List<TextLine> list, Action action) {
		foreach(TextLine line in list) {
			dialog.text = "";
			dialog.color = line.color;

			for(int i = 0; i < line.text.Length; i++) {
				dialog.text = dialog.text + line.text[i];
				yield return new WaitForSeconds(textSpeed);
			}

			yield return new WaitForSeconds(displaySeconds);
		}

		fade.FadeOutWithCallback(action, fadeSpeed);
	}
}

public struct TextLine {
	public string text;
	public Color color;
}