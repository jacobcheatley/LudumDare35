using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;

	void Start()
	{
	    rb = GetComponent<Rigidbody2D>();
	    Debug.Log(GetComponent<CircleCollider2D>());
	}

    void Update()
    {
        rb.MovePosition(rb.position + Vector2.up * Time.deltaTime);
    }

	void OnTriggerExit2D(Collider2D other)
	{
	    Debug.Log("Exited");
	}
}
