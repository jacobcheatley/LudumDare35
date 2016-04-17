using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

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

    private void Pause()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }
}
