using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using TMPro;
public class PlayerController : MonoBehaviour
{
    public TextMeshProUGUI thoughtDisplay;
    public TextMeshProUGUI inspectPrompt;
    public TextMeshProUGUI itemFoundMessage;
    public Inventario inventory;
    public Camera cinemachineCamera;
    public Volume damageVolume;
    public float speed = 5.0f;
    public float mouseSensitivity = 2.0f;
    public float jumpForce = 5.0f;
    public float gravity = -9.81f;
    public float maxPitch = 90.0f;
    public float minPitch = -90.0f;
    float xRotation = 0f;
    float yRotation = 0f;
    public AudioClip sonidoSalto;
    public AudioClip sonidoBajaVida; // Clip de sonido para baja vida
    public Healthbar healthBar;

    public bool isInventoryOpen = false;
    private CharacterController controller;
    private float pitch = 0.0f;

    public float runSpeed = 10.0f;
    private bool canJump = true;
    private float verticalVelocity = 0f;

    public Arma armaScript;

    public float currentHealth;
    public float maxHealth = 10;

    public bool canMove = true;
    public KeyInventory keyInventory;
    public float aimingSpeed = 2.5f;
    private bool isAiming = false;

    public float nivelDeMiedo = 0f;
    public float incrementoMiedo = 0.1f;
    public float decrementoMiedo = 0.01f;
    public float maxMiedo = 1f;
    [SerializeField] private float fearRadius = 10.0f; // Nuevo rango de miedo visible en el editor
    [SerializeField] private bool showFearRadius = true;

    private bool inTriggerZone = false;
    private DoorController currentDoor;

    public Vector3 posicionSpawnPlayer;

    // Variables para daño por caída
    public float fallThreshold = 5.0f;
    public float fallDamageMultiplier = 2.0f;
    private float lastGroundedY;
    private bool isFalling = false;

    private AudioSource audioSource;

