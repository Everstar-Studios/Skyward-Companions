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
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";
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