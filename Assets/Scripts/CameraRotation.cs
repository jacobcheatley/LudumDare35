using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField]
    private float speed = 10f;

	void Update()
	{
	    transform.Rotate(Vector3.forward, Time.deltaTime * speed);
	}
}
