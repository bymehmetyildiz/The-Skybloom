using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    public Player player;

    private void Start()
    {
        player = GetComponentInParent<Player>();
    }

    public void TriggerLand()
    {
        player.landTrigger = true;
      
    }
   

}
