using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    private Canvas canvas;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
    }

	void Update()
	{
	    if (Input.GetKeyDown(KeyCode.Escape))
	    {
	        canvas.enabled = !canvas.enabled;
	        Pause();
	    }
    }

    public void Pause()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }

    public void Restart()
    {
        SceneManager.LoadScene("Main");
    }

    public void Quit()
    {
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
