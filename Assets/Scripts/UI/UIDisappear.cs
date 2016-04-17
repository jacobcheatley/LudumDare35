using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIDisappear : MonoBehaviour
{
    [SerializeField] private Text textComponent;
    [SerializeField] private Image imageComponent;

    public void StartFloatAway()
    {
        StartCoroutine(FloatAway());
    }

    public IEnumerator FloatAway()
    {
        float lifeTime = 1f;
        Vector2 initialPosition = transform.position;
        float startTime = Time.time;
        float endTime = startTime + lifeTime;
        Vector3 destination = initialPosition + UnityEngine.Random.insideUnitCircle.normalized * 50f;
        float initialTextAlpha = 1f;
        float initialImageAlpha = 1f;
        if (textComponent != null)
            initialTextAlpha = textComponent.color.a;
        if (imageComponent != null)
            initialImageAlpha = imageComponent.color.a;
        while (Time.time < endTime)
        {
            float percent = (Time.time - startTime) / lifeTime;
            float alphaMultiplier = 1 - percent;
            if (textComponent != null)
                textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, alphaMultiplier * initialTextAlpha);
            if (imageComponent != null)
                imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, alphaMultiplier * initialImageAlpha);
            transform.position = Vector2.Lerp(initialPosition, destination, percent);
            yield return null;
        }
        Destroy(gameObject);
    }
}
