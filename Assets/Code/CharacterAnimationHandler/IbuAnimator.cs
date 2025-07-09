using UnityEngine;
using System.Collections.Generic;
public class IbuAnimator : BaseCharacterAnimatorHandler
{
    protected override void InitParams()
    {
        allParams = new string[] {
            "run", "ambilBumbu", "jualBeli", "masakRendang",
            "bawaBarang", "duduk", "makan"
        };

        paramMap = new Dictionary<string, string>
        {
            { "jalan", "run" },
            { "ambilBumbu", "ambilBumbu" },
            { "jualBeli", "jualBeli" },
            { "masak", "masakRendang" },
            { "bawaBarang", "bawaBarang" },
            { "duduk", "duduk" },
            { "makan", "makan" }
        };
    }
}