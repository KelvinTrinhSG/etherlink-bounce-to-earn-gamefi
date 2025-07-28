using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlatfromBehaviour : Platform
{
    public Animator animator;

    private List<JumpPlatfromBehaviour> platformPartsList = new List<JumpPlatfromBehaviour>();
    private int jumpTrigger;

    protected override void Awake()
    {
        base.Awake();
        jumpTrigger = Animator.StringToHash("jump");
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        platformPartsList.Clear();
        //platformPartsList.Add(this);
    }

    public override void AddPlatformPart(GameObject platformPart, bool isSpikedPlatformSpawnedNow)
    {
        base.AddPlatformPart(platformPart, isSpikedPlatformSpawnedNow);

        platformPartsList.Add(platformPart.GetComponent<JumpPlatfromBehaviour>());
    }

    public void RunAnimation()
    {
        animator.SetTrigger(jumpTrigger);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            for (int i = 0; i < platformPartsList.Count; i++)
            {
                platformPartsList[i].RunAnimation();
               
                if(platformPartsList[i].isComboPlatform)
                {
                    platformPartsList[i].DisableComboLook(true);
                }
            }
        }
    }
}