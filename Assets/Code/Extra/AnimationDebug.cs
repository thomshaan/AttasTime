using UnityEngine;

public class NPCAnimationTester : MonoBehaviour
{
    [SerializeField] private Animator animator;

    void Update()
    {
        // Idle by default
        if (Input.GetKeyDown(KeyCode.Alpha0))
            ResetAll();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ResetAll();
            animator.SetBool("run", true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ResetAll();
            animator.SetBool("ceramah", true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ResetAll();
            animator.SetBool("duduk", true);
            animator.SetBool("doa", true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ResetAll();
            animator.SetBool("duduk", true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ResetAll();
            animator.SetBool("duduk", true);
            animator.SetBool("makan", true);
        }
    }

    void ResetAll()
    {
        animator.SetBool("run", false);
        animator.SetBool("ceramah", false);
        animator.SetBool("doa", false);
        animator.SetBool("duduk", false);
        animator.SetBool("makan", false);
    }
}
