using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class EventWeights
{
    private static Dictionary<RandomEvent, int> weights = new Dictionary<RandomEvent, int>
    {
        { RandomEvent.SidesUp, 3},
        { RandomEvent.SidesDown, 3},
        { RandomEvent.RandomSides, 1},
        { RandomEvent.RadiusUp, 3},
        { RandomEvent.RadiusDown, 3},
        { RandomEvent.RandomRadius, 1},
        { RandomEvent.SwapPaddles, 1},
        { RandomEvent.ReversePaddles, 1},
        { RandomEvent.ReverseBalls, 1},
        { RandomEvent.NewBall, 10},
        { RandomEvent.BallSpeedUp, 5},
        { RandomEvent.BallSpeedDown, 2},
        { RandomEvent.CameraSpin, 1},
        { RandomEvent.CameraSpeedUp, 1},
        { RandomEvent.CameraSpeedReverse, 3}
    };

    public static List<RandomEvent> WeightedEvents()
    {
        List<RandomEvent> result = new List<RandomEvent>();
        foreach (KeyValuePair<RandomEvent, int> pair in weights)
        {
            for (int i = 0; i < pair.Value; i++)
                result.Add(pair.Key);
        }
        return result;
    }
}
