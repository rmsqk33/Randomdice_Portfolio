using System.Collections.Generic;
using UnityEngine;

public class FPathManager : FSingleton<FPathManager>
{
    List<FPath> startPointList = new List<FPath>();
    int pathCount = 0;

    public int StartPointCount { get { return startPointList.Count; }}

    public void OnStartBattle()
    {
        GameObject[] pointList = GameObject.FindGameObjectsWithTag("StartPoint");
        foreach (GameObject point in pointList)
        {
            startPointList.Add(point.GetComponent<FPath>());
        }

        if (startPointList.Count != 0)
        {
            for (FPath path = startPointList[0].NextPath; path != null; path = path.NextPath)
            {
                ++pathCount;
            }
        }
    }

    public Vector2 GetPointInPathByRate(int InStartPointIndex, float InRate)
    {
        if (InStartPointIndex < 0 || startPointList.Count <= InStartPointIndex)
            return Vector2.zero;

        float pathRatePerOnePath = 1.0f / pathCount;
        int pathIndex = (int)Mathf.Min(InRate / pathRatePerOnePath, pathCount - 1);

        FPath startPath = startPointList[InStartPointIndex];
        FPath endPath = startPath.NextPath;
        for (int i = 0; i < pathIndex; ++i)
        {
            startPath = endPath;
            endPath = endPath.NextPath;
        }

        float pathRate = (InRate - pathIndex * pathRatePerOnePath) * pathCount;
        float distance = Vector2.Distance(startPath.WorldPosition, endPath.WorldPosition);
        Vector2 point = Vector2.MoveTowards(startPath.WorldPosition, endPath.WorldPosition, distance * pathRate);

        return point;
    }

    public float GetAngleByRate(int InStartPointIndex, float InRate)
    {
        if (InStartPointIndex < 0 || startPointList.Count <= InStartPointIndex)
            return 0;

        float pathRatePerOnePath = 1.0f / pathCount;
        int pathIndex = (int)Mathf.Min(InRate / pathRatePerOnePath, pathCount - 1);

        FPath startPath = startPointList[InStartPointIndex];
        FPath endPath = startPath.NextPath;
        for (int i = 0; i < pathIndex; ++i)
        {
            startPath = endPath;
            endPath = endPath.NextPath;
        }

        return Vector2.Angle(Vector2.right, startPath.WorldPosition - endPath.WorldPosition);
    }

    public FPath FindStartPoint(int InIndex)
    {
        if (InIndex < 0 || startPointList.Count <= InIndex)
            return null;

        return startPointList[InIndex];
    }

    public delegate void ForeachStartPointHandler(int InIndex, FPath path);
    public void ForeachStartPoint(ForeachStartPointHandler InFunc)
    {
        for(int i = 0; i < startPointList.Count; ++i)
        {
            InFunc(i, startPointList[i]);
        }
    }
}
