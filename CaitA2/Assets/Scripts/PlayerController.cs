using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed = 5f; 

    LayerMask ground;

    //Rigidbody Vraiables
    public float acceleration, deceleration;
    public float duration = 3;
    private Vector3 velocity = Vector3.zero; 


    //week 12 Variables
    int runSpeed = 2;
    float doubleJump;
    bool jumped = false;
    LayerMask wall;

    //Animation
    public int currentHealth = 10;

    //Jump Variables
    public float apexHeight;
    public float apexTime;

    public float gravityScale;
    float terminalSpeed = 5;
    public float coyoteTime;

    //Enums
    public enum FacingDirection
    {
        left, right
    }

    public enum CharacterState
    {
        idle, walk, jump, die
    }

    public CharacterState currentState = CharacterState.idle;
    public CharacterState previousState = CharacterState.idle;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        ground = LayerMask.GetMask("Ground");
        wall = LayerMask.GetMask("Ground");

        acceleration = speed / duration;
    }

    // Update is called once per frame
    void Update()
    {
        //Animation stuff starts
        previousState = currentState;

        if(IsDead())
        {
            currentState = CharacterState.die;
        }
        switch(currentState)
        {
            case CharacterState.idle:
            if(IsWalking())
            {
                currentState = CharacterState.walk;
            } 
            if(!IsGrounded())
            {
                currentState = CharacterState.jump;
            }
            break;

            case CharacterState.walk:
            if(!IsWalking())
            {
                currentState = CharacterState.idle;
            } 
            if(!IsGrounded())
            {
                currentState = CharacterState.jump;
            }
            break;

            case CharacterState.jump:
            if(IsGrounded())
            {
                if(IsWalking())
                {
                    currentState = CharacterState.walk;
                } else {
                    currentState = CharacterState.idle;
                }
            }
    
            break;
            case CharacterState.die:

            break;
        }
        

        //Animation stuff ends


        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
        Vector2 playerInput = new Vector2();
        MovementUpdate(playerInput);



        //Jump Code starts
        if(IsGrounded())
        {
            jumped = false;
            gravityScale = 1;
            if(Input.GetKey(KeyCode.Space))
            {
                //Jump
                apexTime = Mathf.Sqrt(apexHeight * -2 * (Physics2D.gravity.y * gravityScale));
            }
            if(apexTime < 0){
                //stops you from falling through the world
                apexTime = 0;
            }
        }
        
        if(!IsGrounded())
        {
            //gravity
            apexTime += Physics2D.gravity.y * gravityScale * Time.deltaTime;

            //Terminal Speed Stuff
            if(gravityScale > terminalSpeed){
                gravityScale = terminalSpeed;
            }

            gravityScale = gravityScale + 1;

            //Coyote Time
            if(gravityScale >= 4){
                if(Input.GetKey(KeyCode.RightArrow)){
                    transform.position = transform.position + new Vector3(2, 0, 0) * Time.deltaTime;
                }
                if(Input.GetKey(KeyCode.LeftArrow)){
                    transform.position = transform.position + new Vector3(-2, 0, 0) * Time.deltaTime;
                }
            }

            //Double jump code
            if(jumped == false)
            {
                doubleJump = 1;
            }
            if(jumped == true)
            {
                doubleJump = 0;
            }

            if(Input.GetKeyDown(KeyCode.Space) && doubleJump == 1)
            {
                apexTime = Mathf.Sqrt(apexHeight * -2 * (Physics2D.gravity.y * gravityScale));
                jumped = true;
            }
        }

        //makes the jump move
        transform.Translate(new Vector2(0, apexTime) * Time.deltaTime);

        //jump code ends


        //Climbing Code Starts

        if(IsTouchingWall())
        {
            if(Input.GetKey(KeyCode.UpArrow))
            {
                gravityScale = 0;
                transform.position = transform.position + new Vector3(0, 2, 0) * Time.deltaTime;
            }
        }

        //Climbing Code Ends
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            runSpeed = runSpeed * 2;
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            runSpeed = 2;
        }

        if(Input.GetKey(KeyCode.RightArrow))
        {
            transform.position = transform.position + new Vector3(runSpeed, 0, 0) * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position = transform.position + new Vector3(-runSpeed, 0, 0) * Time.deltaTime;
        }
    }

    public bool IsWalking()
    {
        //Is character moving?
        //Attempt 1, arrow keys
        if(Input.GetKey(KeyCode.RightArrow)){
            return true;
        }else if(Input.GetKey(KeyCode.LeftArrow)){
            return true;
        }else {
            return false;
        }
    }
    public bool IsGrounded()
    {
        //Is player on the ground?
        //Layers, raycasting, do not use collision sensing(more complicated)
        Debug.DrawRay(transform.position, Vector2.down * 1, Color.green, 1);
        bool groundSense = Physics2D.Raycast(transform.position, Vector2.down, 1, ground);
        if(groundSense == true){
            Debug.Log("Grounded");
            return true;
        }else{
            Debug.Log("Airborne");
            return false;
        }
    }

    public bool IsTouchingWall()
    {
        //Draw Vision Lines
        Debug.DrawRay(transform.position, Vector2.right * 1, Color.red, 1);
        Debug.DrawRay(transform.position, Vector2.left * 1, Color.red, 1);

        //Sense if touching
        bool wallSense1 = Physics2D.Raycast(transform.position, Vector2.right, 1, ground);
        bool wallSense2 = Physics2D.Raycast(transform.position, Vector2.left, 1, ground);

        //result
        if(wallSense1 == true || wallSense2 == true){
            Debug.Log("Touching Wall");
            return true;
        }else{
            return false;
        }
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    public FacingDirection GetFacingDirection()
    {
        //Use the 'up' sensitive version of movement, reference space shooter
        //Vector3.up, Vector3.down, Vector3.left, Vector3.right
        if(Input.GetKey(KeyCode.LeftArrow)){
            return FacingDirection.left;
        }
        if(Input.GetKey(KeyCode.RightArrow)){
            return FacingDirection.right;
        }
        return FacingDirection.left;
    }
}
