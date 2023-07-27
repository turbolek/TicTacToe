using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AssetBundleExporterWindow : EditorWindow
{
    private string _assetBundleName = "SkinBundle";
    private Sprite _xIcon;
    private Sprite _oIcon;
    private Sprite _background;
    private Sprite _line;

    private BuildTarget _buildTarget = BuildTarget.StandaloneWindows64;
    private BuildAssetBundleOptions _buildOptions = BuildAssetBundleOptions.None;

    [MenuItem("Tools/Asset Bundle Exporter")]
    public static void ShowWindow()
    {
        EditorWindow wnd = GetWindow<AssetBundleExporterWindow>();
        wnd.titleContent = new GUIContent("Asset Bundle Exporter");
    }

    private void OnGUI()
    {
        GUILayout.Label("X Icon:");
        _xIcon = EditorGUILayout.ObjectField(_xIcon, typeof(Sprite), false) as Sprite;

        GUILayout.Label("O Icon:");
        _oIcon = EditorGUILayout.ObjectField(_oIcon, typeof(Sprite), false) as Sprite;

        GUILayout.Label("Background:");
        _background = EditorGUILayout.ObjectField(_background, typeof(Sprite), false) as Sprite;

        GUILayout.Label("Line:");
        _line = EditorGUILayout.ObjectField(_line, typeof(Sprite), false) as Sprite;

        _assetBundleName = EditorGUILayout.TextField("Bundle Name", _assetBundleName);

        if (GUILayout.Button("Export"))
        {
            if (_xIcon == null || _oIcon == null || _background == null || _line == null)
            {
                Debug.LogError("Asset bundle export failed. Not all assets assigned.");
                return;
            }
            var xPath = AssetDatabase.GetAssetPath(_xIcon);
            var xAssetImporter = AssetImporter.GetAtPath(xPath);
            xAssetImporter.assetBundleName = _assetBundleName;
            xAssetImporter.name = "xIcon";
            AssetDatabase.RenameAsset(xPath, SkinManager.X_ICON_NAME);

            var oPath = AssetDatabase.GetAssetPath(_oIcon);
            var oAssetImporter = AssetImporter.GetAtPath(oPath);
            oAssetImporter.assetBundleName = _assetBundleName;
            AssetDatabase.RenameAsset(oPath, SkinManager.O_ICON_NAME);

            var backgroundPath = AssetDatabase.GetAssetPath(_background);
            var backgroundAssetImporter = AssetImporter.GetAtPath(backgroundPath);
            backgroundAssetImporter.assetBundleName = _assetBundleName;
            AssetDatabase.RenameAsset(backgroundPath, SkinManager.BACKGROUND_NAME);

            var linePath = AssetDatabase.GetAssetPath(_line);
            var lineAssetImporter = AssetImporter.GetAtPath(linePath);
            lineAssetImporter.assetBundleName = _assetBundleName;
            AssetDatabase.RenameAsset(linePath, SkinManager.LINE_NAME);

            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }

            BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, _buildOptions, _buildTarget);
        }
    }
}
