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

    public void SpeedUp()
    {
        speed += 0.5f;
    }

    public void Reverse()
    {
        speed *= -1f;
    }
}
