using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionDatabase",menuName = "Missions/Mission Database")]

public class MissionDatabase : ScriptableObject
{
    public List<MissionData> missions = new();
}