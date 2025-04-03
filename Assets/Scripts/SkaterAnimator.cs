using System.Collections;
using UnityEngine;

public class SkaterAnimator : MonoBehaviour
{
    public Animator animator;
    private bool isSkating = false;
    private float kickTimer = 0f;
    private float nextKickTime = 0f;

    void Start()
    {
        SetNextKickTime();
    }

    private void Update()
    {
        if (isSkating)
        {
            kickTimer += Time.deltaTime;

            // Eğer belirlenen süre dolduysa skate_kick animasyonuna geç
            if (kickTimer >= nextKickTime)
            {
                kickTimer = 0f;  // Sayaç sıfırlanır
                SetNextKickTime();
                Debug.Log("SkaterKick");
                animator.SetTrigger("SkaterKick");
            }

        }
    }
    void SetNextKickTime()
    {
        nextKickTime = Random.Range(2f, 4f); // Rastgele bir süre belirle
    }


    public void EnterSkate()
    {
        animator.SetTrigger("EnterSkate");
        isSkating = true;
        Debug.Log("enter skate");

    }
    public void ExitSkate()
    {
        animator.SetTrigger("ExitSkate");
        isSkating = false;
        kickTimer = 0;
        SetNextKickTime();
        Debug.Log("exitskate");

    }

}
