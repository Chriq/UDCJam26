using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuCanvas;
    private bool isPaused = false;

    public void TogglePause() {
        if(isPaused) {
            pauseMenuCanvas.SetActive(false);
            Resume();
        } else {
            pauseMenuCanvas.SetActive(true);
            Pause();
        }
    }

    private void Pause() {
        Time.timeScale = 0f;
        isPaused = true;
    }

    private void Resume() {
        Time.timeScale = 1f;
        isPaused = false;
    }

	public void Quit() {
		Application.Quit();
	}
}
