﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using UnityEngine.EventSystems;

public enum RandomEvent
{
    SidesUp,
    SidesDown,
    RandomSides,
    RadiusUp,
    RadiusDown,
    RandomRadius,
    SwapPaddles,
    ReversePaddles,
    ReverseBalls,
    NewBall,
    BallSpeedUp,
    BallSpeedDown,
    CameraSpin,
    CameraSpeedUp,
    CameraSpeedReverse
}

public class Controller : MonoBehaviour
{
    [SerializeField]
    private GameObject ball;
    [Range(5f, 30f)]
    public float secondsBetweenEvents = 20f;

    private ArenaPolygon arena;
    private Paddle[] paddles;
    private CameraRotation cameraRotation;
    [HideInInspector] public int ballCount = 0;
    [HideInInspector] public List<Ball> balls;
    private bool checkForNoBalls = false;
    private List<RandomEvent> weightedEvents;
    private bool begun = false;

    void Start()
    {
        balls = new List<Ball>();
        arena = GameObject.FindGameObjectWithTag("Arena").GetComponent<ArenaPolygon>();
        paddles = GameObject.FindGameObjectsWithTag("Player").Select(p => p.GetComponent<Paddle>()).ToArray();
        cameraRotation = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraRotation>();
        weightedEvents = EventWeights.WeightedEvents();
    }

    public void BeginGame()
    {
        if (!begun)
        {
            begun = true;
            StartCoroutine(EventLoop());
            StartCoroutine(EventSpeedUp());
        }
    }

    private IEnumerator EventSpeedUp()
    {
        while (true)
        {
            yield return new WaitForSeconds(20f);
            secondsBetweenEvents /= 1.05f;
        }
    }

    private void Update()
    {
        if (checkForNoBalls && ballCount == 0)
        {
            checkForNoBalls = false;
            StartCoroutine(BeginAgain());
        }
    }

    private IEnumerator BeginAgain()
    {
        SayGenericMessage("GO");
        yield return new WaitForSeconds(3f);
        SpawnBall();
    }

    private void SpawnBall()
    {
        GameObject newBall = Instantiate(ball, Vector3.zero, Quaternion.identity) as GameObject;
        ballCount++;
        balls.Add(newBall.GetComponent<Ball>());
        checkForNoBalls = true;
    }

    private IEnumerator EventLoop()
    {
        SayGenericMessage("GO");
        yield return new WaitForSeconds(3f);
        SpawnBall();
        yield return new WaitForSeconds(secondsBetweenEvents - 3f);
        while (true)
        {
            Action a;
            string message;
            RandomEvent x = RandomRandomEvent();
            x = RandomEvent.RandomRadius;
            #region Event Switch
            switch (x)
            {
                case RandomEvent.SidesUp:
                    if (arena.sides != arena.maxSides)
                    {
                        a = arena.IncreaseSides;
                        message = "♦ ++";
                    }
                    else
                    {
                        a = arena.DecreaseSides;
                        message = "♦ --";
                    }
                    break;
                case RandomEvent.SidesDown:
                    if (arena.sides != arena.minSides)
                    {
                        a = arena.DecreaseSides;
                        message = "♦ --";
                    }
                    else
                    {
                        a = arena.IncreaseSides;
                        message = "♦ ++";
                    }
                    break;
                case RandomEvent.RandomSides:
                    a = arena.RandomSides;
                    message = "♦ = ?";
                    break;
                case RandomEvent.RadiusUp:
                    if (arena.radius < arena.maxRadius)
                    {
                        a = arena.RadiusUp;
                        message = "R ++";
                    }
                    else
                    {
                        a = arena.RadiusDown;
                        message = "R --";
                    }
                    break;
                case RandomEvent.RadiusDown:
                    if (arena.radius > arena.minRadius)
                    {
                        a = arena.RadiusDown;
                        message = "R --";
                    }
                    else
                    {
                        a = arena.RadiusUp;
                        message = "R ++";
                    }
                    break;
                case RandomEvent.RandomRadius:
                    a = arena.RandomRadius;
                    message = "R = ?";
                    break;
                case RandomEvent.SwapPaddles:
                    a = SwapPaddles;
                    message = "■«»■";
                    break;
                case RandomEvent.ReversePaddles:
                    a = ReversePaddles;
                    message = "««■";
                    break;
                case RandomEvent.ReverseBalls:
                    a = ReverseBalls;
                    message = "««O";
                    break;
                case RandomEvent.NewBall:
                    a = SpawnBall;
                    message = "O ++";
                    break;
                case RandomEvent.BallSpeedUp:
                    a = BallSpeedUp;
                    message = "O»» ++";
                    break;
                case RandomEvent.BallSpeedDown:
                    a = BallSpeedDown;
                    message = "O»» --";
                    break;
                case RandomEvent.CameraSpin:
                    a = cameraRotation.CameraSpin;
                    message = "C = ?";
                    break;
                case RandomEvent.CameraSpeedUp:
                    a = cameraRotation.SpeedUp;
                    message = "C»» ++";
                    break;
                case RandomEvent.CameraSpeedReverse:
                    a = cameraRotation.Reverse;
                    message = "««C";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            #endregion
            SayGenericMessage(message, 0f);
            a();
            yield return new WaitForSeconds(secondsBetweenEvents);
        }
    }

    #region Random Event Methods
    private void BallSpeedDown()
    {
        balls.RemoveAll(ball => ball == null);
        foreach (Ball ball in balls)
            ball.SpeedDown();
    }

    private void BallSpeedUp()
    {
        balls.RemoveAll(b => b == null);
        foreach (Ball ball in balls)
            ball.SpeedUp();
    }

    private void ReverseBalls()
    {
        balls.RemoveAll(b => b == null);
        foreach (Ball ball in balls)
            ball.Reverse();
    }

    private void ReversePaddles()
    {
        foreach (Paddle paddle in paddles)
            paddle.Reverse();
    }

    private void SwapPaddles()
    {
        float tempAngle = paddles[0].angle;
        paddles[0].angle = paddles[1].angle;
        paddles[1].angle = tempAngle;
    }
    #endregion

    private RandomEvent RandomRandomEvent()
    {
        return weightedEvents[UnityEngine.Random.Range(0, weightedEvents.Count)];
    }

    public void SayGenericMessage(string message, float countdown = 3f)
    {
        OnGenericMessage(new GenericMessageArgs { Countdown = countdown, Message = message, Color = Color.black });
    }

    public event EventHandler<GenericMessageArgs> GenericMessage;
    protected virtual void OnGenericMessage(GenericMessageArgs e)
    {
        EventHandler<GenericMessageArgs> handler = GenericMessage;
        if (handler != null)
            handler(this, e);
    }
}

public class GenericMessageArgs : EventArgs
{
    public float Countdown;
    public string Message;
    public Color Color;
}