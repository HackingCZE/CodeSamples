using UnityEngine;

namespace DH.Core.Helpers.Extensions
{
    public static class AnimatorExtension
    {
        public static void UnpauseAnimationState(this Animator animator)
        {
            animator.speed = 1;
        }

        public static void ChangeAnimationState(this Animator animator, string newState)
        {
            string dummy;
            animator.ChangeAnimationState(newState, out dummy);
        }

        public static void ChangeAnimationState(this Animator animator, string newState, out string currentAnimationState)
        {
            animator.UnpauseAnimationState();
            currentAnimationState = newState;
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(newState)) return;

            animator.Play(newState);
        }

        public static void ResetAnimationState(this Animator animator)
        {
            animator.PauseAnimationState();
            animator.Play(animator.GetCurrentAnimatorStateInfo(0).shortNameHash, -1, normalizedTime: 0.0f);
            animator.UnpauseAnimationState();
        }

        public static void PauseAnimationState(this Animator animator)
        {
            animator.speed = 0;
        }

        public static bool IsAnimationFinished(this Animator animator, string animationName)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            return stateInfo.IsName(animationName) && stateInfo.normalizedTime >= 1f;
        }
    }

}
