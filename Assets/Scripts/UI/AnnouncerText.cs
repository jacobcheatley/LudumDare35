using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AnnouncerText : MonoBehaviour
{
    private float distance;
    private float lifeTime;
    private Text textComponent;

	void Awake()
	{
	    textComponent = GetComponent<Text>();
	}

    public IEnumerator FloatAway()
    {
        Vector2 initialPosition = transform.position;
        float startTime = Time.time;
        float endTime = startTime + lifeTime;
        Vector3 destination = initialPosition + UnityEngine.Random.insideUnitCircle.normalized * distance;
        while (Time.time < endTime)
        {
            float percent = (Time.time - startTime) / lifeTime;
            textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 1 - percent);
            transform.position = Vector2.Lerp(initialPosition, destination, percent);
            yield return null;
        }
        Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(FloatAway());
    }
    
    public void Initialise(string text, Color textColor, float duration, float distance)
    {
        textComponent.text = text;
        textComponent.color = textColor;
        lifeTime = duration;
        this.distance = distance;
    }
}
