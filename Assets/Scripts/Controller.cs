using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField]
    private GameObject ball;

	void Update()
	{
	    if (Input.GetKeyDown(KeyCode.Space))
	        SpawnBall();
	}

    private void SpawnBall()
    {
        Instantiate(ball, Vector3.zero, Quaternion.identity);
    }
}
