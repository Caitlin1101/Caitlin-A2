using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed = 5f; 

    LayerMask ground;

    //Jump Variables
    public float apexHeight;
    public float apexTime;

    public float gravityScale;
    float terminalSpeed = 5;
    public float coyoteTime;

    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        ground = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
    void Update()
    {
        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
        Vector2 playerInput = new Vector2();
        MovementUpdate(playerInput);



        //Jump Code
        if(IsGrounded())
        {
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
        }

        //makes the jump move
        transform.Translate(new Vector2(0, apexTime) * Time.deltaTime);
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        if(Input.GetKey(KeyCode.RightArrow))
        {
            transform.position = transform.position + new Vector3(2, 0, 0) * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position = transform.position + new Vector3(-2, 0, 0) * Time.deltaTime;
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
