using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHitbox : StateMachineBehaviour
{
    private BoxCollider2D itemHitBox;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // When the item is done spawning, it will be active
        if(itemHitBox == null)
        {
            itemHitBox = animator.gameObject.GetComponentInParent<BoxCollider2D>();
        }
        itemHitBox.enabled = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateinfo, int layerindex)
    {
        // Once the bool"canDespawn" is active, this deactivates the item
        itemHitBox.enabled = false;
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
