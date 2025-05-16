using UnityEngine;
using GoogleMobileAds.Api;


public class Banner : MonoBehaviour
{
    private BannerView bannerView;

    void Start()
    {
        MobileAds.Initialize(initStatus => { });
        RequestBanner();
    }

    private void RequestBanner()
    {
        string adUnitId = "ca-app-pub-7734548175252541/1047528756";
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
        AdRequest request = new AdRequest();  
        bannerView.LoadAd(request);
    }

    private void OnDestroy()
    {
         if (bannerView != null)
        {
            bannerView.Destroy();
        }
    }
}