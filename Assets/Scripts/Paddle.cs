using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum PlayerIndex
{
    None,
    One,
    Two
}

public class Paddle : MonoBehaviour
{
    [Header("Player Specific")]
    [SerializeField] private string axis;
    public PlayerIndex playerIndex;
    [SerializeField] private bool isAI = false;

    [Header("General")]
    [SerializeField] private float startAngle = 0f;
    [SerializeField] private float maxLinearVelocity = 1000f;
    [SerializeField] private float linearAcceleration = 500f;

    private ArenaPolygon arena;
    private float linearVelocity;
    [HideInInspector] public float angle;
    [HideInInspector] public Material material;
    private float multiplier = 1f;

    //AI Stuff
    private Controller controller;
    
    void Start()
	{
	    angle = startAngle;
	    arena = GameObject.FindGameObjectWithTag("Arena").GetComponent<ArenaPolygon>();
        material = GetComponent<Renderer>().material;
        GetComponent<TrailRenderer>().sortingLayerName = "Background";
        if (isAI)
            controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<Controller>();
	}

	void Update()
    {
        float inRadius = (arena.radius * Mathf.Cos(Mathf.PI / arena.sides) - 0.4f);

	    float input = isAI ? GetAIInput() : Input.GetAxisRaw(axis);

	    if (Math.Abs(input) > 0.2f)
            linearVelocity -= linearAcceleration * input * Time.deltaTime;
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

    private float GetAIInput()
    {
        Vector2 targetPosition = GetAITarget();
        if (targetPosition != Vector2.zero)
        {
            // Vector and trig magic
            float dot = Vector2.Dot(Vector2.right, targetPosition);
            float det = targetPosition.y;
            float targetAngle = Mathf.Rad2Deg * Mathf.Atan2(det, dot);
            targetAngle = 360f - (targetAngle < 0 ? 360f + targetAngle : targetAngle);
            float angleDifference = targetAngle - angle;
            if (Mathf.Abs(angleDifference) < 10f)
                return 0;
            else if (angleDifference < 0)
                return -multiplier * (angleDifference < 180f ? -1f : 1f);
            else
                return -multiplier * (angleDifference < 180f ? 1f : -1f);
        }
        else
        {
            return 0f;
        }
    }

    private Vector2 GetAITarget()
    {
        List<Ball> balls = controller.balls;
        float shortestTime = 10000f;
        Vector2 targetPosition = Vector2.zero;
        foreach (Ball ball in balls)
        {
            // Find potential targets
            if (ball == null)
                continue;
            if (ball.lastHit != playerIndex && ball.lastHit != PlayerIndex.None)
            {
                // Get information about how soon the ball will hit the edge
                float speed = ball.speed;
                Vector2 velocityDir = ball.rb.velocity.normalized;
                Vector2 position = ball.rb.position;
                RaycastHit2D hit = Physics2D.Raycast(position + velocityDir * 11f, -velocityDir, 11f);
                if (hit)
                {
                    if (hit.transform.tag == "Arena")
                    {
                        // Find shortest time
                        Debug.Log("GOT HIT");
                        float distanceToEdge = (hit.point - position).magnitude;
                        float time = distanceToEdge / speed;
                        if (time < shortestTime)
                        {
                            shortestTime = time;
                            targetPosition = hit.point;
                        }
                    }
                }
            }
        }
        return targetPosition;
    }

    public void Reverse()
    {
        multiplier *= -1f;
    }
}
