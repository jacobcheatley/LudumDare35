using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float speed = 3f;
//    [SerializeField] private float angularSpread = 8f;

    [HideInInspector] public Rigidbody2D rb;
    private Renderer rend;
    private TrailRenderer trail;
    [HideInInspector] public PlayerIndex lastHit;

	void Start()
	{
        lastHit = PlayerIndex.None;
	    rb = GetComponent<Rigidbody2D>();
//	    rb.velocity = UnityEngine.Random.insideUnitCircle.normalized * speed;
        rb.velocity = Vector2.right * speed;
	    rend = GetComponent<Renderer>();
	    trail = GetComponent<TrailRenderer>();
	    trail.sortingLayerName = "Background";
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            //TODO: Fix side pushing bug - Maybe use magnitude checking?
            Paddle paddle = other.gameObject.GetComponent<Paddle>();
            Vector2 paddlePosition = paddle.gameObject.transform.position;
            Vector2 positionDifference = (Vector2)transform.position - paddlePosition;
            rb.velocity = (positionDifference.normalized + -paddlePosition.normalized).normalized * speed; // Half between -velocity and position difference
            trail.material = rend.material = paddle.material;
            lastHit = paddle.playerIndex;
        }
        /* OLD:
        if (other.gameObject.tag == "Player")
        {
            Paddle paddle = other.gameObject.GetComponent<Paddle>();
            float reflectionAngle = 180f - paddle.angle + UnityEngine.Random.Range(-angularSpread, angularSpread);
            rb.velocity = Quaternion.Euler(0, 0, reflectionAngle) * Vector2.right * speed;
            trail.material = rend.material = paddle.material;
            lastHit = paddle.playerIndex;
        }
        */
    }

    public void Explode()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<Controller>().ballCount--;
        //Particle stuff
        Destroy(gameObject);
    }

    public void SpeedUp()
    {
        speed *= 1.5f;
        rb.velocity *= 1.5f;
    }

    public void SpeedDown()
    {
        speed /= 1.5f;
        rb.velocity /= 1.5f;
    }

    public void Reverse()
    {
        rb.velocity *= -1f;
    }
}
