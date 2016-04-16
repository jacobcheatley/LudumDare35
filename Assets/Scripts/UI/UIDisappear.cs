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
        Vector3 destination = initialPosition + UnityEngine.Random.insideUnitCircle.normalized * 4f;
        while (Time.time < endTime)
        {
            float percent = (Time.time - startTime) / lifeTime;
            float alpha = 1 - percent;
            if (textComponent != null)
                textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, alpha);
            if (imageComponent != null)
                imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, alpha);
            transform.position = Vector2.Lerp(initialPosition, destination, percent);
            yield return null;
        }
        Destroy(gameObject);
    }
}
