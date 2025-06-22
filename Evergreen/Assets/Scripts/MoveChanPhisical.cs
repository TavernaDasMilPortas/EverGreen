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

        // Checagem de solo
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
            jumpbtn = false; // evita aplicar várias vezes
        }

        // Gravidade manual se estiver no ar
        if (!grounded)
        {
            rdb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
        }

        jumpbtndown = false;

        GroundControl();

    }

    private void GroundControl()
    {
        Vector3 relativedirection = currentCamera.transform.TransformVector(movaxis).normalized;
        relativedirection = new Vector3(relativedirection.x, 0, relativedirection.z);

        if (grounded)
        {
            // Primeiro, detecta o plano inclinado em que o personagem está
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, 1f))
            {
                Vector3 groundNormal = hit.normal;

                // Projeta a direção do movimento no plano da rampa
                Vector3 moveOnSlope = Vector3.ProjectOnPlane(relativedirection, groundNormal).normalized;

                // Define a velocidade alvo ao longo da rampa
                Vector3 targetVelocity = moveOnSlope * 5f;
                targetVelocity.y = rdb.linearVelocity.y; // mantém velocidade vertical atual para evitar saltos bruscos

                // Aplica suavização na velocidade para movimento mais natural
                rdb.linearVelocity = Vector3.Lerp(rdb.linearVelocity, targetVelocity, Time.fixedDeltaTime * 10f);

                // Se não está se movendo, desacelera suavemente
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
                // fallback caso não detecte chão, apenas movimenta normalmente (sem inclinação)
                Vector3 targetVelocity = new Vector3(relativedirection.x * 5, rdb.linearVelocity.y, relativedirection.z * 5);
                rdb.linearVelocity = Vector3.Lerp(rdb.linearVelocity, targetVelocity, Time.fixedDeltaTime * 5f);
            }
        }
        else
        {
            rdb.AddForce(new Vector3(relativedirection.x * 500, 0, relativedirection.z * 500));
        }

        if (!joint && relativedirection.sqrMagnitude > 0.01f)
        {
            Quaternion rottogo = Quaternion.LookRotation(relativedirection * 2 + transform.forward);
            transform.rotation = Quaternion.Lerp(transform.rotation, rottogo, Time.fixedDeltaTime * 5f);
        }
    }


    void OnAnimatorIK()
    {
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
        // Zera o input de movimentação
        moveInput = Vector3.zero;
        movaxis = Vector3.zero;

        // Para imediatamente a movimentação física
        if (rdb != null)
        {
            rdb.linearVelocity = Vector3.zero;
            rdb.angularVelocity = Vector3.zero;
        }

    }
    private void OnCollisionExit(Collision collision)
    {
        // Nenhuma ação ao sair da colisão
    }
}
