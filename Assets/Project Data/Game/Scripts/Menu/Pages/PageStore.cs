#pragma warning disable 0649

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Watermelon;

public sealed class PageStore : Page
{
    [SerializeField]
    private GameObject skinUIPrefab;
    [SerializeField]
    private Transform skinUIContainer;

    [Space]
    [SerializeField]
    private GameObject skinDescriptionPanel;
    [SerializeField]
    private Text skinDescription;

    [SerializeField]
    private GameObject skinAdsPanel;
    [SerializeField]
    private UXButton skinAdsButton;
    [SerializeField]
    private Text skinAds;

    [SerializeField]
    private GameObject skinSharePanel;
    [SerializeField]
    private UXButton skinShareButton;

    [Space]
    [SerializeField]
    private Color selectedSkinBackground;
    [SerializeField]
    private Color defaultSkinBackground;

    [Space]
    [SerializeField]
    private Color progressSelectedColor;
    [SerializeField]
    private Color progressDefaultColor;

    [Space]
    [SerializeField]
    private Color lockedSkinColor;

    [Space]
    [SerializeField]
    private Image storeButtonSkinImage;

    [Space]
    [SerializeField]
    private Text storeButtonText;

    [Space]
    [SerializeField]
    private ScrollRect storeScrollRect;
    
    private UIStoreSkin selectedStoreSkin;
    
    private int enabledSkinID = -1;

    private List<UIStoreSkin> storeElements = new List<UIStoreSkin>();
    private Dictionary<int, int> storeElementsLink = new Dictionary<int, int>();

    private const string LAST_PRODUCT_ID_PREFS = "LAST_SKIN_ID";
    private const string UNLOCKED_SKIN = "Skin unlocked";
    private const string COMPLETED_MESSAGE = " (Completed)";

    private readonly Vector2 unselectedSize = new Vector2(-25, -25);
    private readonly Vector2 selectedSize = new Vector2(5, 5);

    private void Start()
    {
        StoreController.OpenProduct(0);

        StoreProduct[] storeProducts = StoreController.GetActiveProducts().OrderByDescending(x => x.Progress()).ToArray();
        for (int i = 0; i < storeProducts.Length; i++)
        {
            GameObject storeProductGO = Instantiate(skinUIPrefab);
            storeProductGO.transform.SetParent(skinUIContainer);
            storeProductGO.transform.localScale = Vector3.one;
            storeProductGO.transform.rotation = Quaternion.identity;
            storeProductGO.SetActive(true);

            UIStoreSkin storeUIObject = storeProductGO.GetComponent<UIStoreSkin>();

            StoreProduct storeSkinProduct = storeProducts[i];
            storeUIObject.SetSize(unselectedSize);
            storeUIObject.SetBackgroundColor(defaultSkinBackground);
            storeUIObject.Init(storeSkinProduct, OnSkinObjectClicked);
            
            storeUIObject.SetProgress(storeSkinProduct.Progress());

            if(!storeSkinProduct.IsOpened())
                storeUIObject.SetSkinColor(lockedSkinColor);

            storeElements.Add(storeUIObject);
            storeElementsLink.Add(storeSkinProduct.ID, i);
        }
        
        SelectSkin(PlayerPrefs.GetInt(LAST_PRODUCT_ID_PREFS, 0));
    }

    private void SelectSkin(int id)
    {
        if (storeElementsLink.ContainsKey(id))
        {
            SelectSkin(storeElements[storeElementsLink[id]]);
        }
    }

