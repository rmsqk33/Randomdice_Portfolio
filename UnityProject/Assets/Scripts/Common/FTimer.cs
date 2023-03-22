using System;

public class FTimer
{
    private float interval;
    private float elapsed;

    public float Interval { get { return interval; } set { interval = value; } }

    public FTimer()
    {
    }

    public FTimer(float InInterval)
    {
        interval = InInterval;
    }

    public bool IsElapsedCheckTime(float InDeltaTime)
    {
        elapsed += InDeltaTime;
        if (interval <= elapsed)
        {
            ResetElapsedTime();
            return true;
        }

        return false;
    }

    public void ResetElapsedTime()
    {
        elapsed = 0;
    }
}
