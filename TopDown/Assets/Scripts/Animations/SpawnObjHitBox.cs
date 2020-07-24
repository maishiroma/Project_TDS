using Matt_Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjHitBox : StateMachineBehaviour
{
    [Header("General Vars")]
    public bool isConsumable;               // Is this spawned object an item?
    
    // Private vars
    private BoxCollider2D spawnObjHitbox;   // Reference to the spawned obj's hitbox

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Caches the hitbox for later
        if (spawnObjHitbox == null)
        {
            spawnObjHitbox = animator.gameObject.GetComponentInParent<BoxCollider2D>();
        }

        if (isConsumable == true)
        {
            // If we're a consumable, we presume that it has finished spawning and it will be active
            spawnObjHitbox.enabled = true;
        }
        else
        {
            // If we are NOT a consumable, we set the is_spawning bool true
            animator.SetBool("is_spawning", true);
            spawnObjHitbox.enabled = false;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateinfo, int layerindex)
    {
        if(isConsumable == true)
        {
            // This presumes we are despawning an item, so we turn off the hitbox
            spawnObjHitbox.enabled = false;
        }
        else
        {
            // This presumes we are an entity, in which case, we tick off the is_spawning animation bool
            animator.SetBool("is_spawning", false);

            // And activate its hitbox
            spawnObjHitbox.enabled = true;

            // As well as entity specific components
            if(animator.gameObject.GetComponentInParent<EnemyMovement>() != null)
            {
                // If this entity is an enemy, we enable its range to be active
                animator.gameObject.GetComponentInParent<EnemyMovement>().enemyRange.SetActive(true);
            }
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