    void Start()
    {
        keyInventory = GetComponent<KeyInventory>();
        if (keyInventory == null)
        {
            Debug.LogError("KeyInventory component not found on player.");
        }
        controller = GetComponent<CharacterController>();
        controller.slopeLimit = 45.0f; // Ajusta esto según tus necesidades

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        canMove = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        posicionSpawnPlayer = transform.position;
        lastGroundedY = transform.position.y; // Inicializa con la posición inicial del jugador

        audioSource = GetComponent<AudioSource>();
    }

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(gameObject);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void RecibirDaño(float cantidad)
    {
        currentHealth -= cantidad;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= maxHealth * 0.4f) // Si la vida es menor o igual al 40%
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = sonidoBajaVida;
                audioSource.loop = true; // Hacer que el sonido se repita
                audioSource.Play(); // Reproducir el sonido
            }
        }
        else if (audioSource.clip == sonidoBajaVida && audioSource.isPlaying)
        {
            audioSource.Stop(); // Detener el sonido si la vida está por encima del 40%
        }

        if (currentHealth <= 0)
        {
            Morir();
        }
    }

    public void Reiniciar()
    {
        transform.position = posicionSpawnPlayer;
        canMove = true;
        currentHealth = maxHealth;
        isInventoryOpen = false;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(maxHealth);
        audioSource.Stop(); // Detener el sonido al reiniciar
    }

    public void Morir()
    {
        print("Muerto");
        GameManager.instance.GameOver();
        canMove = false;
    }

    void Update()
    {
        if (canMove && !isInventoryOpen)
        {
            HandleCameraRotation();
            MovePlayer();
            ApplyCameraTremble(nivelDeMiedo);
            UpdateFearLevel();
            UpdateAimingStatus();

            if (inTriggerZone && Input.GetKeyDown(KeyCode.E) && currentDoor != null)
            {
                currentDoor.ToggleDoor(GetComponent<KeyInventory>());
            }

            CheckFallDamage();
        }
    }

    void HandleCameraRotation()
    {
        if (!isInventoryOpen)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, minPitch, maxPitch);
            yRotation += mouseX;

            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
    }

    void UpdateFearLevel()
    {
        bool enemySeen = false;
        int layerMask = LayerMask.GetMask("Enemy", "Obstaculos");

        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, fearRadius, layerMask);

        foreach (var enemy in enemiesInRange)
        {
            if (enemy.CompareTag("Enemy1") || enemy.CompareTag("Enemy2"))
            {
                Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
                float dotProduct = Vector3.Dot(transform.forward, directionToEnemy);

                if (dotProduct > 0.5f)
                {
                    if (!IsBlockedByObstacle(transform.position, enemy.transform.position))
                    {
                        float fearIncrement = enemy.CompareTag("Enemy1") ? incrementoMiedo * 2 : incrementoMiedo * 0.5f;
                        nivelDeMiedo = Mathf.Min(nivelDeMiedo + fearIncrement * Time.deltaTime, maxMiedo);
                        enemySeen = true;
                        break;
                    }
                }
            }
        }

        if (!enemySeen)
        {
            nivelDeMiedo = Mathf.Max(nivelDeMiedo - (decrementoMiedo * 2) * Time.deltaTime, 0);
        }
    }

    void OnDrawGizmos()
    {
        if (showFearRadius)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, fearRadius);
        }
    }

    bool IsBlockedByObstacle(Vector3 startPosition, Vector3 enemyPosition)
    {
        Vector3 direction = enemyPosition - startPosition;
        float distance = direction.magnitude;
        direction.Normalize();

        if (Physics.Raycast(startPosition, direction, out RaycastHit hit, distance, LayerMask.GetMask("Obstaculos")))
        {
            return true;
        }
        return false;
    }

    void ApplyCameraTremble(float fearLevel)
    {
        float fearThreshold = 0.5f;

        if (fearLevel > fearThreshold)
        {
            float trembleIntensity = (fearLevel - fearThreshold) / (maxMiedo - fearThreshold);
            float trembleAmount = trembleIntensity * 0.3f;

            float tremblePitch = Random.Range(-trembleAmount, trembleAmount);
            float trembleYaw = Random.Range(-trembleAmount, trembleAmount);

            Quaternion originalRotation = cinemachineCamera.transform.localRotation;
            cinemachineCamera.transform.localRotation = Quaternion.Euler(originalRotation.eulerAngles.x + tremblePitch, originalRotation.eulerAngles.y + trembleYaw, originalRotation.eulerAngles.z);
        }
        else
        {
            Quaternion targetRotation = Quaternion.Euler(pitch, 0, 0);
            cinemachineCamera.transform.localRotation = Quaternion.Lerp(cinemachineCamera.transform.localRotation, targetRotation, Time.deltaTime * 5);
        }
    }

    public void EnableMovement(bool enable)
    {
        canMove = enable;
    }

    void AdjustVerticalCameraRotation(float rotation)
    {
        float fearThreshold = 0.5f;
        if (nivelDeMiedo > fearThreshold)
        {
            float trembleIntensity = (nivelDeMiedo - fearThreshold) / (maxMiedo - fearThreshold);
            float trembleAmount = trembleIntensity * 0.3f;
            rotation += Random.Range(-trembleAmount, trembleAmount);
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, maxPitch, minPitch);
        yRotation += mouseX;
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    void UpdateAimingStatus()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            isAiming = true;
        }
        else if (Input.GetButtonUp("Fire2"))
        {
            isAiming = false;
        }
    }

    void MovePlayer()
    {
        float currentSpeed = speed;

        if (Input.GetKey(KeyCode.LeftShift) && !isAiming)
        {
            currentSpeed = runSpeed;
            if (armaScript != null) armaScript.precisionActual = armaScript.precisionDesdeCadera * 1.5f;
        }
        else if (!isAiming)
        {
            currentSpeed = speed;
            if (armaScript != null) armaScript.precisionActual = armaScript.precisionDesdeCadera;
        }
        else
        {
            currentSpeed = aimingSpeed;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.right * x + transform.forward * z;

        if (controller.isGrounded)
        {
            if (isFalling)
            {
                float fallDistance = lastGroundedY - transform.position.y;
                if (fallDistance > fallThreshold)
                {
                    float damage = (fallDistance - fallThreshold) * fallDamageMultiplier;
                    RecibirDaño(damage);
                }
                isFalling = false;
            }
            lastGroundedY = transform.position.y;
            verticalVelocity = -1f;
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            if (!isFalling)
            {
                isFalling = true;
            }
            verticalVelocity += gravity * Time.deltaTime;
        }

        moveDirection.y = verticalVelocity;
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);
    }

    void CheckFallDamage()
    {
        if (controller.isGrounded && isFalling)
        {
            float fallDistance = lastGroundedY - transform.position.y;
            if (fallDistance > fallThreshold)
            {
                float damage = (fallDistance - fallThreshold) * fallDamageMultiplier;
                RecibirDaño(damage);
            }
            isFalling = false;
        }
        else if (!controller.isGrounded)
        {
            if (!isFalling)
            {
                lastGroundedY = transform.position.y;
                isFalling = true;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PuertaPuzzle"))
        {
            PuertaAbiertaPuzzle puerta = other.GetComponent<PuertaAbiertaPuzzle>();
            if (puerta != null)
            {
                puerta.SetPlayerCerca(true);
                puerta.SetPlayerInventory(keyInventory);
                Debug.Log("SetPlayerInventory called.");
            }
        }
        if (other.CompareTag("DoorTrigger"))
        {
            inTriggerZone = true;
            currentDoor = other.GetComponentInParent<DoorController>();
        }
        if (other.CompareTag("ItemThoughtTrigger"))
        {
            ItemThoughtTrigger thoughtTrigger = other.GetComponent<ItemThoughtTrigger>();
            if (thoughtTrigger != null)
            {
                thoughtTrigger.thoughtDisplay = thoughtDisplay;
                thoughtTrigger.inspectPrompt = inspectPrompt;
                thoughtTrigger.itemFoundMessage = itemFoundMessage;
                thoughtTrigger.ShowInspectPrompt();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PuertaPuzzle"))
        {
            PuertaAbiertaPuzzle puerta = other.GetComponent<PuertaAbiertaPuzzle>();
            if (puerta != null)
            {
                puerta.SetPlayerCerca(false);
            }
        }
        if (other.CompareTag("DoorTrigger"))
        {
            inTriggerZone = false;
            currentDoor = null;
        }
        if (other.CompareTag("ItemThoughtTrigger"))
        {
            ItemThoughtTrigger thoughtTrigger = other.GetComponent<ItemThoughtTrigger>();
            if (thoughtTrigger != null)
            {
                thoughtTrigger.HideInspectPrompt();
                thoughtTrigger.HideThought();
            }
        }
    }

    public void AdjustFearIncrement(float amount, float duration)
    {
        StartCoroutine(AdjustFearIncrementCoroutine(amount, duration));
    }

    private IEnumerator AdjustFearIncrementCoroutine(float amount, float duration)
    {
        incrementoMiedo -= amount;
        incrementoMiedo = Mathf.Max(0, incrementoMiedo);
        yield return new WaitForSeconds(duration);
        incrementoMiedo += amount;
    }

    public void AdjustSpeed(float amount, float duration)
    {
        StartCoroutine(AdjustSpeedCoroutine(amount, duration));
    }

    private IEnumerator AdjustSpeedCoroutine(float amount, float duration)
    {
        speed += amount;
        yield return new WaitForSeconds(duration);
        speed -= amount;
    }

    public void SetFearIncrement(float targetValue, float duration)
    {
        StartCoroutine(SetFearIncrementCoroutine(targetValue, duration));
    }

    private IEnumerator SetFearIncrementCoroutine(float targetValue, float duration)
    {
        float originalIncrementoMiedo = incrementoMiedo;
        incrementoMiedo = targetValue;
        yield return new WaitForSeconds(duration);
        incrementoMiedo = originalIncrementoMiedo;
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.SetHealth(currentHealth);
    }
}