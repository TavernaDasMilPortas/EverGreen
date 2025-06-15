using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveChanPhisical : MonoBehaviour
{
    public Rigidbody rdb;
    public Animator anim;
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

    public void TriggerAttack()
    {
        anim.SetTrigger("PunchA");
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

        currentCamera = Camera.main.gameObject;
    }



    void FixedUpdate()
    {
        movaxis = moveInput;
        anim.SetFloat("Speed", rdb.linearVelocity.magnitude);

        // Checagem de solo
        grounded = false;
        RaycastHit hit;
        float rayLength = 0.6f;
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        if (Physics.Raycast(origin, Vector3.down, out hit, rayLength))
        {
            grounded = true;
            anim.SetFloat("JumpHeight", hit.distance);

            if (jumpbtn)
            {
                jumptime = 0.25f;
            }
        }
        else
        {
            anim.SetFloat("JumpHeight", 2); // valor indicando que está no ar
        }

        // Pulo
        /*if (jumpbtn && grounded)
        {
            rdb.AddForce(Vector3.up * jumpspeed, ForceMode.Impulse);
            jumpbtn = false; // evita aplicar várias vezes
        }

        // Gravidade manual se estiver no ar
        if (!grounded)
        {
            rdb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
        }*/

        jumpbtndown = false;

        GroundControl();

        if (Input.GetButtonDown("Fire1"))
        {
            anim.SetTrigger("PunchA");
        }
    }

    private void GroundControl()
    {
        Vector3 relativedirection = currentCamera.transform.TransformVector(movaxis).normalized;
        relativedirection = new Vector3(relativedirection.x, 0, relativedirection.z);

        if (grounded)
        {
            Vector3 targetVelocity = new Vector3(relativedirection.x * 5, rdb.linearVelocity.y, relativedirection.z * 5);
            rdb.linearVelocity = Vector3.Lerp(rdb.linearVelocity, targetVelocity, Time.fixedDeltaTime * 5f);

            if (moveInput.magnitude < 0.01f)
            {
                rdb.linearVelocity = Vector3.Lerp(rdb.linearVelocity, new Vector3(0, rdb.linearVelocity.y, 0), Time.fixedDeltaTime * 5f);
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
        if (closeThing)
        {
            Vector3 handDirection = closeThing.transform.position - transform.position;
            float lookto = Vector3.Dot(handDirection.normalized, transform.forward);
            weight = Mathf.Lerp(weight, (lookto * 3 / (Mathf.Pow(handDirection.magnitude, 3))), Time.fixedDeltaTime * 2);

            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, weight);
            anim.SetIKPosition(AvatarIKGoal.RightHand, closeThing.transform.position + transform.right * 0.1f);
            anim.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.identity);

            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, weight);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, closeThing.transform.position - transform.right * 0.1f);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.identity);

            if (weight <= 0)
            {
                Destroy(closeThing);
                if (joint)
                {
                    Destroy(joint);
                }
            }
        }
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

        // Atualiza a animação de velocidade
        if (anim != null)
        {
            anim.SetFloat("Speed", 0f);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        // Nenhuma ação ao sair da colisão
    }
}
