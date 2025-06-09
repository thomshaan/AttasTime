using UnityEngine;
using System.Collections.Generic;
public class SyekhAnimator : BaseCharacterAnimatorHandler
{
    protected override void InitParams()
    {
        paramMap = new Dictionary<string, string>
        {
            { "jalan", "run" },
            { "doa", "doa" },
            { "duduk", "duduk" },
            { "makan", "makan" },
            { "ceramah", "ceramah" },
            { "jualBeli", "ceramah" }
        };

        allParams = new string[paramMap.Values.Count];
        paramMap.Values.CopyTo(allParams, 0);
    }
}