using UnityEngine;

public class AttaAnimator : BaseCharacterAnimatorHandler
{
    protected override void InitParams()
    {
        allParams = new string[] {
            "lari", "duduk", "doa", "ngaji", "makan",
            "masak", "tanamPadi", "diam"
        };
    }
}