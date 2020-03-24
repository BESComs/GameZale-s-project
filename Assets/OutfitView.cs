using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OutfitView : MonoBehaviour
{
    public static OutfitView lastClickedView;
    
    public Image outfitIcon;
    public TextMeshProUGUI priceLabel;

    public OutfitPreset outfitPreset;

    public bool purchased;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(ClickListener);
    }

    public void Init(OutfitPreset outfitPreset)
    {
        this.outfitPreset = outfitPreset;
        outfitIcon.sprite = outfitPreset.icon;
        priceLabel.text = outfitPreset.price.ToString();
        purchased = ProfileController.Instance.purchasedItems.Exists(op => op.id == outfitPreset.id);
        if (purchased) priceLabel.gameObject.SetActive(false);
    }
    
    public void ClickListener()
    {
        if (purchased) 
        {
            PreviewOutfit();
            return;
        }
        
        if (lastClickedView == this)
        {
            SetPriceLabel();
            RequestPurchasing();
            lastClickedView = null;
        }
        else
        {
            if (lastClickedView != null) lastClickedView.SetPriceLabel();
            PreviewOutfit();
            SetPurchaseLabel();
            lastClickedView = this;
        }
        
    }

    public async void RequestPurchasing()
    {
        purchased = await ServerRequests.PurchaseItem(outfitPreset.id);
        if (purchased) priceLabel.gameObject.SetActive(false);
    }

    public void PreviewOutfit()
    {
        OutfitContent.Instance.PreviewOutfit(outfitPreset);
    }

    public void SetPriceLabel()
    {
        priceLabel.text = outfitPreset.price.ToString();
    }

    public void SetPurchaseLabel()
    {
        const string purchaseText = "Купить";
        priceLabel.text = purchaseText;
    }

}
