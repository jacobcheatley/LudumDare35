using System;
using UnityEngine;

public enum PlayerIndex
{
    None,
    One,
    Two,
    AI
}

public class Paddle : MonoBehaviour
{
    [Header("Player Specific")]
    [SerializeField] private string axis;
    public PlayerIndex playerIndex;

    [Header("General")]
    [SerializeField] private float startAngle = 0f;
    [SerializeField] private float maxLinearVelocity = 1000f;
    [SerializeField] private float linearAcceleration = 500f;

    private ArenaPolygon arena;
    private float linearVelocity;
    [HideInInspector] public float angle;
    [HideInInspector] public Material material;
    private float multiplier = 1f;
    
    void Start()
	{
	    angle = startAngle;
	    arena = GameObject.FindGameObjectWithTag("Arena").GetComponent<ArenaPolygon>();
        material = GetComponent<Renderer>().material;
        GetComponent<TrailRenderer>().sortingLayerName = "Background";
    }

	void Update()
    {
        float inRadius = (arena.radius * Mathf.Cos(Mathf.PI / arena.sides) - 0.4f);

	    float input = Input.GetAxisRaw(axis);

	    if (Math.Abs(input) > 0.2f)
            linearVelocity -= linearAcceleration * Input.GetAxisRaw(axis) * Time.deltaTime;
        if (Mathf.Sign(input) == Mathf.Sign(linearVelocity) || Math.Abs(input) <= 0.2f)
            linearVelocity = Mathf.Lerp(linearVelocity, 0, Time.deltaTime * 5f);
        
        linearVelocity = Mathf.Clamp(linearVelocity, -maxLinearVelocity, maxLinearVelocity);
	    float angularVelocity = linearVelocity / inRadius;
	    angle += angularVelocity * Time.deltaTime * multiplier;

        angle %= 360f;
	    angle = angle < 0 ? angle + 360f : angle;
        transform.position = Quaternion.Euler(0, 0, -angle) * Vector3.right * inRadius;
	    transform.rotation = Quaternion.Euler(0, 0, 90f - angle);
	}

    public void Reverse()
    {
        multiplier *= -1f;
    }
}
