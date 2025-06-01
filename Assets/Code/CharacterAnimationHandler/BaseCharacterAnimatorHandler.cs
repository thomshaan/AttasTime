using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacterAnimatorHandler : MonoBehaviour
{
    public Animator animator;

    protected string[] allParams;
    protected Dictionary<string, string> paramMap;

    private Coroutine currentRoutine;

    protected virtual void Awake()
    {
        InitParams();
    }


    protected virtual void InitParams()
    {
        allParams = new string[0];
        paramMap = new Dictionary<string, string>();
    }


    public void PlayAnim(string alias, float duration)
    {
        if (paramMap == null || !paramMap.ContainsKey(alias))
        {
            Debug.LogWarning($"[{name}] Tidak punya animasi untuk alias: {alias}");
            return; // Tidak melakukan apapun
        }

        string paramName = paramMap[alias];

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(PlayAnimRoutine(paramName, duration));
    }

    private IEnumerator PlayAnimRoutine(string paramName, float duration)
    {
        ResetAllBools();
        animator.SetBool(paramName, true);
        yield return new WaitForSeconds(duration);
        animator.SetBool(paramName, false);
    }

    /// <summary>
    /// Reset semua parameter animator yang ada di allParams.
    /// </summary>
    private void ResetAllBools()
    {
        if (allParams == null) return;

        foreach (var param in allParams)
        {
            animator.SetBool(param, false);
        }
    }
}
