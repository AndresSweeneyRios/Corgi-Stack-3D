// ------------------------------------------------------------------
// MRAO Packer PRO - All Channels Optional
// Place in any "Editor" folder
// ------------------------------------------------------------------

using UnityEngine;
using UnityEditor;
using System.IO;

public class MRAOPackerPro : EditorWindow {
    Texture2D metallicTex;
    Texture2D roughnessTex;
    Texture2D aoTex;
    Texture2D alphaTex;

    bool invertRoughness = true;
    string saveName = "MRAO_Packed";

    [MenuItem("Tools/Pack MRAO Texture (All Channels Optional)")]
    static void Init() {
        var window = (MRAOPackerPro)GetWindow(typeof(MRAOPackerPro));
        window.titleContent = new GUIContent("MRAO Packer PRO");
        window.Show();
    }

    void OnGUI() {
        GUILayout.Label("MRAO Packer PRO", EditorStyles.boldLabel);
        GUILayout.Label("All channels are optional — empty = neutral value", EditorStyles.helpBox);

        GUILayout.Space(10);

        metallicTex = (Texture2D)EditorGUILayout.ObjectField("Metallic Map (R) - Optional", metallicTex, typeof(Texture2D), false);
        roughnessTex = (Texture2D)EditorGUILayout.ObjectField("Roughness Map (G) - Optional", roughnessTex, typeof(Texture2D), false);
        aoTex = (Texture2D)EditorGUILayout.ObjectField("AO Map (B) - Optional", aoTex, typeof(Texture2D), false);
        alphaTex = (Texture2D)EditorGUILayout.ObjectField("Alpha / Detail (A) - Optional", alphaTex, typeof(Texture2D), false);

        GUILayout.Space(10);
        invertRoughness = EditorGUILayout.Toggle("Invert Roughness (when used)", invertRoughness);
        saveName = EditorGUILayout.TextField("Output Filename", saveName);

        GUILayout.Space(20);

        // Count how many textures are assigned
        int assigned = 0;
        if (metallicTex) assigned++;
        if (roughnessTex) assigned++;
        if (aoTex) assigned++;
        if (alphaTex) assigned++;

        GUI.enabled = assigned > 0;

        if (GUILayout.Button("PACK MRAO TEXTURE", GUILayout.Height(50))) {
            PackFlexible();
        }

        GUI.enabled = true;

        GUILayout.Space(10);
        EditorGUILayout.HelpBox(
            "Neutral values used when channel is empty:\n" +
            "• Metallic (R) ? 0.0\n" +
            "• Roughness (G) ? 1.0 (fully rough)\n" +
            "• AO (B) ? 1.0 (no occlusion)\n" +
            "• Alpha (A) ? 1.0\n\n" +
            "All input textures must be same size if multiple are used.",
            MessageType.Info);
    }

    void PackFlexible() {
        // Determine final resolution from first assigned texture
        Texture2D refTex = metallicTex ?? roughnessTex ?? aoTex ?? alphaTex;
        if (!refTex) {
            EditorUtility.DisplayDialog("Error", "No texture assigned!", "OK");
            return;
        }

        int w = refTex.width;
        int h = refTex.height;

        // Validate sizes of assigned textures
        if ((metallicTex && (metallicTex.width != w || metallicTex.height != h)) ||
            (roughnessTex && (roughnessTex.width != w || roughnessTex.height != h)) ||
            (aoTex && (aoTex.width != w || aoTex.height != h)) ||
            (alphaTex && (alphaTex.width != w || alphaTex.height != h))) {
            EditorUtility.DisplayDialog("Error", "All assigned textures must be the same resolution!", "OK");
            return;
        }

        // Load readable versions
        Texture2D met = metallicTex ? LoadReadable(metallicTex) : null;
        Texture2D rou = roughnessTex ? LoadReadable(roughnessTex) : null;
        Texture2D ao = aoTex ? LoadReadable(aoTex) : null;
        Texture2D alp = alphaTex ? LoadReadable(alphaTex) : null;

        Texture2D packed = new Texture2D(w, h, TextureFormat.RGBA32, false);
        Color[] pixels = packed.GetPixels();

        for (int i = 0; i < pixels.Length; i++) {
            int x = i % w;
            int y = i / w;

            float m = met ? met.GetPixel(x, y).r : 0.0f;
            float r = rou ? (invertRoughness ? 1.0f - rou.GetPixel(x, y).r : rou.GetPixel(x, y).r) : 1.0f;
            float a = ao ? ao.GetPixel(x, y).r : 1.0f;
            float alpha = alp ? alp.GetPixel(x, y).r : 1.0f;

            pixels[i] = new Color(m, r, a, alpha);
        }

        packed.SetPixels(pixels);
        packed.Apply();

        // Save
        byte[] png = packed.EncodeToPNG();
        string path = EditorUtility.SaveFilePanelInProject(
            "Save Packed MRAO Texture",
            saveName + ".png",
            "png",
            "Save your MRAO texture");

        if (!string.IsNullOrEmpty(path)) {
            File.WriteAllBytes(path, png);
            AssetDatabase.Refresh();

            // Auto-configure import settings
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer) {
                importer.sRGBTexture = false;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();
            }

            EditorUtility.DisplayDialog("Success!", $"MRAO texture saved!\n{path}", "OK");
            EditorUtility.RevealInFinder(path);
        }

        // Cleanup
        DestroyImmediate(packed);
        if (met) DestroyImmediate(met);
        if (rou) DestroyImmediate(rou);
        if (ao) DestroyImmediate(ao);
        if (alp) DestroyImmediate(alp);
    }

    Texture2D LoadReadable(Texture2D src) {
        if (src.isReadable) return src;

        string path = AssetDatabase.GetAssetPath(src);
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        bool wasReadable = importer ? importer.isReadable : false;

        if (importer && !importer.isReadable) {
            importer.isReadable = true;
            importer.SaveAndReimport();
        }

        var rt = RenderTexture.GetTemporary(src.width, src.height, 0, RenderTextureFormat.ARGB32);
        Graphics.Blit(src, rt);
        var prev = RenderTexture.active;
        RenderTexture.active = rt;
        var readable = new Texture2D(src.width, src.height, TextureFormat.RGBA32, false);
        readable.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        readable.Apply();
        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(rt);

        // Restore original setting
        if (importer && !wasReadable) {
            importer.isReadable = false;
            importer.SaveAndReimport();
        }

        return readable;
    }
}
