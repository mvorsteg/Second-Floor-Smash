﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirPunchDown : StateMachineBehaviour
{

    CharacterController charCtrl;
    public float damageAmount = 3;
    public float launchFactor = 0.1f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        charCtrl = animator.gameObject.GetComponent<CharacterController>();
        charCtrl.Attack(0.45f, 0.65f, -1.9f, damageAmount, -1.75f, launchFactor);
        charCtrl.Attack(0.45f, 0.65f, -1.4f, damageAmount, -1.4f, launchFactor);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        charCtrl.Attack(0.45f, 0.65f, -1.9f, damageAmount, -1.75f, launchFactor);
        charCtrl.Attack(0.45f, 0.65f, -1.4f, damageAmount, -1.4f, launchFactor);
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
