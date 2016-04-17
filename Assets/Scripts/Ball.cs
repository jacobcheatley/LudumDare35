using System;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float speed = 3f;
    [SerializeField] private GameObject particleParent;
    [SerializeField] private List<AudioClip> bounceSounds;
    //    [SerializeField] private float angularSpread = 8f;

    [HideInInspector] public Rigidbody2D rb;
    private Renderer rend;
    private TrailRenderer trail;
    [HideInInspector] public PlayerIndex lastHit;
    private AudioSource audioSource;

	void Start()
	{
        lastHit = PlayerIndex.None;
	    rb = GetComponent<Rigidbody2D>();
        rb.velocity = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 2) * 180f) * Vector2.right * speed;
	    rend = GetComponent<Renderer>();
	    trail = GetComponent<TrailRenderer>();
	    trail.sortingLayerName = "Background";
	    audioSource = GetComponent<AudioSource>();
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

            audioSource.PlayOneShot(bounceSounds[UnityEngine.Random.Range(0, bounceSounds.Count)]);
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

    void Update()
    {
        // Fixes radius shrinking bug
        if (transform.position.sqrMagnitude > 30f)
            Explode();
    }

    public void Explode()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<Controller>().ballCount--;
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraRotation>().StartShake(0.2f, 0.05f);
        GameObject newParticleParent = Instantiate(particleParent, transform.position, Quaternion.identity) as GameObject;
        ParticleSystem particle = newParticleParent.transform.GetComponentInChildren<ParticleSystem>();
        float angle = (float)Math.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        newParticleParent.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        particle.GetComponent<Renderer>().material = rend.material;
        particle.Play();
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
