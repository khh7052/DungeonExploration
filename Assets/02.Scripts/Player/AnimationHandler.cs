using UnityEngine;
using Constants;

public class AnimationHandler : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void Jump(bool isJump)
    {
        Debug.Log("Jump");
        anim.SetBool(AnimatorHash.JumpHash, isJump);
    }

    public void MoveSpeed(float speed)
    {
        anim.SetFloat(AnimatorHash.MoveSpeedHash, speed);
    }

    public void Dash(bool isDashing)
    {
        anim.SetBool(AnimatorHash.DashHash, isDashing);
    }

}
