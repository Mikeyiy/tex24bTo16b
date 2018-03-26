using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureEncoder
{
    private static void EncodeRG88ToRGBA4444(Color rg88, out Color rgba4444)
    {
        int X = (int)(rg88.r * 255);
        int Y = (int)(rg88.g * 255);

        int u = (X >> 4) & 0x000f;
        int b = X & 0x000f;
        float a1 = (float)u / 0x000f;
        float a2 = (float)b / 0x000f;

        u = (Y >> 4) & 0x000f;
        b = Y & 0x000f;
        float b1 = (float)u / 0x000f;
        float b2 = (float)b / 0x000f;

        rgba4444 = new Color(a1, a2, b1, b2);
    }


    [MenuItem("Assets/Texture24To16")]
    public static void EncodeRG88ToRGBA4444Test()
    {
        int texw = 256;
        int texh = 256;
        var pixels24 = new Color[texw * texh];
        var pixels16 = new Color[texw * texh];

        bool flag = true;

        for (var y = 0; y < texh; y++)
        {
            for (var x = 0; x < texw; x++)
            {
                Vector2 random = 255.0f * Random.insideUnitCircle;

                int X = Mathf.Abs(Mathf.RoundToInt(random.x));
                int Y = Mathf.Abs(Mathf.RoundToInt(random.y));

                float a = (float)X / 255.0f;
                float b = (float)Y / 255.0f;

                Color rg88 = new Color(a, b, 0, 1.0f);
                pixels24[y * texw + x] = rg88;

                Color rgba4444 = new Color();
                EncodeRG88ToRGBA4444(rg88, out rgba4444);

                // Validate in script
                float X1_f = (16 * rgba4444.r + rgba4444.g) * 15 / 255;
                float Y1_f = (16 * rgba4444.b + rgba4444.a) * 15 / 255;

                byte X1 = (byte)(X1_f * 255.0f);
                byte Y1 = (byte)(Y1_f * 255.0f);

                if (X != X1 || Y != Y1)
                {
                    flag = false;
                }

                pixels16[y * texw + x] = rgba4444;
            }
        }

        Debug.Log(flag);

        Texture2D texture24 = new Texture2D(texw, texh);
        texture24.SetPixels(pixels24);
        texture24.Apply();

        Texture2D texture16 = new Texture2D(texw, texh);
        texture16.SetPixels(pixels16);
        texture16.Apply();

        byte[] bytes = texture24.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/texture24bit.png", bytes);

        bytes = texture16.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/texture16bit.png", bytes);

        AssetDatabase.Refresh();


    }

    [MenuItem("Assets/SetTextureImporterSettings")]
    public static void SetTextureImporterSettings()
    {
        string path = "Assets/texture24bit.png";
        TextureImporter textureImporter = TextureImporter.GetAtPath(path) as TextureImporter;
        textureImporter.isReadable = true;
        textureImporter.sRGBTexture = false;
        textureImporter.mipmapEnabled = false;
        textureImporter.wrapMode = TextureWrapMode.Clamp;
        TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings();
        settings.format = TextureImporterFormat.RGB24;
        textureImporter.SetPlatformTextureSettings(settings);
        TextureImporterSettings importerSettings = new TextureImporterSettings();
        importerSettings.mipmapEnabled = false;
        textureImporter.SetTextureSettings(importerSettings);
        textureImporter.SaveAndReimport(); //  Could not create texture from Assets/texture24bit.png: Texture could not be created.
                                           //Assertion failed: GetObject failed to find Object for Library Representation
                                           //UnityEditor.AssetImporter:SaveAndReimport()
                                           //TextureEncoder: SetTextureImporterSettings()(at Assets / Editor / TextureEncoder.cs:107)

        path = path = "Assets/texture16bit.png";
        textureImporter = TextureImporter.GetAtPath(path) as TextureImporter;
        textureImporter.isReadable = true;
        textureImporter.sRGBTexture = false;
        textureImporter.mipmapEnabled = false;
        textureImporter.wrapMode = TextureWrapMode.Clamp;
        settings = new TextureImporterPlatformSettings();
        settings.format = TextureImporterFormat.RGBA16;
        textureImporter.SetPlatformTextureSettings(settings);
        importerSettings = new TextureImporterSettings();
        importerSettings.mipmapEnabled = false;
        textureImporter.SetTextureSettings(importerSettings);
        textureImporter.SaveAndReimport();  // Could not create texture from Assets/texture24bit.png: Texture could not be created.
    }
}
