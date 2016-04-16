using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private PlayerIndex playerIndex;

    private int score = 0;
    private Text text;

	void Start()
	{
        ArenaPolygon arena = GameObject.FindGameObjectWithTag("Arena").GetComponent<ArenaPolygon>();
        arena.BallExit += BallExit;
	    text = GetComponent<Text>();
	}

    private void BallExit(object sender, BallExitArgs e)
    {
        if (e.LastHit == playerIndex)
        {
            score++;
            text.text = score.ToString();
        }
    }
}
