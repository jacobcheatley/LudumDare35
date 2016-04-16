using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    public float secondsBetweenEvents = 15f;

    private ArenaPolygon arena;
    private Paddle[] paddles;
    private CameraRotation cameraRotation;
    [HideInInspector] public int ballCount = 0;
    private List<Ball> balls;
    private bool checkForNoBalls = false;
    
    void Start()
    {
        balls = new List<Ball>();
        arena = GameObject.FindGameObjectWithTag("Arena").GetComponent<ArenaPolygon>();
        paddles = GameObject.FindGameObjectsWithTag("Player").Select(p => p.GetComponent<Paddle>()).ToArray();
        cameraRotation = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraRotation>();
        StartCoroutine(EventLoop());
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
        SayGenericMessage("BEGIN!");
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
        SayGenericMessage("BEGIN!");
        yield return new WaitForSeconds(3f);
        SpawnBall();
        yield return new WaitForSeconds(secondsBetweenEvents - 3f);
        while (true)
        {
            Action a;
            string message;
            RandomEvent x = RandomRandomEvent();
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
                        message = "r ++";
                    }
                    else
                    {
                        a = arena.RadiusDown;
                        message = "r --";
                    }
                    break;
                case RandomEvent.RadiusDown:
                    if (arena.radius > arena.minRadius)
                    {
                        a = arena.RadiusDown;
                        message = "r --";
                    }
                    else
                    {
                        a = arena.RadiusUp;
                        message = "r ++";
                    }
                    break;
                case RandomEvent.RandomRadius:
                    a = arena.RandomRadius;
                    message = "r = ?";
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
                    message = "© = ?";
                    break;
                case RandomEvent.CameraSpeedUp:
                    a = cameraRotation.SpeedUp;
                    message = "©»» ++";
                    break;
                case RandomEvent.CameraSpeedReverse:
                    a = cameraRotation.Reverse;
                    message = "««©";
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

    private RandomEvent RandomRandomEvent()
    {
        //TODO: Weighted random
        return (RandomEvent)UnityEngine.Random.Range(0, Enum.GetValues(typeof (RandomEvent)).Length);
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