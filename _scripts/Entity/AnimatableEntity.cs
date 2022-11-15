using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public abstract class AnimatableEntity : Entity
{
    [SerializeField]
    protected Animator animator;


    [SerializeField]
    int inputCount = 4;
    int actionInputCount = 0;
    protected bool[] actionInput;
    [SerializeField]
    protected ActionKey[] entityActions;
    bool movementLocked = false;

    [ExecuteInEditMode]
    public void UpdateActionKeys()
    {
        actionInputCount = inputCount;
        entityActions = new ActionKey[actionInputCount];
    }

    public override void Update()
    {
        if (gravity.getGrounded())
        {
            velocity.y = -2f;
        }
        velocity.y += gravity.getValue() * Time.deltaTime;


        if (!movementLocked)
        {
            EntityState finalizedState = state;
            Vector2 currentMoveInput = Vector2.zero;
            bool[] currentActionInput = new bool[actionInputCount]; 


            rotateTowards(new Vector3(currentMoveInput.x, 0f, currentMoveInput.y));

            if (gravity.getGrounded())
            {
                finalizedState = EntityState.grounded;
                currentMoveInput = HandleGroundedInput();
                if (currentMoveInput != Vector2.zero)
                {
                    finalizedState = EntityState.grounded_moving;
                }
            }
            else
            {
                finalizedState = EntityState.airborn;
                currentMoveInput = HandleAirInput();
            }

            setState(finalizedState);

            currentActionInput = HandleActionInput();



            SetInput(currentMoveInput);
            SetActions(currentActionInput);
            Move();
        }

        if(movementLocked && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            movementLocked = false;
            setState(EntityState.grounded);
        }
    }
    public void SetActions(bool[] actions)
    {
        actionInput = actions;
    }

    virtual public bool[] HandleActionInput()
    {
        bool [] result = new bool[entityActions.Length];
        for (int i = 0; i < entityActions.Length; i++)
        {
            bool keyPressed = false;
            KeyCode[] entityActionKeys = entityActions[i].Keys;
            int keyCount = entityActionKeys.Length;
            bool[] keysDown = new bool[keyCount];
            for (int j = 0; j < keyCount; j++)
            {
                keysDown[j] = Input.GetKey(entityActionKeys[j]);
                if (keysDown[j])
                {
                    keyPressed = true;
                }
            }
            entityActions[i].SetKeysDown(keysDown);
            result[i] = keyPressed;
        }
        return result;
    }
    public bool hasAnimator { get { return animator != null; }}
}


