using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    [Header("Win Screen")]
    public GameObject winScreenPanel;

    [Header("Game Objects")]
    public HunterAI hunter; 

    [Header("Audio")]
    public AudioClip winSound;
    public float winSoundVolume = 1.0f;

    private bool hasWon = false; 

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasWon)
        {
            SimpleFPC player = other.GetComponent<SimpleFPC>();

            if (player != null && player.hasKey)
            {
                hasWon = true;
                Debug.Log("Level Complete!");

                if (winSound != null)
                {
                    AudioSource.PlayClipAtPoint(winSound, transform.position, winSoundVolume);
                }

                if (hunter != null)
                {
                    hunter.enabled = false; 
                    
                    var hunterAgent = hunter.GetComponent<UnityEngine.AI.NavMeshAgent>();
                    if (hunterAgent != null)
                        hunterAgent.isStopped = true;
                    
                    var hunterAudio = hunter.GetComponent<AudioSource>();
                    if (hunterAudio != null)
                        hunterAudio.Stop();
                }

                player.enabled = false;
                Cursor.lockState = CursorLockMode.None;

                if (winScreenPanel != null)
                {
                    winScreenPanel.SetActive(true);
                }
            }
            else
            {
                Debug.Log("The exit is locked. I need to find the key.");
                HintManager.instance.ShowHint("You need to find a key.");
            }
        }
    }
}