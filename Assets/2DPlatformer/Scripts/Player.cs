using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : PhysicsObject
{
    public float moveForce;
    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;

    public PlayerState playerState;
    
    public PlayerState previousState;
    public float score;
    
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // Use this for initialization
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    protected override void ComputeVelocity()
    {
        switch (playerState)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Moving: Move();
                break;
            case PlayerState.Jumping: Jump();
                break;
            default:
                break;
        }
        ComputeScore();
        UpdatePlayerAnimation();
    }

    internal void UpdateState(PlayerState newState)
    {
        previousState = playerState;
        playerState = newState;

    }

    public void Move()
    {
        targetVelocity.x = maxSpeed;

    }

    public void Jump()
    {
        if (grounded)
        {
            velocity.y = jumpTakeOffSpeed;
        }
        else
        {
            if (velocity.y > 0)
            {
                velocity.y = velocity.y * 0.5f;
            }
        }
    }

    public void Die()
    {

    }

    public void UpdatePlayerAnimation()
    {
        bool flipSprite = (spriteRenderer.flipX ? (targetVelocity.x > 0.01f) : (targetVelocity.x < 0.01f));
        if (flipSprite)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        animator.SetBool("grounded", grounded);
        animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
    }

    public void ComputeScore()
    {
        var notIdle = (playerState == PlayerState.Idle) ? false : true;
        if (notIdle)
        {
            score += maxSpeed * Time.deltaTime;
        }
    }
}
