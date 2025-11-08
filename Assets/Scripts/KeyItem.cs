using UnityEngine;

public class KeyItem : MonoBehaviour
{
    [Header("Pickup")]
    public AudioClip collectSound;

    [Header("Bobbing Effect")]
    public float bobSpeed = 2f;
    public float bobHeight = 0.2f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float newY = startPosition.y + (Mathf.Sin(Time.time * bobSpeed) * bobHeight);
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SimpleFPC player = other.GetComponent<SimpleFPC>();

            player.hasKey = true;

            Debug.Log("Player collected the key!");

            HintManager.instance.ShowHint("You got the key!");

            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }

            Destroy(gameObject);
        }
    }
}