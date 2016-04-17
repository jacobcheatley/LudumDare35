using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    [SerializeField] private float lifeTime;

	void Start()
	{
	    Destroy(gameObject, lifeTime);
	}
}
