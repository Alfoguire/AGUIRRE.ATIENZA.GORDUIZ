using UnityEngine;

public class BobbingVisuals : MonoBehaviour
{
    [Header("Bobbing Settings")]
    public float bobSpeed = 2f;   
    public float bobHeight = 0.1f; 

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        float newY = startPosition.y + (Mathf.Sin(Time.time * bobSpeed) * bobHeight);

        transform.localPosition = new Vector3(startPosition.x, newY, startPosition.z);
    }
}