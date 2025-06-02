using UnityEngine;

public class PamanAnimator : BaseCharacterAnimatorHandler
{
    protected override void InitParams()
    {
        allParams = new string[] {
            "diam", "tanamPadi", "jalan", "lari", "doa"
        };
    }
}