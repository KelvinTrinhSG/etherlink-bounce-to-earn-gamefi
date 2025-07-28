#pragma warning disable 0649

using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CloseEventHandler : MonoBehaviour
{
    [SerializeField]
    private Page page;

    public void OnOpened()
    {
        PageController.OnPageOpened(page.PageType);
        
        page.OnOpened();
    }

    public void OnClosed()
    {
        gameObject.SetActive(false);

        PageController.OnMainPageClosedEvent(page.PageType);
    }
}
