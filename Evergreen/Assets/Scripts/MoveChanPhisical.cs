using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveChanPhisical : MonoBehaviour
{
    public Rigidbody rdb;
    public GameObject currentCamera;
    public float jumpspeed = 1;
    public float gravity = 20;
    public Animator animator;

    Vector3 movaxis;
    Vector3 moveInput;
    float jumptime;
    bool jumpbtn = false;
    bool grounded = false;
    bool jumpbtndown = false;

    GameObject closeThing;
    float weight;
    FixedJoint joint;

    public Transform rightHandObj, leftHandObj;

    public void SetMoveInput(float h, float v)
    {
        moveInput = new Vector3(h, 0, v);
    }

    public void TriggerJump()
    {
        jumpbtn = true;
        jumpbtndown = true;
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name.Equals("Land"))
        {
            if (PlayerPrefs.HasKey("OldPlayerPosition"))
            {
                transform.position = PlayerPrefsX.GetVector3("OldPlayerPosition");
            }
        }
    }

    void FixedUpdate()
    {
        movaxis = moveInput;

        // Animação de movimento
        if (animator != null)
        {
            Vector3 horizontalVel = new Vector3(rdb.linearVelocity.x, 0, rdb.linearVelocity.z);
            bool isMoving = horizontalVel.magnitude > 0.1f && grounded;
            animator.SetBool("Andando", isMoving);

            float speed = horizontalVel.magnitude;
            animator.speed = grounded ? Mathf.Lerp(1f, 1.5f, speed / 5f) : 1f;
        }

        // Verifica se está no chão
        grounded = false;
        RaycastHit hit;
        float rayLength = 0.6f;
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        if (Physics.Raycast(origin, Vector3.down, out hit, rayLength))
        {
            grounded = true;

            if (jumpbtn)
            {
                jumptime = 0.25f;
            }
        }

        // Pulo
        if (jumpbtn && grounded)
        {
            rdb.AddForce(Vector3.up * jumpspeed, ForceMode.Impulse);
            jumpbtn = false;
        }

        // Gravidade manual
        if (!grounded)
        {
            rdb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
        }

        jumpbtndown = false;

        GroundControl();
    }

    private void GroundControl()
    {
        // Direção relativa à câmera
        Vector3 relativedirection = currentCamera.transform.TransformVector(movaxis).normalized;
        relativedirection = new Vector3(relativedirection.x, 0, relativedirection.z);

        if (grounded)
        {
            // Detecta plano inclinado
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, 1f))
            {
                Vector3 groundNormal = hit.normal;

                // Movimento adaptado ao solo inclinado
                Vector3 moveOnSlope = Vector3.ProjectOnPlane(relativedirection, groundNormal).normalized;
                Vector3 targetVelocity = moveOnSlope * 5f;
                targetVelocity.y = rdb.linearVelocity.y;

                rdb.linearVelocity = Vector3.Lerp(rdb.linearVelocity, targetVelocity, Time.fixedDeltaTime * 10f);

                // Desacelera se não estiver se movendo
                if (moveInput.magnitude < 0.01f)
                {
                    Vector3 vel = rdb.linearVelocity;
                    vel.x = Mathf.Lerp(vel.x, 0, Time.fixedDeltaTime * 10f);
                    vel.z = Mathf.Lerp(vel.z, 0, Time.fixedDeltaTime * 10f);
                    rdb.linearVelocity = vel;
                }
            }
            else
            {
                // Movimento plano sem inclinação
                Vector3 targetVelocity = new Vector3(relativedirection.x * 5, rdb.linearVelocity.y, relativedirection.z * 5);
                rdb.linearVelocity = Vector3.Lerp(rdb.linearVelocity, targetVelocity, Time.fixedDeltaTime * 5f);
            }
        }
        else
        {
            // Movimento no ar
            rdb.AddForce(new Vector3(relativedirection.x * 500, 0, relativedirection.z * 500));
        }

        // Rotação suave apenas ao andar para frente
        if (!joint && relativedirection.sqrMagnitude > 0.01f && moveInput.z > 0f)
        {
            Quaternion rottogo = Quaternion.LookRotation(relativedirection.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rottogo, Time.fixedDeltaTime * 3f);
        }
    }

    void OnAnimatorIK()
    {
        // IK opcional aqui
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.position.y > transform.position.y + 0.05f)
        {
            if (!closeThing)
                closeThing = new GameObject("Handpos");

            weight = 0;
            closeThing.transform.parent = collision.gameObject.transform;
            closeThing.transform.position = collision.GetContact(0).point;
        }
    }

    public void StopMovement()
    {
        moveInput = Vector3.zero;
        movaxis = Vector3.zero;

        if (rdb != null)
        {
            rdb.linearVelocity = Vector3.zero;
            rdb.angularVelocity = Vector3.zero;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Nada necessário aqui
    }
}
