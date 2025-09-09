using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    public static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "AssetBundles";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        try
        {
            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.StrictMode, BuildTarget.StandaloneWindows);
            DirectoryInfo d = new DirectoryInfo(assetBundleDirectory);
            File.Delete($"{assetBundleDirectory}/AssetBundles");
            foreach (var file in d.GetFiles("*.manifest"))
            {
                file.Delete();
            }
            foreach (var file in d.GetFiles("*.manifest.meta"))
            {
                file.Delete();
            }
            AssetDatabase.Refresh();
            Debug.Log("<b>✔️ SUCCESSFULLY BUILDED ASSETBUNDLES ✔️</b>");
        }
        catch (Exception e)
        {
            Debug.LogError("AN ERROR OCCURED WHILE BUILDING THE ASSETBUNDLES!\n" + e.ToString());
        }
    }
}
