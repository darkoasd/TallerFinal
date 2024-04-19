using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class PlayerController : MonoBehaviour
{
    public Camera cinemachineCamera;
    public Volume damageVolume;
    public float speed = 5.0f;
    public float mouseSensitivity = 2.0f;
    public float jumpForce = 5.0f;
    public float maxPitch = 90.0f;
    public float minPitch = -90.0f;
    float xRotation = 0f;
    float yRotation = 0f;
    public AudioClip sonidoSalto;
    public Healthbar healthBar;

    public bool isInventoryOpen = false;
    private Rigidbody rb;
    private float pitch = 0.0f;

    public float runSpeed = 10.0f; 
    private bool canJump = true;

    public Arma armaScript;

    public float currentHealth;
    public float maxHealth = 10;

    public bool canMove = true;

    public float aimingSpeed = 2.5f; // Velocidad de movimiento más baja cuando apuntas
    private bool isAiming = false; // Controla si el jugador está apuntando
                                   //Miedo

    public float nivelDeMiedo = 0f;
    public float incrementoMiedo = 0.1f; // Cantidad de miedo que aumenta por frame al ver a un enemigo
    public float decrementoMiedo = 0.01f; // Cantidad de miedo que disminuye por frame cuando no ve a enemigos
    public float maxMiedo = 1f; // Máximo nivel de miedo
    //Inventario
    


   
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
    public void RecibirDaño(float cantidad)
    {
        currentHealth -= cantidad;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.SetHealth(currentHealth);
        UpdateDamageEffect();
        if (currentHealth <= 0)
        {
            Morir();
        }
    }
    public void Reiniciar()
    {
        canMove = true; // Permite el movimiento
        currentHealth = maxHealth; // Restablece la salud
                                   // Restablecer cualquier otro estado necesario aquí
    }
    public void Morir()
    {
        print("Muerto"); // Mensaje en consola
        GameManager.instance.GameOver(); // Notifica al GameManager que el juego ha terminado
        canMove = false; // Desactiva el movimiento del jugador

        print("Muerto");
    }

    void Update()
    {
        if (canMove)  // Agrega esta condición para asegurarte de que no se ajuste el cursor si el jugador no puede moverse
        {
            if (!isInventoryOpen)  // Solo bloquear el cursor si el inventario no está abierto
            {
                if (Cursor.lockState != CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }

                HandleCameraRotation();
                MovePlayer();
            }

            ApplyCameraTremble(nivelDeMiedo);
            UpdateFearLevel();
            HandleJumping();
            UpdateAimingStatus();
        }

        // Continúa manejo de la rotación de la cámara fuera del bloque `canMove` para asegurar que la cámara pueda ser ajustada aún después de morir
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minPitch, maxPitch);
        yRotation += mouseX;
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
    void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minPitch, maxPitch);
        yRotation += mouseX;

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
    void UpdateFearLevel()
    {
        bool enemySeen = false;
        int layerMask = LayerMask.GetMask("Enemy", "Obstaculos");  // Incluye las layers de enemigos y obstáculos
        float sphereRadius = 5.0f; // Configura el radio de tu SphereCast

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, sphereRadius, transform.forward, 100, layerMask);
        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Enemy1") || hit.collider.CompareTag("Enemy2"))
            {
                // Verificar si hay obstáculos entre el jugador y el enemigo detectado
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

        // Chequea si hay un obstáculo en el camino
        if (Physics.Raycast(startPosition, direction, out RaycastHit hit, distance, LayerMask.GetMask("Obstaculos")))
        {
            return true; // Hay un obstáculo
        }
        return false; // No hay obstáculos
    }
    void ApplyCameraTremble(float fearLevel)
    {
        // Define el umbral a la mitad del máximo miedo, que es 0.5 si maxMiedo es 1
        float fearThreshold = 0.5f;

        if (fearLevel > fearThreshold)
        {
            // Calcula la cantidad de temblor basada en cuánto el nivel de miedo excede el umbral
            float trembleIntensity = (fearLevel - fearThreshold) / (maxMiedo - fearThreshold); // Esto normaliza el temblor de 0 a 1
            float trembleAmount = trembleIntensity * 0.3f; // Controla la intensidad del temblor para que sea sutil

            // Calcula variaciones aleatorias para el pitch y el yaw de la cámara
            float tremblePitch = Random.Range(-trembleAmount, trembleAmount);
            float trembleYaw = Random.Range(-trembleAmount, trembleAmount);

            // Aplica el temblor a la rotación actual de la cámara
            Quaternion originalRotation = cinemachineCamera.transform.localRotation;
            cinemachineCamera.transform.localRotation = Quaternion.Euler(originalRotation.eulerAngles.x + tremblePitch, originalRotation.eulerAngles.y + trembleYaw, originalRotation.eulerAngles.z);
        }
        else
        {
            // Restablece la rotación de la cámara suavemente si el miedo está por debajo del umbral
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

        // Cambia la velocidad y la precisión si el jugador está corriendo
        if (Input.GetKey(KeyCode.LeftShift) && !isAiming) // Asegúrate de que el jugador no pueda correr mientras apunta
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
    private void UpdateDamageEffect()
    {

        float healthPercent = currentHealth / maxHealth;
        Debug.Log("Updating damage effect. Health percent: " + healthPercent);  // Agrega esto para depurar

        if (damageVolume.profile.TryGet(out Bloom bloom))
        {
            bloom.intensity.value = Mathf.Lerp(0.0f, 2.0f, 1 - healthPercent);
        }
       
     
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