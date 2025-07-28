using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePointer : MonoBehaviour
{

    public Animator animator;
    public TextMesh scoreText;

    private int animationHash;

    public void Initialize(string textToDisplay, float xPosition)
    {
        transform.position = new Vector3(xPosition, 0f);
        scoreText.text = textToDisplay;

        animationHash = Animator.StringToHash("OnPassed");
    }

    public void RunPassedAnimation()
    {
        animator.SetTrigger(animationHash);
    }
}