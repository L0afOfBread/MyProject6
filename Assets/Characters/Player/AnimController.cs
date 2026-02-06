using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    public Animator playerAnimator;
    public bool canAttack = true;
    void Update()
    {
        AnimatorClipInfo[] clipInfo = playerAnimator.GetCurrentAnimatorClipInfo(0);
        Debug.Log(clipInfo[0].clip.name + "  canAttack = "+ canAttack);
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            playerAnimator.SetBool("Walk", true);
        }
        else if (Input.GetKeyDown(KeyCode.F) && canAttack == true)
        {
            playerAnimator.SetTrigger("Attack");
            canAttack = false;
        }
        else
        {
            playerAnimator.SetBool("Walk", false);
        }
        if (clipInfo[0].clip.name == "Punching")
        {
            canAttack = false;
        }
        else
        {
            canAttack = true;
        }
    }
}
