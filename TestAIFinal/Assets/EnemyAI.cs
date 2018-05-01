using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

    Animator anim;
    CharacterController charController;
    public Collider spearCollider;
    public float speed = 2.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    private Vector3 moveDirection = Vector3.zero;
    public GameObject player;
    public Collider platform;
    private bool canJump = true;

    void Start()
    {
        charController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (charController.isGrounded)
        {
            transform.LookAt(player.transform.position);
            //Feed moveDirection with input.
            moveDirection = new Vector3(Random.Range(0,2.0f), 0, Random.Range(0,2.0f));
            anim.SetBool("running", true);
            moveDirection = transform.TransformDirection(moveDirection);
            //Multiply it by speed.
            moveDirection *= speed;
            StartCoroutine(Jump(Random.Range(2, 6)));
            canJump = true;

        }

        //If enemy has fallen off platform
        else if (canJump && !charController.isGrounded && (transform.position.x > 11 || transform.position.x < -11 || (transform.position.z > 11 || transform.position.z < -11)))
        {
            anim.SetBool("running", false);
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

    private void LaunchAttack(Collider col) {
        Collider[] cols = Physics.OverlapSphere(col.bounds.center, 2, LayerMask.GetMask("Fight"));
        foreach (Collider c in cols) {

         //   if (c.transform.parent.parent = transform)
         //   {
        //   }
          
                float damage = 10;
                c.SendMessageUpwards("TakeDamage", damage);
            

        }
    }

    void JumpOnce() {

        moveDirection.y = 7;
    }
    IEnumerator Jump(float jumpPower) {

        moveDirection.y = jumpPower;
        anim.SetBool("jumping", true);
        yield return new WaitForSeconds(Random.Range(5.0f, 7.5f));
    }
}
