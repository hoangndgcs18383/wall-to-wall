/*using UnityEngine;
using VoxelBusters.EssentialKit;

public class ShareSocialManager
{
    private static ShareSocialManager _instance;

    public static ShareSocialManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ShareSocialManager();
            }

            return _instance;
        }
    }

    public void ShareFacebook()
    {
        bool isFacebookAvailable = SocialShareComposer.IsComposerAvailable(SocialShareComposerType.Facebook);

        if (!isFacebookAvailable) return;
        SocialShareComposer composer = SocialShareComposer.CreateInstance(SocialShareComposerType.Facebook);
        composer.AddScreenshot();
        composer.SetCompletionCallback((result, error) =>
        {
            Debug.Log("Social Share Composer was closed. Result code: " + result.ResultCode);
        });
        composer.Show();
    }
}*/