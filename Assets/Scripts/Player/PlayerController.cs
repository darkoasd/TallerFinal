using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public CinemachineVirtualCamera cinemachineCamera;
    public float speed = 5.0f;
    public float mouseSensitivity = 2.0f;
    public float jumpForce = 5.0f;
    public float maxPitch = 80.0f;
    public float minPitch = -80.0f;
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

    public float aimingSpeed = 2.5f; // Velocidad de movimiento más baja cuando apuntas
    private bool isAiming = false; // Controla si el jugador está apuntando
                                   //Miedo
    public float nivelDeMiedo = 0f;
    public float incrementoMiedo = 0.1f; // Cantidad de miedo que aumenta por frame al ver a un enemigo
    public float decrementoMiedo = 0.01f; // Cantidad de miedo que disminuye por frame cuando no ve a enemigos
    public float maxMiedo = 1f; // Máximo nivel de miedo

    void Start()
    {
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
                                   // Restablecer cualquier otro estado necesario aquí
    }
    public void Morir()
    {
        // Lógica de muerte del jugador aquí...


        print("Muerto");
    }
    void Update()
    {
        // Maneja la entrada del mouse para la rotación
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);
        AdjustVerticalCameraRotation(-mouseY);

        // Verifica si el jugador está apuntando
        UpdateAimingStatus();

        // Movimiento del jugador
        if (canMove)
        {
            MovePlayer();
        }

        // Maneja la acción de saltar
        HandleJumping();

        // Verifica y actualiza el nivel de miedo
        UpdateFearLevel();

        // Actualiza la precisión basada en el apuntado y el nivel de miedo
        if (armaScript != null)
        {
            armaScript.ActualizarEstadoDeApuntado(isAiming, nivelDeMiedo, maxMiedo);
        }
    }
    void UpdateFearLevel()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100))
        {
            // Verifica si el Raycast golpea a un enemigo y ajusta el miedo basándote en el tipo de enemigo
            switch (hit.collider.tag)
            {
                case "Enemy1":
                    nivelDeMiedo = Mathf.Min(nivelDeMiedo + (incrementoMiedo * 2) * Time.deltaTime, maxMiedo); // Incremento doble para EnemyType1
                    break;
                case "Enemy2":
                    nivelDeMiedo = Mathf.Min(nivelDeMiedo + (incrementoMiedo * 0.5f) * Time.deltaTime, maxMiedo); // Incremento más lento para EnemyType2
                    break;
                default:
                    // Si no es un tipo de enemigo específico, puedes elegir no modificar el miedo o decrementarlo
                    nivelDeMiedo = Mathf.Max(nivelDeMiedo - decrementoMiedo * Time.deltaTime, 0);
                    break;
            }
        }
        else
        {
            // No se detectó a un enemigo, decrementa el miedo gradualmente
            nivelDeMiedo = Mathf.Max(nivelDeMiedo - (decrementoMiedo * 2) * Time.deltaTime, 0); // Ajusta este decremento como prefieras
        }
    }
    public void EnableMovement(bool enable)
    {
        canMove = enable;
    }
    void AdjustVerticalCameraRotation(float rotation)
    {
        pitch = Mathf.Clamp(pitch + rotation, minPitch, maxPitch);
        cinemachineCamera.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
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


}