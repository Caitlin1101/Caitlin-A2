using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed = 5f; 
    public LayerMask groundMask;

    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
        Vector2 playerInput = new Vector2();
        MovementUpdate(playerInput);
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
        bool groundDetect = Physics2D.Raycast(transform.position, transform.up, 1f, groundMask);
        if(groundDetect == true){
            return true;
        }else {
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
