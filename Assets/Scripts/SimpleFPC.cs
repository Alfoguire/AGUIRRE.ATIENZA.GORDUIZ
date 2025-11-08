using UnityEngine;
using System.Collections; // Required for Coroutines

public class SimpleFPC : MonoBehaviour
{
    [Header("References")]
    public Transform cam; 
    public Light flashlight; 

    [Header("Movement")]
    public float walkSpeed = 6f;
    public float sprintSpeed = 10f;
    public float jumpForce = 5f;
    private float currentSpeed;

    [Header("Stamina")]
    public float maxStamina = 100.0f;
    public float staminaDrainRate = 10.0f; 
    public float staminaRegenRate = 10.0f; 
    [HideInInspector] public float currentStamina;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;
    private float verticalLook = 0f;

    [Header("Flashlight")] 
    public float flashlightIntensity = 6000f; 
    public AudioSource flashlightAudio; 
    private bool isFlashlightOn = false; 

    [Header("Noise")]
    public float walkNoise = 4f;
    public float sprintNoise = 10f;
    [HideInInspector] public float currentMicrophoneNoise = 0f;
    [HideInInspector] public float currentNoiseLevel;

    [Header("Inventory")]
    [HideInInspector] public bool hasKey = false;
    
    [Header("Death")]
    public AudioClip deathSound;
    public float deathSoundDelay = 0f; 
    public float deathSoundVolume = 0.5f;
    public GameObject deathScreenPanel; 
    private bool isDead = false;

    private Rigidbody rb;
    private bool isSprinting = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        currentStamina = maxStamina;
        currentSpeed = walkSpeed;

        if (flashlight != null)
        {
            flashlight.intensity = 0f;
            isFlashlightOn = false;
        }
        
        if (deathScreenPanel != null)
            deathScreenPanel.SetActive(false);
    }

    void Update()
    {
        if (isDead) return;

        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        transform.Rotate(0f, Input.GetAxis("Mouse X") * mouseSensitivity, 0f);
        verticalLook -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalLook = Mathf.Clamp(verticalLook, -80f, 80f);
        cam.localRotation = Quaternion.Euler(verticalLook, 0f, 0f);
        
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        bool isTryingToMove = (horizontal != 0 || vertical != 0);

        HandleSprinting(isTryingToMove);
        HandleStamina();

        float movementNoise = 0f;
        if (isTryingToMove)
        {
            movementNoise = isSprinting ? sprintNoise : walkNoise;
        }
        currentNoiseLevel = movementNoise + currentMicrophoneNoise;
        
        Vector3 moveDir = (transform.forward * vertical + transform.right * horizontal).normalized;
        Vector3 newVelocity = new Vector3(moveDir.x * currentSpeed, rb.linearVelocity.y, moveDir.z * currentSpeed);
        rb.linearVelocity = newVelocity;

        if (Input.GetButtonDown("Jump") && IsGrounded())
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        
        HandleFlashlight();
    }
    
    public void Die(AudioSource hunterAudio)
    {
        if (isDead) return;
        isDead = true;
        
        rb.isKinematic = true; 
        this.enabled = false; 
        
        Debug.Log("Player has been caught!");

        StartCoroutine(PlayDeathSequence(hunterAudio));
    }
    
    private IEnumerator PlayDeathSequence(AudioSource hunterAudio)
    {
        yield return new WaitForSeconds(deathSoundDelay);

        if (hunterAudio != null)
        {
            hunterAudio.Stop();
        }

        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, cam.position, deathSoundVolume);
        }

        yield return new WaitForSeconds(1.5f);

        if (deathScreenPanel != null)
        {
            deathScreenPanel.SetActive(true);
        }
        
        Cursor.lockState = CursorLockMode.None;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    void HandleSprinting(bool isTryingToMove)
    {
        bool sprintInput = Input.GetKey(KeyCode.LeftShift);

        if (sprintInput && isTryingToMove && currentStamina > 0)
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }

        currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
    }

    void HandleStamina()
    {
        if (isSprinting)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
        }
        else if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }
    
    void HandleFlashlight()
    {
        if (Input.GetMouseButtonDown(1)) 
        {
            if (flashlight != null && flashlightAudio != null) 
            {
                isFlashlightOn = !isFlashlightOn; 

                if (isFlashlightOn)
                {
                    flashlight.intensity = flashlightIntensity;
                    flashlightAudio.pitch = 1f;
                    flashlightAudio.Play();
                }
                else
                {
                    flashlight.intensity = 0f;
                    flashlightAudio.pitch = -1f;
                    flashlightAudio.Play();
                }
            }
            else
            {
                Debug.LogWarning("Flashlight or Flashlight Audio not assigned!");
            }
        }
    }
}