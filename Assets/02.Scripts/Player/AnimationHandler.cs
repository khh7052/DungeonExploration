using UnityEngine;
using Constants;

public class AnimationHandler : MonoBehaviour
{
    private PlayerController controller;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        controller = GetComponentInParent<PlayerController>();
    }

    public void Jump(bool isJump)
    {
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
    public void Climb(bool isClimbing)
    {
        anim.SetBool(AnimatorHash.ClimbHash, isClimbing);
    }

    public void OnClimbUpEnd()
    {
        controller.ClimbUpEnd();
    }



}
