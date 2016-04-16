using System;
using UnityEngine;

public class AnnouncerTextManager : MonoBehaviour
{
    [SerializeField] private GameObject announcerText;

	void Start()
	{
	    GameObject.FindGameObjectWithTag("Arena").GetComponent<ArenaPolygon>().BallExit += BallExit;
	}

    void BallExit(object sender, BallExitArgs e)
    {
        switch (e.LastHit)
        {
            case PlayerIndex.None:
                break;
            case PlayerIndex.One:
                DrawText("+1", new Color(229 / 255f, 125 / 255f, 13 / 255f));
                break;
            case PlayerIndex.Two:
                DrawText("+1", new Color(22 / 255f, 170 / 255f, 231 / 255f));
                break;
            case PlayerIndex.AI:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

	void DrawText(string text, Color color)
	{
	    GameObject textObject = Instantiate(announcerText, Vector3.zero, Quaternion.identity) as GameObject;
        textObject.transform.SetParent(transform);
        textObject.transform.localPosition = Vector3.zero;
	    textObject.GetComponent<AnnouncerText>().Initialise(text, color);
	}
}
