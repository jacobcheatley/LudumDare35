using System.Collections;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField]
    private float speed = 10f;

	void Update()
	{
	    transform.Rotate(Vector3.forward, Time.deltaTime * speed);
	}

    public void CameraSpin()
    {
        StartCoroutine(Spin());
    }

    private IEnumerator Spin()
    {
        float initialSpeed = speed;
        float startTime = Time.time;
        float endTime = startTime + 2f;
        while (Time.time < endTime)
        {
            float percent = (Time.time - startTime) / 2f;
            speed = initialSpeed * (1 + (100 * percent * (1 - percent))); // Quadratic roots 0, 1 and max y = big
            yield return null;
        }
        speed = initialSpeed;
    }

    public void StartShake(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0.0f;
        Vector3 originalCamPos = Camera.main.transform.position;

        while (elapsed < duration)
        {

            elapsed += Time.deltaTime;

            float percentComplete = elapsed / duration;
            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

            // map value to [-1, 1]
            float x = Random.value * 2.0f - 1.0f;
            float y = Random.value * 2.0f - 1.0f;
            x *= magnitude * damper;
            y *= magnitude * damper;

            Camera.main.transform.position = new Vector3(x, y, originalCamPos.z);

            yield return null;
        }
        Camera.main.transform.position = originalCamPos;
    }

    public void SpeedUp()
    {
        speed *= 1.5f;
    }

    public void Reverse()
    {
        speed *= -1f;
    }
}
