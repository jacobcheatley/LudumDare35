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
    PaddleSpeedUp,
    PaddleSpeedDown,
    BallSpeedUp,
    BallSpeedDown,
    CameraSpin,
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
    private Camera mainCamera;
    [HideInInspector] public int ballCount = 0;
    private bool checkForNoBalls = false;
    
    void Start()
    {
        arena = GameObject.FindGameObjectWithTag("Arena").GetComponent<ArenaPolygon>();
        paddles = GameObject.FindGameObjectsWithTag("Player").Select(p => p.GetComponent<Paddle>()).ToArray();
        mainCamera = Camera.main;
        StartCoroutine(EventLoop());
    }

    private void Update()
    {
        if (checkForNoBalls && ballCount == 0)
            StartCoroutine(BeginAgain());
    }

    private IEnumerator BeginAgain()
    {
        SayGenericMessage("BEGIN!");
        yield return new WaitForSeconds(3f);
        SpawnBall();
    }

    private void SpawnBall()
    {
        Instantiate(ball, Vector3.zero, Quaternion.identity);
        ballCount++;
    }

    private IEnumerator EventLoop()
    {
        SayGenericMessage("BEGIN!");
        yield return new WaitForSeconds(3f);
        SpawnBall();
        checkForNoBalls = true;
        yield return new WaitForSeconds(secondsBetweenEvents - 3f);
        while (true)
        {
            Action a;
            string message;
            //TODO: Weighted random
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
                    if (arena.radius != arena.maxRadius)
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
                    if (arena.radius != arena.minRadius)
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
                case RandomEvent.PaddleSpeedUp:
                    a = PaddleSpeedUp;
                    message = "■»» ++";
                    break;
                case RandomEvent.PaddleSpeedDown:
                    a = PaddleSpeedDown;
                    message = "■»» --";
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
                    a = CameraSpin;
                    message = "© = ?";
                    break;
                case RandomEvent.CameraSpeedReverse:
                    a = CameraSpeedReverse;
                    message = "««©";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            #endregion
            SayGenericMessage(message);
            yield return new WaitForSeconds(3f);
            a();
            yield return new WaitForSeconds(secondsBetweenEvents - 3f);
        }
    }
        
    private void CameraSpeedReverse()
    {
    }

    private void CameraSpin()
    {
    }

    private void BallSpeedDown()
    {
    }

    private void BallSpeedUp()
    {
    }

    private void PaddleSpeedDown()
    {
    }

    private void PaddleSpeedUp()
    {
    }

    private void ReverseBalls()
    {
    }

    private void ReversePaddles()
    {
    }

    private void SwapPaddles()
    {
    }

    private RandomEvent RandomRandomEvent()
    {
        return (RandomEvent)UnityEngine.Random.Range(0, Enum.GetValues(typeof (RandomEvent)).Length);
    }

    public void SayGenericMessage(string message)
    {
        OnGenericMessage(new GenericMessageArgs { Countdown = 3f, Message = message, Color = Color.black });
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