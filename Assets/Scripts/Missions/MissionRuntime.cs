using UnityEngine;

public class MissionRuntime
{
    public MissionData Data;

    public int Progress { get; private set; }

    public bool Claimed { get; private set; }

    public bool Completed => Progress >= Data.targetValue;

    public bool CanClaim => Completed && !Claimed;

    public float ProgressPercentage => Data.targetValue > 0 ? Mathf.Clamp01((float)Progress / Data.targetValue) : 0f;

    public string ProgressText => $"{Progress}/{Data.targetValue}";

    public MissionRuntime(MissionData data)
    {
        Data = data;
    }

    public void AddProgress(int amount)
    {
        if (Completed)
            return;

        Progress += amount;

        Progress = Mathf.Min(Progress, Data.targetValue);
    }
}