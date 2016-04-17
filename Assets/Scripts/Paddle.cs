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
    private float hardMaxLinearVelocity = 1300f;
    private float hardLinearAcceleration = 500f;
    private float easyMaxLinearVelocity = 850f;
    private float easyLinearAcceleration = 250f;
    private bool isEasy = false;
    private bool gameStarted = false;

    void Start()
	{
	    angle = startAngle;
	    arena = GameObject.FindGameObjectWithTag("Arena").GetComponent<ArenaPolygon>();
        material = GetComponent<Renderer>().material;
        GetComponent<TrailRenderer>().sortingLayerName = "Background";
        SetupCollider();
	}

    private void SetupCollider()
    {
        EdgeCollider2D polygon = GetComponent<EdgeCollider2D>();
        polygon.points = new Vector2[]
        {
            new Vector2(-0.5f, 0f),
            new Vector2(0f, -0.4f),
            new Vector2(0.5f, 0f),
        };
    }

	void Update()
    {
        float inRadius = (arena.radius * Mathf.Cos(Mathf.PI / arena.sides) - 0.4f);
	    float input;

	    if (gameStarted)
	        input = isAI ? GetAIInput() : Input.GetAxisRaw(axis);
	    else
	        input = Time.time % 15 < 7.5f ? -1f : 1f;

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
        bool noneBall;
        Vector2 targetPosition = GetAITarget(out noneBall);
        if (targetPosition != Vector2.zero)
        {
            // Vector and trig magic
            float dot = Vector2.Dot(Vector2.right, targetPosition);
            float det = targetPosition.y;
            float targetAngle = Mathf.Rad2Deg * Mathf.Atan2(det, dot);
            targetAngle = 360f - (targetAngle < 0 ? 360f + targetAngle : targetAngle);
            float angleDifference = targetAngle - angle;

            if (noneBall && Mathf.Abs(angleDifference) > 130f)
                return 0;

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

    private Vector2 GetAITarget(out bool noneBall)
    {
        List<Ball> balls = controller.balls;
        float shortestTime = 10000f;
        Vector2 targetPosition = Vector2.zero;
        noneBall = true;
        foreach (Ball ball in balls)
        {
            // Find potential targets
            if (ball == null)
                continue;
            if (ball.lastHit != playerIndex)
            {
                if (ball.lastHit == PlayerIndex.None && !noneBall)
                    continue;
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
                        float distanceToEdge = (hit.point - position).magnitude;
                        float time = distanceToEdge / speed;
                        if (time < shortestTime)
                        {
                            if (ball.lastHit != PlayerIndex.None)
                                noneBall = false;

                            shortestTime = time;
                            targetPosition = hit.point;
                        }
                    }
                }
            }
        }
        return targetPosition;
    }

    public void SetAI()
    {
        isAI = true;
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<Controller>();
        if (isEasy)
        {
            maxLinearVelocity = easyMaxLinearVelocity;
            linearAcceleration = easyLinearAcceleration;
        }
        else
        {
            maxLinearVelocity = hardMaxLinearVelocity;
            linearAcceleration = hardLinearAcceleration;
        }
    }

    public void SetEasyAI(bool value)
    {
        isEasy = value;
    }

    public void GameStart()
    {
        gameStarted = true;
    }

    public void Reverse()
    {
        multiplier *= -1f;
    }
}
