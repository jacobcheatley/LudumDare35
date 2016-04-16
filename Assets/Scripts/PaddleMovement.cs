using System;
using System.Linq;
using UnityEngine;

public class PaddleMovement : MonoBehaviour
{
    [SerializeField]
    private float startAngle = 0f;
    [SerializeField]
    private float angularVelocity = 45f;

    private ArenaPolygon arena;
    private float angle;

	void Start()
	{
	    angle = startAngle;
	    arena = GameObject.FindGameObjectWithTag("Arena").GetComponent<ArenaPolygon>();
	}

	void Update()
	{
	    angle -= angularVelocity * Time.deltaTime;
	    angle %= 360f;
	    angle = angle < 0 ? angle + 360f : angle;
        transform.position = Quaternion.Euler(0, 0, -angle) * Vector3.right * (arena.radius * Mathf.Cos(Mathf.PI / arena.sides) - 0.4f);

	    transform.rotation = Quaternion.Euler(0, 0, 90f - angle);
	}
}
