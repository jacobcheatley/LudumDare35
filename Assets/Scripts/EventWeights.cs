using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class EventWeights
{
    private static Dictionary<RandomEvent, int> weights = new Dictionary<RandomEvent, int>
    {
        { RandomEvent.SidesUp, 6},
        { RandomEvent.SidesDown, 6},
        { RandomEvent.RandomSides, 4},
        { RandomEvent.RadiusUp, 6},
        { RandomEvent.RadiusDown, 6},
        { RandomEvent.RandomRadius, 4},
        { RandomEvent.SwapPaddles, 1},
        { RandomEvent.ReversePaddles, 1},
        { RandomEvent.ReverseBalls, 3},
        { RandomEvent.NewBall, 25},
        { RandomEvent.BallSpeedUp, 10},
        { RandomEvent.BallSpeedDown, 5},
        { RandomEvent.CameraSpin, 1},
        { RandomEvent.CameraSpeedUp, 2},
        { RandomEvent.CameraSpeedReverse, 4}
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
