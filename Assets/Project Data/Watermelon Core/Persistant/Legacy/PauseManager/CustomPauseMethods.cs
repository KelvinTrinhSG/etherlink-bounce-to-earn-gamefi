using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public partial class PauseManager
{
    public void CustomPauseMethods()
    {
        Tween.PauseAll();

        //Set audio effects
    }

    public void CustomResumeMethods()
    {
        Tween.ResumeAll();

        //Remove audio effects
    }
}
