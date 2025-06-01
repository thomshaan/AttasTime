using UnityEngine;

public class IbuAnimator : BaseCharacterAnimatorHandler
{
    protected override void InitParams()
    {
        allParams = new string[] {
            "run", "ambilBumbu", "jualBeli", "masakRendang",
            "bawaBarang", "duduk", "makan"
        };
    }
}