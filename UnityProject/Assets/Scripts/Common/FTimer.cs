using System;

public class FTimer
{
    private float interval;
    private float elapsed;
    bool started;

    public float Interval { get { return interval; } set { interval = value; } }
    public bool Started { get { return started; } }

    public int Hours { get { return (int)(elapsed / 3600) % 24; } }
    public int Minutes { get { return (int)(elapsed / 60) % 60; } }
    public int Seconds { get { return (int)(elapsed) % 60; } }
    public int TotalSeconds { get { return (int)(elapsed); } }

    public FTimer()
    {
    }

    public FTimer(float InInterval)
    {
        interval = InInterval;
    }

    public void Start()
    {
        started = true;
        elapsed = 0;
    }

    public void Stop()
    {
        started = false;
    }

    public void Tick(float InDeltaTime)
    {
        if (started == false)
            return;
     
        elapsed += InDeltaTime;
    }

    public bool IsElapsedCheckTime()
    {
        if (started == false)
            return false;

        if (interval <= elapsed)
        {
            elapsed = 0;
            return true;
        }

        return false;
    }

    public string ToString(string InFormat)
    {
        string retVal = InFormat;

        retVal = retVal.Replace("h", Hours.ToString());
        retVal = retVal.Replace("m", Minutes.ToString());
        retVal = retVal.Replace("s", Seconds.ToString());

        return retVal;
    }
}
