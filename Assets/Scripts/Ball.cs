using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;

	void Start()
	{
	    rb = GetComponent<Rigidbody2D>();
	}

    void Update()
    {
        rb.MovePosition(rb.position + Vector2.up * Time.deltaTime);
    }
}
