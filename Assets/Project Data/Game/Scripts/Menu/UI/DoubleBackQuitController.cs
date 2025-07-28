#pragma warning disable 0649

using System.Collections;
using UnityEngine;
using Watermelon;

public class DoubleBackQuitController : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup quitMessageObject;

    [Space]
    [SerializeField]
    private PageController pageController;
    
    private bool clickedBefore = false;
    private TweenCase quitTween;

    private void Update()
    {
        if(pageController.enabled)
        {
            if (pageController.CurrentPage != pageController.DefaultPage)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    pageController.BackButton();
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Escape) && !clickedBefore)
                {
                    if (quitTween != null && !quitTween.isCompleted)
                        quitTween.Kill();

                    //Set to false so that this input is not checked again. It will be checked in the coroutine function instead
                    clickedBefore = true;

                    //Activate Quit Object
                    quitMessageObject.alpha = 0;
                    quitMessageObject.gameObject.SetActive(true);
                    quitTween = quitMessageObject.DOFade(1, 0.5f).SetEasing(Ease.Type.CircIn);

                    //Start quit timer
                    StartCoroutine(QuitingTimer());
                }
            }
        }
    }

    IEnumerator QuitingTimer()
    {
        //Wait for a frame so that Input.GetKeyDown is no longer true
        yield return null;

        //3 seconds timer
        const float timerTime = 2.5f;
        float counter = 0;

        while (counter < timerTime)
        {
            //Increment counter while it is < timer time(3)
            counter += Time.deltaTime;

            //Check if Input is pressed again while timer is running then quit/exit if is
            if (Input.GetKeyDown(KeyCode.Escape))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }

            //Wait for a frame so that Unity does not freeze
            yield return null;
        }

        quitTween = quitMessageObject.DOFade(0, 0.5f).SetEasing(Ease.Type.CircOut).OnComplete(delegate
        {
            quitMessageObject.gameObject.SetActive(false);
        });

        //Reset clickedBefore so that Input can be checked again in the Update function
        clickedBefore = false;
    }
}
