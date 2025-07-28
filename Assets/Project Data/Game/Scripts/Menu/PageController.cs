#pragma warning disable 0649

using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class PageController : MonoBehaviour
{
    public static PageController instance;

    [SerializeField]
    private PageContainer[] pages;

    [SerializeField]
    private PageType defaultPage;
    public PageType DefaultPage
    {
        get { return defaultPage; }
    }

    [SerializeField]
    private Animator mainAnimator;
    
    private Dictionary<PageType, int> pagesLink = new Dictionary<PageType, int>();

    private int mainPageIndex = -1;
    private int windowPageIndex = -1;

    private PageContainer MainPage
    {
        get { return pages[mainPageIndex]; }
    }

    private PageContainer WindowPage
    {
        get { return pages[windowPageIndex]; }
    }

    private PageType currentPage;
    public PageType CurrentPage
    {
        get { return currentPage; }
    }

    private PageType prevPage;
    
    private bool isBusy;

    public OnPageLoaded onPageLoaded;
    public OnPageLoaded onPageClosed;

    private int parameterCloseHash;
    private int parameterOpenHash;
    private int parameterSelectedWindowHash;
    private int parameterPrevWindowHash;

    private const string SELECTED_WINDOW_ANIMATOR_PARAMETER = "SelectedWindow";
    private const string PREV_WIDNOW_ANIMATOR_PARAMETER = "PrevWindow";
    private const string CLOSE_ANIMATOR_TRIGGER = "Close";
    private const string OPEN_ANIMATOR_TRIGGER = "Open";

    private void Awake()
    {
        instance = this;

        currentPage = defaultPage;

        //Create pages array link
        for (int i = 0; i < pages.Length; i++)
        {
            if(!pagesLink.ContainsKey(pages[i].pageType))
                pagesLink.Add(pages[i].pageType, i);
        }

        mainPageIndex = pagesLink[defaultPage];

        //Inititalize animator hashes
        parameterOpenHash = Animator.StringToHash(OPEN_ANIMATOR_TRIGGER);
        parameterCloseHash = Animator.StringToHash(CLOSE_ANIMATOR_TRIGGER);
        parameterSelectedWindowHash = Animator.StringToHash(SELECTED_WINDOW_ANIMATOR_PARAMETER);
        parameterPrevWindowHash = Animator.StringToHash(PREV_WIDNOW_ANIMATOR_PARAMETER);
    }

    private void Start()
    {
        if(!GameController.showMenu)
        {
            DisableMenu(true);
        }
        else
        {
            Tween.NextFrame(delegate
            {
                mainAnimator.SetInteger(SELECTED_WINDOW_ANIMATOR_PARAMETER, (int)defaultPage);
            });
        }
    }

    public void BackButton()
    {
        PageContainer pageContainer = pages[pagesLink[currentPage]];
        if (pageContainer.backButton)
        {
            if(!pageContainer.isWindow)
            {
                OpenPage(defaultPage);
            }
            else
            {
                CloseCurrent();
            }
        }
    }

    public void DisableMenu(bool immediately = false)
    {
        //Close prev page
        int currentPageIndex = pagesLink[currentPage];
        pages[currentPageIndex].page.Close(immediately);

        if (!immediately)
        {
            mainAnimator.enabled = true;
            
            mainAnimator.SetTrigger(parameterCloseHash);
            mainAnimator.SetInteger(parameterSelectedWindowHash, -1);
            mainAnimator.SetInteger(parameterPrevWindowHash, mainPageIndex);
        }

        mainPageIndex = -1;
        currentPage = PageType.None;

        enabled = false;

        GameController.StartGame();

        AdsManager.ShowBanner();
    }

    public void CloseCurrent()
    {
        int currentPageIndex = pagesLink[currentPage];
        if (pages[currentPageIndex].isWindow)
        {
            pages[currentPageIndex].animator.enabled = true;

            pages[currentPageIndex].animator.SetTrigger(parameterCloseHash);

            pages[currentPageIndex].page.Close(false);

            PageType pageType = currentPage;

            currentPage = prevPage;
            prevPage = pageType;
        }
    }

    public void ClosePage(PageType pageType)
    {
        if (isBusy)
            return;

        if(pageType == currentPage)
        {
            CloseCurrent();
        }
    }
    
    public void OpenPage(PageType pageType)
    {
        if (isBusy)
            return;

        if (pageType == currentPage)
            return;

        isBusy = true;

        int pageIndex = pagesLink[pageType];
        if (pages[pageIndex].isWindow)
        {
            pages[pageIndex].animator.enabled = true;

            pages[pageIndex].animator.gameObject.SetActive(true);
            pages[pageIndex].animator.SetTrigger(parameterOpenHash);

            pages[pageIndex].page.Open();

            windowPageIndex = pageIndex;
        }
        else
        {
            mainAnimator.enabled = true;

            if(currentPage != PageType.None)
            {
                //Close prev page
                int currentPageIndex = pagesLink[currentPage];
                pages[currentPageIndex].page.Close(false);

                mainAnimator.SetTrigger(parameterCloseHash);
            }

            mainAnimator.SetInteger(parameterSelectedWindowHash, (int)pageType);
            mainAnimator.SetInteger(parameterPrevWindowHash, mainPageIndex);

            pages[pageIndex].page.Open();
            
            mainPageIndex = pageIndex;
        }
        
        prevPage = currentPage;
        currentPage = pageType;
    }

    public static void OnPageOpened(PageType pageType)
    {
        if (instance.onPageLoaded != null)
            instance.onPageLoaded.Invoke(instance.pages[instance.pagesLink[pageType]]);

        instance.pages[instance.pagesLink[pageType]].animator.enabled = false;

        instance.isBusy = false;
    }

    public static void OnMainPageOpenedEvent(PageType pageType)
    {
        instance.pages[instance.pagesLink[pageType]].page.OnOpened();

        if (instance.onPageLoaded != null)
            instance.onPageLoaded.Invoke(instance.pages[instance.pagesLink[pageType]]);

        instance.mainAnimator.enabled = false;

        instance.isBusy = false;
    }
    
    public static void OnMainPageClosedEvent(PageType pageType)
    {
        instance.pages[instance.pagesLink[pageType]].page.OnClosed();

        if (instance.onPageClosed != null)
            instance.onPageClosed.Invoke(instance.pages[instance.pagesLink[pageType]]);
    }

    public static void Open(PageType pageType)
    {
        instance.OpenPage(pageType);
    }

    public static void Close(PageType pageType)
    {
        instance.ClosePage(pageType);
    }

    [System.Serializable]
    public class PageContainer
    {
        public PageType pageType;
        public Page page;

        [Space]
        public bool backButton = true;

        [Space]
        public bool isWindow;
        public Animator animator;
    }

    public delegate void OnPageLoaded(PageContainer pageContainer);
}
