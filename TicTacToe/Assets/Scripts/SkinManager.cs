using Sirenix.OdinInspector;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
public class SkinManager : SerializedMonoBehaviour
{
    public static readonly string X_ICON_NAME = "xIcon";
    public static readonly string O_ICON_NAME = "oIcon";
    public static readonly string BACKGROUND_NAME = "background";
    public static readonly string LINE_NAME = "line";

    [SerializeField]
    private GameManager _gameManager;

    [SerializeField]
    private Skin _defaultSkin;

    private Skin _currentSkin;

    public void Init()
    {
        _currentSkin = _defaultSkin;
    }

    public async Task LoadSkin(string skinName)
    {
        string skinPath = Path.Combine(Application.streamingAssetsPath, skinName);
        if (File.Exists(skinPath))
        {
            var assetBundleRequest = AssetBundle.LoadFromFileAsync(skinPath);

            while (!assetBundleRequest.isDone)
            {
                await Task.Yield();
            }

            Skin skin = new Skin();

            skin.XIcon = await LoadSkinElement(assetBundleRequest.assetBundle, X_ICON_NAME);
            skin.OIcon = await LoadSkinElement(assetBundleRequest.assetBundle, O_ICON_NAME);
            skin.Background = await LoadSkinElement(assetBundleRequest.assetBundle, BACKGROUND_NAME);
            skin.Line = await LoadSkinElement(assetBundleRequest.assetBundle, LINE_NAME);

            if (skin.IsValid())
            {
                _currentSkin = skin;
            }
            else
            {
                Debug.LogError("Skin is not valid");
            }
        }
        else
        {
            Debug.LogError("Skin is does not exist");

        }
    }

    private async Task<Sprite> LoadSkinElement(AssetBundle bundle, string elementName)
    {
        if (bundle.Contains(elementName))
        {
            var request = bundle.LoadAssetAsync(elementName);

            while (!request.isDone)
            {
                await Task.Yield();
            }

            return request.asset as Sprite;
        }

        return null;
    }

    public void ApplyCurrentSkin()
    {
        _gameManager.ApplySkin(_currentSkin);
    }
}