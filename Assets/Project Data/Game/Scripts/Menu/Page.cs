#pragma warning disable 0649

using UnityEngine;

public abstract class Page : MonoBehaviour
{
    [SerializeField]
    private PageType pageType;
    public PageType PageType
    {
        get { return pageType; }
    }

    public virtual void Init() { }

    public abstract void Open();
    public abstract void Close(bool immediately);

    public virtual void OnOpened() { }
    public virtual void OnClosed() { }
}
