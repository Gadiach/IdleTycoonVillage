using UnityEngine;

public class FloatingAnimation : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.5f; 
    [SerializeField] private float speed = 2f;     

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position; 
    }

    private void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}