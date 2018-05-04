using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class PlayerMovementController : NetworkBehaviour {

    public float speed;
    Rigidbody body;
    float startTime;
    bool isInAir;
    bool attacking = false;
    bool server_atk1;
    bool server_atk2;
    float server_h;
    float server_v;
    bool temp_atk1;
    bool temp_atk2;
    float temp_h;
    float temp_v;
    bool server_j;
    bool temp_j;
    public bool attacking2;
    GameObject target;
    public float playerDamage = 0;
    public bool hurt = false;
    GameObject[] inRange = new GameObject[4];
    Quaternion server_turn;
    Quaternion temp_turn;
    Ray rA;
    RaycastHit aHit;

    // Animator anim;
    // Use this for initialization
    /*void Start() {
        body = GetComponent<Rigidbody>();
        
    }*/

    public override void OnStartServer()
    {
        body = GetComponent<Rigidbody>();
    }

    [Command]
    void CmdMove(float h, float v, bool j, Quaternion t, bool a1, bool a2)
    {
        server_h = h;
        server_v = v;
        server_j = j;
        server_turn = t;
        server_atk1 = a1;
        server_atk2 = a2;
        //print(a2);
    }

    // Update is called once per frame
    void Update() {

        if(isClient && isLocalPlayer)
        {
            //get Inputs
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            bool j = Input.GetButtonDown("Jump");
            bool a1 = Input.GetButtonDown("Fire1");
            bool a2 = Input.GetButtonDown("Fire2");
            Quaternion turn = new Quaternion();
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
                if (Physics.Raycast(r, out hit))
                {
                    Vector3 playertoMouse = hit.point - transform.position;
                    playertoMouse.y = 0;
                    turn = Quaternion.LookRotation(playertoMouse);
                    //body.MoveRotation(newRot);
                }
      

            //Call CmdMove only if inputs change
            if (h != temp_h || v != temp_v || j != temp_j || turn != temp_turn || a1 != temp_atk1 || a2 != temp_atk2)
            {
                CmdMove(h, v, j, turn, a1, a2);
                temp_h = h;
                temp_v = v;
                temp_j = j;
                temp_turn = turn;
                temp_atk1 = a1;
                temp_atk2 = a2;
            }       
        }
        if (isServer)
        {
            // Move the rigid body
            if ((server_v == 0 || server_h == 0) && !attacking && !hurt)//Stops the player from sliding when releasing movement buttons by setting velocity and angular velocity values to 0
            {
                if(server_v == 0)
                {
                    body.velocity = new Vector3(body.velocity.x, body.velocity.y, 0);
                    body.angularVelocity = new Vector3(body.angularVelocity.x, body.angularVelocity.y, 0);
                }
                if (server_h == 0)
                {
                    body.velocity = new Vector3(0, body.velocity.y, body.velocity.z);
                    body.angularVelocity = new Vector3(0, body.angularVelocity.y, body.angularVelocity.z);
                }

            }
            if(body.velocity.magnitude > 10 && !attacking && !hurt)//my not-so-great way of clamping player speed
            {
                body.velocity = body.velocity.normalized * 10;
            }
            body.MoveRotation(server_turn); //turn player towards mouse
            body.AddForce(speed * (new Vector3(server_h, 0, server_v))); //move player
            if (transform.position.y > .48f && transform.position.y < .51f)// ground check
            {
                isInAir = false;
            }
            else
            {
                isInAir = true;
            }
            if (server_j && !isInAir) //Jump if on ground and player hits space bar
            {
                server_j = false;
                body.AddForce(7 * (new Vector3(0, 1, 0)), ForceMode.Impulse);
            }
            if(!attacking && server_atk1)
            {
                AttackOne();
            }
            if(!attacking2 && server_atk2)
            {
                AttackTwo();
            }
        }
    }

    //Moves the player (not using anymore)
    //void MovePlayer(float h, float v)
     //{
         //move.Set(h, 0, v);
        // move = move.normalized * speed * Time.deltaTime;
        // body.MovePosition(transform.position + move);
  //   }


    //Turns Player towards the mouse's position on the platform
    Quaternion TurnPlayer()
    {
        if(isClient && isLocalPlayer)
        {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit))
            {
                Vector3 playertoMouse = hit.point - transform.position;
                playertoMouse.y = 0;
                Quaternion newRot = Quaternion.LookRotation(playertoMouse);
                //body.MoveRotation(newRot);
                return newRot;
            }
            else
            {
                return new Quaternion();
            }

        }
        else
        {
            return new Quaternion();
        }


    }
    void AttackOne()//Charge forward 
    {
        
        attacking = true;
        body.AddForce(transform.forward * 20, ForceMode.Impulse);
        Invoke("endAttack", .5f);
        print("attacked enemy" + playerDamage);
    }
    void AttackTwo() {
        attacking2 = true;
        target.GetComponent<PlayerMovementController>().playerDamage += 5;
        target.GetComponent<PlayerMovementController>().hurt = true;
        target.GetComponent<PlayerMovementController>().endHurt();
        target.GetComponent<Rigidbody>().AddForce(0, 5, 0, ForceMode.Impulse);
    }
    public void endAttack()
    {
        attacking = false;
        attacking2 = false;
    }
    public void endHurt()
    {
        Invoke("stopHurt", 1f);
    }
    void stopHurt()
    {
        hurt = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (attacking && collision.collider.tag=="Player")
        {
            collision.collider.GetComponent<PlayerMovementController>().playerDamage += 10;
            collision.collider.GetComponent<PlayerMovementController>().hurt = true;
            collision.collider.GetComponent<PlayerMovementController>().endHurt();
            collision.collider.GetComponent<Rigidbody>().AddForce(body.velocity.normalized * collision.collider.GetComponent<PlayerMovementController>().playerDamage, ForceMode.Impulse);
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }

       else if (attacking && collision.collider.tag == "Enemy")
       {
            collision.collider.GetComponent<EnemyAI>().playerDamage += 10;
            collision.collider.GetComponent<EnemyAI>().hurt = true;
            collision.collider.GetComponent<EnemyAI>().endHurt();
            collision.collider.GetComponent<Rigidbody>().AddForce(body.velocity.normalized * collision.collider.GetComponent<EnemyAI>().playerDamage, ForceMode.Impulse);
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
       }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            target = other.gameObject;
            /*for (int i = 0; i < inRange.Length; i++)
            {
                if(inRange[i] == null)
                {
                    inRange[i] = other.gameObject;
                    
                    print(other.name + " added to " + i);
                    break;
                }

            }*/
        }
        /*if(other.tag == "Player" && attacking2)
        {
            //other.GetComponent<PlayerMovementController>().playerDamage += 5;
            //other.GetComponent<PlayerMovementController>().hurt = true;
            //other.GetComponent<PlayerMovementController>().endHurt();
            //other.GetComponent<Rigidbody>().AddForce(0, 5, 0, ForceMode.Impulse);
            print("Should work");
        }
        else if(other.tag == "Player" && !attacking2)
        {
            print("in range, not attacking");
        }else
        {
            print("not in range, not attacking");
        }*/
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            /*for (int i = 0; i < inRange.Length; i++)
            {
                if (inRange[i] == other.gameObject )
                {
                    inRange[i] = null;
                    print("removed from " + i);
                }

            }*/

            target = null;
        }
    }
}
