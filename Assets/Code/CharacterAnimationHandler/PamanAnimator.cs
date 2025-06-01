using UnityEngine;
using System.Collections.Generic;

public class PamanAnimator : BaseCharacterAnimatorHandler
{
    protected override void InitParams()
    {
        paramMap = new Dictionary<string, string>
        {
            { "jalan", "run" },
            { "duduk", "duduk" },
            { "makan", "makan" },
            { "bawaBarang", "bawaBarang" },
            { "jualBeli", "jualBeli" },
            { "tanamPadi", "tanamPadi" }
        };

        allParams = new string[paramMap.Values.Count];
        paramMap.Values.CopyTo(allParams, 0);
    }
}
