using UnityEngine;

public class PiaAnimator : BaseCharacterAnimatorHandler
{
    protected override void InitParams()
    {
        allParams = new string[] {
            "jalan", "ngaji", "diam", "duduk", "lari", "tanganBelakang"
        };
    }
}