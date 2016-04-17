using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

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
    [Header("Gameplay")]
    [SerializeField] private GameObject ball;
    [Header("UI")]
    [SerializeField] private Canvas menuCanvas;
    [SerializeField] private Canvas gameOverCanvas;
    [SerializeField] private Button restartButton;
    [Header("Sounds")]
    [SerializeField] private DictionaryEventSound eventSounds;
    [SerializeField] private List<AudioClip> countdownSounds;
    [SerializeField] private DictionaryPlayerSound winSounds;
    [SerializeField] private AudioClip ggSound;
    [SerializeField] private AudioMixer audioMixer;

    private ArenaPolygon arena;
    private Paddle[] paddles;
    private CameraRotation cameraRotation;
    [HideInInspector] public int ballCount = 0;
    [HideInInspector] public List<Ball> balls;
    private bool checkForNoBalls = false;
    private List<RandomEvent> weightedEvents;
    private bool begun = false;
    private AudioSource announcerAudio;
    private int maxScore = 10;
    private bool infiniteScore = false;
    private int playerOneScore = 0;
    private int playerTwoScore = 0;
    private bool gameOver = false;
    private float secondsBetweenEvents = 15f;
    private bool noEvents = false;
    private bool speedUpBetweenEvents = true;
    private float baseBallSpeed = 3f;

    void Start()
    {
        balls = new List<Ball>();
        arena = GameObject.FindGameObjectWithTag("Arena").GetComponent<ArenaPolygon>();
        arena.BallExit += BallExit;
        paddles = GameObject.FindGameObjectsWithTag("Player").Select(p => p.GetComponent<Paddle>()).ToArray();
        gameOverCanvas.enabled = false;
        cameraRotation = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraRotation>();
        announcerAudio = GetComponent<AudioSource>();
        weightedEvents = EventWeights.WeightedEvents();
        audioMixer.SetFloat("MusicPitch", 1f);
    }

    private void BallExit(object sender, BallExitArgs e)
    {
        if (!gameOver)
        {
            switch (e.LastHit)
            {
                case PlayerIndex.None:
                    break;
                case PlayerIndex.One:
                    playerOneScore++;
                    if (!infiniteScore && playerOneScore >= maxScore)
                        WinGame(e.LastHit);
                    break;
                case PlayerIndex.Two:
                    playerTwoScore++;
                    if (!infiniteScore && playerTwoScore >= maxScore)
                        WinGame(e.LastHit);
                    break;
            }
        }
    }

    private void WinGame(PlayerIndex playerIndex)
    {
        foreach (Ball ball1 in balls)
        {
            if (ball1 != null)
                ball1.Explode();
        }
        gameOver = true;
        menuCanvas.enabled = false;
        gameOverCanvas.enabled = true;
        StopAllCoroutines();
        if (playerIndex == PlayerIndex.Two)
        {
            ColorBlock restartButtonColors = restartButton.colors;
            restartButtonColors.highlightedColor = Constants.PlayerTwoColour;
            restartButtonColors.pressedColor = Constants.PlayerTwoColour;
            restartButton.colors = restartButtonColors;
        }
        StartCoroutine(LowerMusicPitch());
        StartCoroutine(AnnouncerSayWinner(playerIndex));
        StartCoroutine(ChangeCameraBackground(playerIndex));
    }

    private IEnumerator LowerMusicPitch()
    {
        float currentPitch;
        float endPitch = 0.7f;
        float percent = 0f;
        float duration = 5f;
        while (percent < 1)
        {
            audioMixer.GetFloat("MusicPitch", out currentPitch);
            audioMixer.SetFloat("MusicPitch", Mathf.Lerp(currentPitch, endPitch, percent));
            percent += Time.deltaTime / duration;
            yield return null;
        }
        audioMixer.SetFloat("MusicPitch", endPitch);
    }

    private IEnumerator AnnouncerSayWinner(PlayerIndex playerIndex)
    {
        announcerAudio.PlayOneShot(ggSound);
        yield return new WaitForSeconds(1f);
        announcerAudio.PlayOneShot(winSounds[playerIndex]);
    }

    private IEnumerator ChangeCameraBackground(PlayerIndex playerIndex)
    {
        Camera camera = cameraRotation.gameObject.GetComponent<Camera>();
        Color to = playerIndex == PlayerIndex.One ? Constants.PlayerOneColour : Constants.PlayerTwoColour;
        float percent = 0;
        float time = 5f;
        while (percent < 1)
        {
            camera.backgroundColor = Color.Lerp(camera.backgroundColor, to, percent);
            percent += Time.deltaTime / time;
            yield return null;
        }
        camera.backgroundColor = to;
    }

    public void BeginGame()
    {
        if (!begun)
        {
            begun = true;
            if (!noEvents)
            {
                StartCoroutine(EventLoop());
                if (speedUpBetweenEvents)
                    StartCoroutine(EventSpeedUp());
            }
            else
                StartCoroutine(BeginAgain());
        }
    }

    private IEnumerator EventSpeedUp()
    {
        while (true)
        {
            yield return new WaitForSeconds(secondsBetweenEvents);
            secondsBetweenEvents /= 1.05f;
        }
    }

    private void Update()
    {
        if (checkForNoBalls && ballCount == 0 && !gameOver)
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
            AnnouncerSayEvent(x);
            a();
            yield return new WaitForSeconds(secondsBetweenEvents);
        }
    }

    private void AnnouncerSayEvent(RandomEvent randomEvent)
    {
        announcerAudio.PlayOneShot(eventSounds[randomEvent]);
    }

    #region Random Event Methods
    private void SpawnBall()
    {
        GameObject newBall = Instantiate(ball, Vector3.zero, Quaternion.identity) as GameObject;
        ballCount++;
        Ball newBallComponent = newBall.GetComponent<Ball>();
        newBallComponent.speed = baseBallSpeed;
        balls.Add(newBallComponent);
        checkForNoBalls = true;
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
    #endregion

    private RandomEvent RandomRandomEvent()
    {
        return weightedEvents[UnityEngine.Random.Range(0, weightedEvents.Count)];
    }

    public void SayGenericMessage(string message, float countdown = 3f)
    {
        OnGenericMessage(new GenericMessageArgs { Countdown = countdown, Message = message, Color = Color.black });
        if (countdown > 0f)
            StartCoroutine(AnnouncerSayCountDown((int)countdown));
    }

    private IEnumerator AnnouncerSayCountDown(int countdown)
    {
        for (int i = countdown; i >= 0; i--)
        {
            announcerAudio.PlayOneShot(countdownSounds[i]);
            yield return new WaitForSeconds(1f);
        }
    }

    public void SetScoreLimit(float value)
    {
        maxScore = (int)value;
        if (maxScore == 31) // TODO: FIX Ugly hardcoding
            infiniteScore = true;
    }

    public void SetEventTime(float value)
    {
        secondsBetweenEvents = (int)value;
        if (secondsBetweenEvents == 31) // TODO: FIX Ugly hardcoding
            noEvents = true;
    }

    public void SetSpeedUp(bool value)
    {
        speedUpBetweenEvents = value;
    }

    public void SetBallSpeed(float value)
    {
        baseBallSpeed = value;
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