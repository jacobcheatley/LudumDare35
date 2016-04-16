using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField]
    private GameObject ball;

	void Update()
	{
        //TODO: Remove this before publishing
	    if (Input.GetKeyDown(KeyCode.Space))
	        SpawnBall();
	}

    private void SpawnBall()
    {
        Instantiate(ball, Vector3.zero, Quaternion.identity);
    }
}
