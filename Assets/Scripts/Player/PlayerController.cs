using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public Camera cinemachineCamera;
    public float speed = 5.0f;
    public float mouseSensitivity = 2.0f;
    public float jumpForce = 5.0f;
    public float maxPitch = 90.0f;
    public float minPitch = -90.0f;
    float xRotation = 0f;
    float yRotation = 0f;
    public AudioClip sonidoSalto;
    public Healthbar healthBar;


    private Rigidbody rb;
    private float pitch = 0.0f;

    public float runSpeed = 10.0f; 
    private bool canJump = true;

    public Arma armaScript;

    public float currentHealth;
    public float maxHealth = 10;

    public bool canMove = true;

    public float aimingSpeed = 2.5f; // Velocidad de movimiento m�s baja cuando apuntas
    private bool isAiming = false; // Controla si el jugador est� apuntando
                                   //Miedo

    public float nivelDeMiedo = 0f;
    public float incrementoMiedo = 0.1f; // Cantidad de miedo que aumenta por frame al ver a un enemigo
    public float decrementoMiedo = 0.01f; // Cantidad de miedo que disminuye por frame cuando no ve a enemigos
    public float maxMiedo = 1f; // M�ximo nivel de miedo
    //Inventario
    public Inventory playerInventory;
    public ItemPickup currentItemPickup;


   
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        canMove = true; // Asegura que el jugador puede moverse

        Cursor.visible = false; // Oculta el cursor
        Cursor.lockState = CursorLockMode.Locked;

    }
    public void RecibirDa�o(float cantidad)
    {
        currentHealth -= cantidad;
        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            // Manejar la muerte del jugador
            Morir();
        }
    }
    public void Reiniciar()
    {
        canMove = true; // Permite el movimiento
        currentHealth = maxHealth; // Restablece la salud
                                   // Restablecer cualquier otro estado necesario aqu�
    }
    public void Morir()
    {
        // L�gica de muerte del jugador aqu�...


        print("Muerto");
    }

    void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        ApplyCameraTremble(nivelDeMiedo);
        if (Input.GetKeyDown(KeyCode.E) && currentItemPickup != null)
        {
            currentItemPickup.Pickup();
            currentItemPickup = null;
        }
        
        UpdateFearLevel();

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minPitch, maxPitch);
        yRotation += mouseX;

        // Aplica la rotaci�n usando Quaternion directamente
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);

        UpdateAimingStatus();

        if (canMove)
        {
            MovePlayer();
        }

        HandleJumping();

        if (armaScript != null)
        {
            armaScript.ActualizarEstadoDeApuntado(isAiming, nivelDeMiedo, maxMiedo);
        }
    }
   
    void UpdateFearLevel()
    {
        bool enemySeen = false;
        int layerMask = LayerMask.GetMask("Enemy", "Obstaculos");  // Incluye las layers de enemigos y obst�culos
        float sphereRadius = 5.0f; // Configura el radio de tu SphereCast

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, sphereRadius, transform.forward, 100, layerMask);
        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Enemy1") || hit.collider.CompareTag("Enemy2"))
            {
                // Verificar si hay obst�culos entre el jugador y el enemigo detectado
                if (!IsBlockedByObstacle(transform.position, hit.point))
                {
                    float fearIncrement = hit.collider.CompareTag("Enemy1") ? incrementoMiedo * 2 : incrementoMiedo * 0.5f;
                    nivelDeMiedo = Mathf.Min(nivelDeMiedo + fearIncrement * Time.deltaTime, maxMiedo);
                    enemySeen = true;
                    break;  // Salir del bucle si ya se ha encontrado un enemigo visible
                }
            }
        }

        if (!enemySeen)
        {
            nivelDeMiedo = Mathf.Max(nivelDeMiedo - (decrementoMiedo * 2) * Time.deltaTime, 0);
        }
    }
    bool IsBlockedByObstacle(Vector3 startPosition, Vector3 enemyPosition)
    {
        Vector3 direction = enemyPosition - startPosition;
        float distance = direction.magnitude;
        direction.Normalize();

        // Chequea si hay un obst�culo en el camino
        if (Physics.Raycast(startPosition, direction, out RaycastHit hit, distance, LayerMask.GetMask("Obstaculos")))
        {
            return true; // Hay un obst�culo
        }
        return false; // No hay obst�culos
    }
    void ApplyCameraTremble(float fearLevel)
    {
        // Define el umbral a la mitad del m�ximo miedo, que es 0.5 si maxMiedo es 1
        float fearThreshold = 0.5f;

        if (fearLevel > fearThreshold)
        {
            // Calcula la cantidad de temblor basada en cu�nto el nivel de miedo excede el umbral
            float trembleIntensity = (fearLevel - fearThreshold) / (maxMiedo - fearThreshold); // Esto normaliza el temblor de 0 a 1
            float trembleAmount = trembleIntensity * 0.3f; // Controla la intensidad del temblor para que sea sutil

            // Calcula variaciones aleatorias para el pitch y el yaw de la c�mara
            float tremblePitch = Random.Range(-trembleAmount, trembleAmount);
            float trembleYaw = Random.Range(-trembleAmount, trembleAmount);

            // Aplica el temblor a la rotaci�n actual de la c�mara
            Quaternion originalRotation = cinemachineCamera.transform.localRotation;
            cinemachineCamera.transform.localRotation = Quaternion.Euler(originalRotation.eulerAngles.x + tremblePitch, originalRotation.eulerAngles.y + trembleYaw, originalRotation.eulerAngles.z);
        }
        else
        {
            // Restablece la rotaci�n de la c�mara suavemente si el miedo est� por debajo del umbral
            Quaternion targetRotation = Quaternion.Euler(pitch, cinemachineCamera.transform.localEulerAngles.y, 0);
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

        // Cambia la velocidad y la precisi�n si el jugador est� corriendo
        if (Input.GetKey(KeyCode.LeftShift) && !isAiming) // Aseg�rate de que el jugador no pueda correr mientras apunta
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
        rb.velocity = new Vector3(moveDirection.x * currentSpeed, rb.velocity.y, moveDirection.z * currentSpeed);
    }

    void HandleJumping()
    {
        if (Input.GetButtonDown("Jump") && canJump)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            canJump = false;
            if (armaScript != null) armaScript.precisionActual = armaScript.precisionDesdeCadera * 2.0f;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            canJump = true;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.SetHealth(currentHealth);
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.SetHealth(currentHealth);
    }
 

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            canJump = false;
        }
    }
    public void AdjustSpeed(float amount)
    {
        speed += amount;
    }

    public void AdjustFearDecrement(float multiplier)
    {
        decrementoMiedo *= multiplier;
    }
   
   

}