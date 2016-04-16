using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [SerializeField] private Text pausedText;

    private bool paused = false;
    private bool canToggle = true;

	void OnGUI()
	{
	    if (Input.GetKeyDown(KeyCode.Escape))
	    {
	        if (!canToggle)
	            return;

            if (paused)
            {
                Time.timeScale = 1f;
                paused = false;
                Color temp = pausedText.color;
                temp.a = 0f;
                pausedText.color = temp;
            }
            else
            {
                Time.timeScale = 0f;
                paused = true;
                Color temp = pausedText.color;
                temp.a = 0.5f;
                pausedText.color = temp;
            }

	        canToggle = false;
	    }

	    if (Input.GetKeyUp(KeyCode.Escape))
            canToggle = true;
    }
}