    private void SelectSkin(UIStoreSkin uIStoreSkin)
    {
        StoreProduct storeSkinProduct = uIStoreSkin.StoreSkinProduct;
        bool isOpened = uIStoreSkin.StoreSkinProduct.IsOpened();

        //Unselect
        if (selectedStoreSkin != null && selectedStoreSkin != uIStoreSkin)
        {
            selectedStoreSkin.SetSize(unselectedSize);
            selectedStoreSkin.SetBackgroundColor(defaultSkinBackground);
        }

        uIStoreSkin.SetSize(selectedSize);
        uIStoreSkin.SetBackgroundColor(selectedSkinBackground);

        if (isOpened)
        {
            if (enabledSkinID != uIStoreSkin.ID)
            {
                //Unselect prev skin
                if (enabledSkinID != -1)
                    storeElements[storeElementsLink[enabledSkinID]].SetProgressColor(progressDefaultColor);
                
                //Change skin
                PlayerController.SelectSkin(storeSkinProduct);
            }

            uIStoreSkin.SetProgressColor(progressSelectedColor);

            enabledSkinID = uIStoreSkin.ID;

            PlayerPrefs.SetInt(LAST_PRODUCT_ID_PREFS, enabledSkinID);

            storeButtonSkinImage.sprite = uIStoreSkin.StoreSkinProduct.icon;

            uIStoreSkin.SetSkinColor(Color.white);
        }

        if(storeSkinProduct.ProductBehaviourType == BehaviourType.Default)
        {
            ShowDescriptionPanel("Hi, How Are You?");
        }
        else if(storeSkinProduct.ProductBehaviourType == BehaviourType.Achievement)
        {
            StoreSkinProduct storeSkinAchievementProduct = (StoreSkinProduct)storeSkinProduct;

            if (isOpened)
            {
                ShowDescriptionPanel(Multilanguage.GetWord(storeSkinAchievementProduct.Achievement.Description) + COMPLETED_MESSAGE);
            }
            else
            {
                ShowDescriptionPanel(Multilanguage.GetWord(storeSkinAchievementProduct.Achievement.Description));
            }
        }
        else if(storeSkinProduct.ProductBehaviourType == BehaviourType.Ads)
        {
            //Disable other panels
            skinDescriptionPanel.SetActive(false);
            skinSharePanel.SetActive(false);
            
            StoreSkinAdsProduct storeSkinAdsProduct = (StoreSkinAdsProduct)storeSkinProduct;
            int adsViews = storeSkinAdsProduct.GetCurrentViews();

            if(adsViews > 0)
            {
                skinAdsPanel.SetActive(true);

                skinAds.text = adsViews.ToString();
                int storeElementIndex;

                skinAdsButton.onClick.RemoveAllListeners();
                skinAdsButton.onClick.AddListener(delegate
                {
                    storeSkinAdsProduct.ShowAds((currentViews) =>
                    {
                        if (currentViews == 0)
                        {
                            storeScrollRect.verticalNormalizedPosition = 1;

                            uIStoreSkin.transform.SetAsFirstSibling();

                            if (storeElementsLink.TryGetValue(storeSkinAdsProduct.ID, out storeElementIndex))
                            {
                                storeElements[storeElementIndex].SetProgress(storeSkinAdsProduct.Progress());
                            }

                            StoreController.BuyProduct(storeSkinAdsProduct);

                            SelectSkin(uIStoreSkin);
                        }
                        else
                        {
                            skinAds.text = currentViews.ToString();

                            
                            if (storeElementsLink.TryGetValue(storeSkinAdsProduct.ID, out storeElementIndex))
                            {
                                storeElements[storeElementIndex].SetProgress(storeSkinAdsProduct.Progress());
                            }
                        }
                    });
                });
            }
            else
            {
                ShowDescriptionPanel(UNLOCKED_SKIN);
            }
        }
        else if (storeSkinProduct.ProductBehaviourType == BehaviourType.Share)
        {
            //Disable other panels
            skinDescriptionPanel.SetActive(false);
            skinAdsPanel.SetActive(false);

            if(isOpened)
            {
                ShowDescriptionPanel(UNLOCKED_SKIN);
            }
            else
            {
                StoreSkinShareProduct storeSkinShareProduct = (StoreSkinShareProduct)storeSkinProduct;

                skinSharePanel.SetActive(true);

                skinShareButton.onClick.RemoveAllListeners();
                skinShareButton.onClick.AddListener(delegate
                {
                    storeScrollRect.verticalNormalizedPosition = 1;

                    uIStoreSkin.transform.SetAsFirstSibling();

                    storeSkinShareProduct.Share();

                    StoreController.BuyProduct(storeSkinShareProduct);

                    SelectSkin(uIStoreSkin);
                });
            }
        }

        selectedStoreSkin = uIStoreSkin;

        storeButtonText.text = StoreController.BoughtProductsAmount() + "/" + StoreController.GetProductsAmount();
    }
    
    private void ShowDescriptionPanel(string text)
    {
        //Disable other panels
        skinAdsPanel.SetActive(false);
        skinSharePanel.SetActive(false);

        skinDescriptionPanel.SetActive(true);
        skinDescription.text = text;
    }

    public void OnSkinObjectClicked(UIStoreSkin storeUIObject)
    {
        if(selectedStoreSkin != storeUIObject)
        {
            SelectSkin(storeUIObject);
        }
    }

    public override void Close(bool immediately)
    {
        CameraController.OnMenuOpened();

        if (AdsManager.IsForcedAdEnabled())
            AdsManager.ShowBanner();
    }

    public override void Open()
    {
        CameraController.OnStoreOpened();

        if (AdsManager.IsForcedAdEnabled())
            AdsManager.HideBanner();

        SelectSkin(PlayerPrefs.GetInt(LAST_PRODUCT_ID_PREFS, 0));

        storeScrollRect.verticalNormalizedPosition = 1;
    }

    public override void OnClosed()
    {
    }

    public override void OnOpened()
    {

    }

    public void BackToMainButton()
    {
        PageController.Open(PageType.Main);
    }
}
