using UnityEngine;
using System.Collections.Generic;

public class AttaAnimator : BaseCharacterAnimatorHandler
{
    protected override void InitParams()
    {
        paramMap = new Dictionary<string, string>
        {
            { "jalan", "run" },
            { "ambilBumbu", "ambilBumbu" },
            { "bawaBarang", "bawaBarang" },
            { "jualBeli", "jualBeli" },
            { "gelarTikar", "gelarTikar" },
            { "lari", "lari" },
            { "masak", "masak" },
            { "tanamPadi", "tanamPadi" },
            { "duduk", "duduk" },
            { "doa", "doa" },
            { "ngaji", "ngaji" },
            { "makan", "makan" }
        };

        // Isi allParams agar bisa di-reset semuanya
        allParams = new List<string>(paramMap.Values).ToArray();
    }
}