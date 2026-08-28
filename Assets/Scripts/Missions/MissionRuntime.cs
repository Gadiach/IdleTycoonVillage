using UnityEngine;

public class MissionRuntime
{
    public MissionData Data { get; }

    public int Progress { get; private set; }
    public int TargetValue { get; private set; }

    public bool Claimed { get; private set; }

    public bool Completed => Progress >= TargetValue;

    public bool CanClaim => Completed && !Claimed;

    public int RemainingProgress => TargetValue - Progress;

    public float ProgressPercentage =>
        TargetValue > 0
            ? Mathf.Clamp01((float)Progress / TargetValue)
            : 0f;

    public string ProgressText => $"{Progress}/{TargetValue}";

    public MissionRuntime(MissionData data)
    {
        Data = data;
        TargetValue = data.targetValue;
    }

    public void AddProgress(int amount)
    {
        if (Completed)
            return;

        Progress += amount;

        Progress = Mathf.Min(Progress, TargetValue);
    }

    public void SetProgress(int progress)
    {
        Progress = Mathf.Min(progress, TargetValue);
    }

    public void SetTargetValue(int targetValue)
    {
        TargetValue = targetValue;
    }

    public void ClaimReward()
    {
        if (!CanClaim)
            return;

        CurrencySystem.Instance.AddCurrency(
            Data.rewardCurrency,
            Data.rewardAmount
        );

        Claimed = true;

        EventManager.Instance.QueueEvent(
            new MissionClaimedEvent(this)
        );
    }
}