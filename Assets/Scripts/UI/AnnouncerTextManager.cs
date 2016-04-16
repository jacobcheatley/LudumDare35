using System;
using System.Collections;
using UnityEngine;

public class AnnouncerTextManager : MonoBehaviour
{
    [SerializeField] private GameObject announcerText;

	void Start()
	{
	    ArenaPolygon arena = GameObject.FindGameObjectWithTag("Arena").GetComponent<ArenaPolygon>();
        arena.BallExit += BallExit;
        Controller controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<Controller>();
	    controller.GenericMessage += GenericMessage;
	}

    private void GenericMessage(object sender, GenericMessageArgs e)
    {
        StartCoroutine(CountDownMessage(e));
    }

    private IEnumerator CountDownMessage(GenericMessageArgs e)
    {
        for (int i = 0; i < e.Countdown; i++)
        {
            DrawText((e.Countdown - i).ToString(), Color.black, 1f, 50f);
            yield return new WaitForSeconds(1f);
        }
        DrawText(e.Message, e.Color, 2f, 0f);
    }

    void BallExit(object sender, BallExitArgs e)
    {
        switch (e.LastHit)
        {
            case PlayerIndex.None:
                break;
            case PlayerIndex.One:
                DrawText("+1", new Color(229 / 255f, 125 / 255f, 13 / 255f), 2f, 200f);
                break;
            case PlayerIndex.Two:
                DrawText("+1", new Color(22 / 255f, 170 / 255f, 231 / 255f), 2f, 200f);
                break;
            case PlayerIndex.AI:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

	void DrawText(string text, Color color, float duration, float distance)
	{
	    GameObject textObject = Instantiate(announcerText, Vector3.zero, Quaternion.identity) as GameObject;
        textObject.transform.SetParent(transform);
        textObject.transform.localPosition = Vector3.zero;
	    textObject.GetComponent<AnnouncerText>().Initialise(text, color, duration, distance);
	}
}
