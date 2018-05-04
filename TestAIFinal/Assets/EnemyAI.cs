using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

    Animator anim;
    CharacterController charController;
    //public Collider spearCollider;
    public float speed = 2.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    private Vector3 moveDirection = Vector3.zero;
    public GameObject player;
    public Collider platform;
    private bool canJump = true;
    float playerxDiff, playerzDiff;
    bool isRunning, isJumping, isAttacking;
    GameObject target;
    public bool attacking = false;
    public bool hurt = false;
    Rigidbody body;
    public float playerDamage = 0;

    void Start()
    {
        charController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        StartCoroutine(InvokeWalk());
        StartCoroutine(InvokeJump());
        StartCoroutine(InvokeAttack());
        body = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        playerxDiff = transform.position.x - player.transform.position.x;
        playerzDiff = transform.position.z - player.transform.position.z;

        if (isAttacking)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("attacking"))
            {
                anim.ResetTrigger("jumping");
                anim.SetTrigger("attacking");
            }

        }
        else if (isRunning)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("running"))
            {
                anim.SetBool("running", true);
            }

        }
        else if (isJumping) {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("jumping"))
            {
                anim.ResetTrigger("attacking");
                anim.SetTrigger("jumping");
            }

        }
        if (charController.isGrounded)
        {
            canJump = true;
        }

        //If enemy has fallen off platform
        else if (canJump && !charController.isGrounded && (transform.position.x > 11 || transform.position.x < -11 || (transform.position.z > 11 || transform.position.z < -11)))
        {
            isJumping = true;

            transform.LookAt(player.transform.position);
            if (transform.position.x > 11)
            {
                moveDirection = new Vector3(-2, 0, 0);

            }
            else if (transform.position.x < -11)
            {
                moveDirection = new Vector3(2, 0, 0);

            }
            else if (transform.position.z > 11)
            {
                moveDirection = new Vector3(0, 0, -2);

            }
            else if (transform.position.z < -11)
            {
                moveDirection = new Vector3(0, 0, 2);

            }
            moveDirection = transform.TransformDirection(moveDirection);
            //Multiply it by speed.
            moveDirection *= speed;
            Invoke("JumpOnce", .15f);
            canJump = false;
        }
        //Applying gravity to the controller
        moveDirection.y -= gravity * Time.deltaTime;
        //Making the character move
        charController.Move(moveDirection * Time.deltaTime);

    }
    /*
    private void LaunchAttack(Collider col) {
        Collider[] cols = Physics.OverlapSphere(col.bounds.center, 2, LayerMask.GetMask("Fight"));
        foreach (Collider c in cols) {
                float damage = 10;
            isAttacking = true;
            c.SendMessageUpwards("TakeDamage", damage);
        }
    }*/

    void Attack()
    {
        attacking = true;
        //body.AddForce(transform.forward * 20, ForceMode.Impulse);
        isAttacking = true;
        Invoke("endAttack", .5f);
    }
    public void endAttack()
    {
        attacking = false;
    }
    public void endHurt()
    {
        Invoke("stopHurt", 1f);
    }
    public void stopHurt()
    {
        hurt = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (attacking && collision.collider.tag == "Player")
        {
            collision.collider.GetComponent<PlayerMovementController>().playerDamage += 10;
            print("attacked player " + playerDamage);
            collision.collider.GetComponent<PlayerMovementController>().hurt = true;
            collision.collider.GetComponent<PlayerMovementController>().endHurt();
            collision.collider.GetComponent<Rigidbody>().AddForce(body.velocity.normalized * collision.collider.GetComponent<PlayerMovementController>().playerDamage, ForceMode.Impulse);
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }
    }

    void JumpOnce() {
        
        moveDirection.y = 7;
    }

    IEnumerator InvokeAttack()
    {
        while (true)
        {
            if (Mathf.Abs(playerxDiff) < 1.5f && Mathf.Abs(playerzDiff) < 1.5f)
            {
                Attack();
            }
            else {
                isAttacking = false;
                attacking = false;
            }
            yield return new WaitForSeconds(Random.Range(1.0f, 3.0f));
        }

    }

    IEnumerator InvokeJump() {
        while (true) {
            Jump(Random.Range(2,5));
            yield return new WaitForSeconds(Random.Range(1.0f, 3.0f));
        }
        
    }

    void Jump(float jumpPower) {
        moveDirection.y = jumpPower;
        isJumping = true;
    }

    IEnumerator InvokeWalk() {
        while (true) {
            Walk();
            isRunning = true;
            yield return new WaitForSeconds(Random.Range(1, 2));
        }
    }

    void Walk() {
        if (charController.isGrounded)
        {
            transform.LookAt(player.transform.position);
            //Feed moveDirection with input.
            moveDirection = new Vector3(Random.Range(0, 2.0f), 0, Random.Range(0, 2.0f));
            moveDirection = transform.TransformDirection(moveDirection);
            //Multiply it by speed.
            moveDirection *= speed;
            canJump = true;

        }
    }
}
