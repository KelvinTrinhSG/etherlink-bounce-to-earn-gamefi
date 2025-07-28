using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CloseEventMainHandler : MonoBehaviour
{
    public void OnOpened(PageType pageType)
    {
        PageController.OnMainPageOpenedEvent(pageType);
    }

    public void OnClosed(PageType pageType)
    {
        PageController.OnMainPageClosedEvent(pageType);
    }
}
