using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovementAdvanced : MonoBehaviour
{
    //her yap�lacak hareket i�lemi i�in de�i�kenler tan�mland�.


    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;


    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        air
    }

    private void Start()
    {     //rigid body al�nd�
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
         //z�plama durumuna m�saitlik aktive edildi
        readyToJump = true;

    }

    private void Update()
    {
        // grounded olup olmama durum kontrol�
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        //girilecek inputa g�re h�z ve durum kontrolleri sa�land�
        MyInput();
        SpeedControl();
        StateHandler();

        //  grounded durumuna gelinceye kadar drag 0 a �ekildi
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // z�plama durumlar�n�n uygunlupuna g�re ger�ekle�mesi sa�land�
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

    }

    private void StateHandler()
    {       //stateler belirlendi (ba�lang��ta biri se�ilmek �zere)
        // Ko�ma durumu
        if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        // y�r�me durumu
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        // z�plama durumu
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        // Hareket edilecek y�n hesaplamas�
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // yer hareketi
         if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // hava hareketi
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {

        // ground veya hava da h�z limitlenmesi sa�land�
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        
    }

    private void Jump()
    {
        // z�plama ger�ekle�tirildi
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {  //z�plama sonras� durum resetlendi
        readyToJump = true;

    }
}