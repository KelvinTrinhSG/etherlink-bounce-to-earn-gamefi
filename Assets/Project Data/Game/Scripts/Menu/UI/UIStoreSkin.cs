#pragma warning disable 0649

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIStoreSkin : MonoBehaviour, IPointerClickHandler
{
    public int ID
    {
        get { return storeSkinProduct.ID; }
    }

    [SerializeField]
    private Image background;
    [SerializeField]
    private Image progress;
    [SerializeField]
    private Image skin;
        
    private SelectSkinDelegate onSkinClicked;

    private StoreProduct storeSkinProduct;
    public StoreProduct StoreSkinProduct
    {
        get { return storeSkinProduct; }
    }

    public void Init(StoreProduct storeSkinProduct, SelectSkinDelegate onSkinClicked)
    {
        this.storeSkinProduct = storeSkinProduct;
        this.onSkinClicked = onSkinClicked;

        skin.sprite = storeSkinProduct.icon;
    }

    public void SetSize(Vector2 size)
    {
        background.rectTransform.sizeDelta = size;
    }

    public void SetBackgroundColor(Color color)
    {
        background.color = color;
    }

    public void SetProgressColor(Color color)
    {
        progress.color = color;
    }

    public void SetProgress(float value)
    {
        progress.fillAmount = value;
    }

    public void SetSkinColor(Color color)
    {
        skin.color = color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onSkinClicked != null)
            onSkinClicked.Invoke(this);
    }

    public delegate void SelectSkinDelegate(UIStoreSkin skin);
}
