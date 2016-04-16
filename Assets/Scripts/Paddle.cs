using System;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    [Header("Player Specific")]
    [SerializeField] private string axis;

    [Header("General")]
    [SerializeField] private float startAngle = 0f;
    [SerializeField] private float maxLinearVelocity = 300f;
    [SerializeField] private float linearAcceleration = 300f;
    [SerializeField] private float deceleration = 0.75f;

    private ArenaPolygon arena;
    private float linearVelocity;
    private float angle;
    private Material material;
    
    void Start()
	{
	    angle = startAngle;
	    arena = GameObject.FindGameObjectWithTag("Arena").GetComponent<ArenaPolygon>();
        material = GetComponent<Renderer>().material;
	}

	void Update()
    {
        float inRadius = (arena.radius * Mathf.Cos(Mathf.PI / arena.sides) - 0.4f);

	    float input = Input.GetAxisRaw(axis);

	    if (Math.Abs(input) > 0.2f)
            linearVelocity -= linearAcceleration * Input.GetAxisRaw(axis) * Time.deltaTime;
        if (Mathf.Sign(input) == Mathf.Sign(linearVelocity) || Math.Abs(input) <= 0.2f)
            linearVelocity = Mathf.Lerp(linearVelocity, 0, Time.deltaTime * 10f);
        
        linearVelocity = Mathf.Clamp(linearVelocity, -maxLinearVelocity, maxLinearVelocity);
	    float angularVelocity = linearVelocity / inRadius;
	    angle += angularVelocity * Time.deltaTime;

        angle %= 360f;
	    angle = angle < 0 ? angle + 360f : angle;
        transform.position = Quaternion.Euler(0, 0, -angle) * Vector3.right * inRadius;
	    transform.rotation = Quaternion.Euler(0, 0, 90f - angle);
	}
}
