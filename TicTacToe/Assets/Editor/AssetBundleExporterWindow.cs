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
            var xAssetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(_xIcon));
            xAssetImporter.assetBundleName = _assetBundleName;
            xAssetImporter.name = "xIcon";

            var oAssetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(_oIcon));
            oAssetImporter.assetBundleName = _assetBundleName;
            oAssetImporter.name = "oIcon";

            var backgroundAssetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(_background));
            backgroundAssetImporter.assetBundleName = _assetBundleName;
            backgroundAssetImporter.name = "background";

            var lineAssetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(_line));
            lineAssetImporter.assetBundleName = _assetBundleName;
            lineAssetImporter.name = "line";

            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }

            BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, _buildOptions, _buildTarget);
        }
    }
}
