using System.Collections;
using UnityEngine;

public class BaseCharacterAnimatorHandler : MonoBehaviour
{
    public Animator animator;
    protected string[] allParams;

    private Coroutine currentRoutine;

    protected virtual void Awake()
    {
        InitParams();
    }

    protected virtual void InitParams()
    {
        // Default kosong, akan di-override oleh turunan
        allParams = new string[0];
    }

    public void PlayAnim(string paramName, float duration)
    {
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

    private void ResetAllBools()
    {
        foreach (var param in allParams)
        {
            animator.SetBool(param, false);
        }
    }
}
