using UnityEngine;
using System.Collections.Generic;
public class PiaAnimator : BaseCharacterAnimatorHandler
{
    protected override void InitParams()
    {
        paramMap = new Dictionary<string, string>
        {
            { "jalan", "run" },
            { "masak", "masakRendang" },
            { "ambilBumbu", "ambilBumbu" },
            { "gelarTikar", "gelarTikar" },
            { "bawaBarang", "bawaBarang" },
            { "duduk", "duduk" },
            { "makan", "makan" },
            { "doa", "doa" },
            { "jualBeli", "doa" }
        };

        allParams = new string[paramMap.Values.Count];
        paramMap.Values.CopyTo(allParams, 0);
    }
}