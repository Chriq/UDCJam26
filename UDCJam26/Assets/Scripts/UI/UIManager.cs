using UnityEngine;

public class UIManager : MonoBehaviour {
	private static UIManager instance;

	public FadeCanvasController fade;

	public static UIManager Instance {
		get { return instance; }
	}

	private void Awake() {
		instance = this;
	}

	public void SwitchCanvas(CanvasGroup current, CanvasGroup next) {
		fade.canvas = current;
		fade.FadeOutWithCallback(delegate {
			fade.canvas = next;
			fade.FadeInWithCallback(delegate {
				current = next;
			}, 0.5f);
		}, 0.5f);
	}

	public void SwitchCanvas(CanvasGroup current, CanvasGroup next, float duration) {
		fade.canvas = current;
		fade.FadeOutWithCallback(delegate {
			fade.canvas = next;
			fade.FadeInWithCallback(delegate {
				current = next;
			}, duration);
		}, duration);
	}

	public void EnableCanvas(CanvasGroup canvas) {
		fade.canvas = canvas;
		fade.FadeIn(0.5f);
	}

	public void DisableCanvas(CanvasGroup canvas) {
		fade.canvas = canvas;
		fade.FadeOut(0.5f);
	}
}
