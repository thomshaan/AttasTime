using UnityEngine;

public class SyekhAnimator : BaseCharacterAnimatorHandler
{
    protected override void InitParams()
    {
        allParams = new string[] {
            "doa", "duduk", "diam", "ngaji", "mengajar"
        };
    }
}