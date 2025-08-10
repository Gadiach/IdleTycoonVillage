using UnityEngine;

[System.Serializable]
public class WorkerData
{
    public BusinessType type;
    public bool available = true;
    public int level = 1;
    public float speedBonus;
    public float incomeBonus;
    public Sprite icon; 
}
