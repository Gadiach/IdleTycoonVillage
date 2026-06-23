public class MissionRuntime
{
    public MissionData Data;

    public int Progress;

    public bool Completed;

    public MissionRuntime(MissionData data)
    {
        Data = data;
        Progress = 0;
        Completed = false;
    }
}