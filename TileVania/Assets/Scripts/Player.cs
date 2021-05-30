using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    #region Config
    [SerializeField] private float speed=1;
    [SerializeField] private float jumpSpeed = 1;
    [SerializeField] private float climbSpeed = 5;
    [SerializeField] private Vector2 deathKick = new Vector2(25f,25f);
    #endregion
    #region State
    bool isAlive = true;
    bool isClimbing = false;
    #endregion

    #region Cached Components
    private Rigidbody2D myRigidbody;
    private CapsuleCollider2D myBodyCollider;
    private BoxCollider2D myFeetCollider;
    private Animator myAnimator;
    float gravityScaleAtStart;
    #endregion

    #region Message then methods
    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
        isAlive = true;
    }

    private void Update()
    {
        if (isAlive)
        {
            Run();
            ClimbLadder();
            Jump();
            FlipSprite();
            Die();
        }
    }


    private void Run()
    {

        float controlThrow = Input.GetAxis("Horizontal");
        Vector2 playerVelocity = new Vector2(controlThrow * speed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("Running", playerHasHorizontalSpeed);
        
  
    }


    private void ClimbLadder()
    {
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ladder")))
        {
            myAnimator.SetBool("Climbing", false);
            myRigidbody.gravityScale = gravityScaleAtStart;
            return;
        }
        myRigidbody.gravityScale = 0;
        float verticalInput = Input.GetAxis("Vertical");
        Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, verticalInput * climbSpeed);
        myRigidbody.velocity = climbVelocity;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("Climbing", playerHasVerticalSpeed);

    }

    private void Jump()
    {
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            return;
        }
        if (Input.GetButtonDown("Jump"))
        { 
            Vector2 jumpVelocityToAdd = new Vector2(0f,jumpSpeed);
            myRigidbody.velocity += jumpVelocityToAdd;

        }
    }


    private void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2 (Mathf.Sign(myRigidbody.velocity.x),1f);
        }
    }

    private void Die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy","Traps")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            GetComponent<Rigidbody2D>().velocity = deathKick;
        }
    }



    #endregion
}
