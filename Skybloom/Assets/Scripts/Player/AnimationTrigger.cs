using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    public Player player;
    public Animator animator;
    

    private void Start()
    {
        player = GetComponentInParent<Player>();

    }

    public void TriggerLand()
    {
        player.landTrigger = true;
      
    }

    public void TriggerAttack()
    {
        player.AnimationTrigger();
        animator.SetBool("Attack", false);
    }

    public void EffectAnimTrigger()
    {
        animator.SetBool("Attack", true);
        animator.SetInteger("ComboCounter", player.comboCounter);
    }



}
