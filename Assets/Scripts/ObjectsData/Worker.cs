using UnityEngine;

public enum BusinessType { Farming, Engineering, Science }

public class Worker : MonoBehaviour
{
    public BusinessType type;
    public bool available = true;
    public int level = 1;
    public float speedBonus;
    public float incomeBonus; 
}
