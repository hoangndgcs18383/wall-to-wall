#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using DG.DemiEditor;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct SpinSkinImage
{
    public Sprite sprite;
    public string imageName;
    public Vector2 size;
    public string format;
    public string filter;
    public string repeat;
    public string hash;
    public bool rotate;
    public Vector2 xy;
    public Vector2 sizeStr;
    public Vector2 orig;
    public Vector2 offset;
    public string index;
}

public partial class SpineSkeleton
{
    [JsonProperty("skeleton")] public Skeleton Skeleton { get; set; }

    [JsonProperty("bones")] public BoneElement[] Bones { get; set; }

    [JsonProperty("slots")] public Slot[] Slots { get; set; }

    [JsonProperty("skins")] public Skin[] Skins { get; set; }

    [JsonProperty("animations")] public Animations Animations { get; set; }
}

public partial class Animations
{
    [JsonProperty("hydro_idle")] public HydroIdle HydroIdle { get; set; }
}

public partial class HydroIdle
{
    [JsonProperty("bones")] public Bones Bones { get; set; }
}

public partial class Bones
{
    [JsonProperty("bone")] public BonesBone Bone { get; set; }
}

public partial class BonesBone
{
    [JsonProperty("translate")] public Scale[] Translate { get; set; }

    [JsonProperty("scale")] public Scale[] Scale { get; set; }
}

public partial class Scale
{
    [JsonProperty("y", NullValueHandling = NullValueHandling.Ignore)]
    public double? Y { get; set; }

    [JsonProperty("curve", NullValueHandling = NullValueHandling.Ignore)]
    public double? Curve { get; set; }

    [JsonProperty("c2", NullValueHandling = NullValueHandling.Ignore)]
    public double? C2 { get; set; }

    [JsonProperty("c3", NullValueHandling = NullValueHandling.Ignore)]
    public double? C3 { get; set; }

    [JsonProperty("time", NullValueHandling = NullValueHandling.Ignore)]
    public long? Time { get; set; }

    [JsonProperty("x", NullValueHandling = NullValueHandling.Ignore)]
    public double? X { get; set; }

    [JsonProperty("c4", NullValueHandling = NullValueHandling.Ignore)]
    public double? C4 { get; set; }
}

public partial class BoneElement
{
    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("parent", NullValueHandling = NullValueHandling.Ignore)]
    public string Parent { get; set; }
}

public partial class Skeleton
{
    [JsonProperty("hash")] public string Hash { get; set; }

    [JsonProperty("spine")] public string Spine { get; set; }

    [JsonProperty("x")] public long X { get; set; }

    [JsonProperty("y")] public double Y { get; set; }

    [JsonProperty("width")] public long Width { get; set; }

    [JsonProperty("height")] public long Height { get; set; }

    [JsonProperty("images")] public string Images { get; set; }

    [JsonProperty("audio")] public string Audio { get; set; }
}

public partial class Skin
{
    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("attachments")] public Attachments Attachments { get; set; }
}

public partial class Attachments
{
    [JsonProperty("hyro_skin")] public AttachmentsHyroSkin HyroSkin { get; set; }
}

public partial class AttachmentsHyroSkin
{
    [JsonProperty("skin")] public DefaultSkin DefaultSkin { get; set; }
}

public partial class DefaultSkin
{
    [JsonProperty("x")] public double X { get; set; }

    [JsonProperty("y")] public double Y { get; set; }

    [JsonProperty("width")] public long Width { get; set; }

    [JsonProperty("height")] public long Height { get; set; }
}

public partial class Slot
{
    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("bone")] public string Bone { get; set; }

    [JsonProperty("attachment")] public string Attachment { get; set; }
}

public partial class SpineSkeleton
{
    public static SpineSkeleton FromJson(string json) =>
        JsonConvert.DeserializeObject<SpineSkeleton>(json, Converter.Settings);
}

public static class Serialize
{
    public static string ToJson(this SpineSkeleton self) => JsonConvert.SerializeObject(self, Converter.Settings);
}

internal static class Converter
{
    public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Converters =
        {
            new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
        },
    };
}

[CreateAssetMenu(fileName = "SpineSkinTool", menuName = "Tools/SpineSkinTool")]
public class SpineSkinTool : ScriptableObject
{
    [SerializeField] private Texture templateTexture;
    [SerializeField] private List<Texture> textures;
    [FolderPath] public string rootPath;

    public SpinSkinImage[] images;
    [SerializeField] private TextAsset hydroAtlasAsset;

    [Button(ButtonSizes.Large)]
    [GUIColor(0.4f, 0.8f, 1f)]
    public void Parse()
    {
        //string atlasText = hydroAtlasAsset.text;
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < images.Length; i++)
        {
            if (images[i].sprite == null)
            {
                Debug.LogError($"Sprite is null at index {i}");
                break;
            }

            if (images[i].imageName.IsNullOrEmpty() || images[i].hash.IsNullOrEmpty() ||
                images[i].index.IsNullOrEmpty() || images[i].format.IsNullOrEmpty() ||
                images[i].filter.IsNullOrEmpty() || images[i].repeat.IsNullOrEmpty())
                continue;

            rootPath = rootPath.Replace("Assets", "");
            if (!Directory.Exists(Path.Combine(Application.dataPath, rootPath, $"{images[i].imageName}")))
            {
                Directory.CreateDirectory(Path.Combine(Application.dataPath, rootPath, $"{images[i].imageName}"));
            }

            sb.Append("\n");
            sb.AppendLine(images[i].imageName + ".png");
            sb.AppendLine($"size: {images[i].size.x},{images[i].size.y}");
            sb.AppendLine($"format: {images[i].format}");
            sb.AppendLine($"filter: {images[i].filter}");
            sb.AppendLine($"repeat: {images[i].repeat}");
            sb.AppendLine(images[i].hash);
            sb.AppendLine($"  rotate: {images[i].rotate.ToString().ToLower()}");
            sb.AppendLine($"  xy: {images[i].xy.x}, {images[i].xy.y}");
            sb.AppendLine($"  size: {images[i].sizeStr.x}, {images[i].sizeStr.y}");
            sb.AppendLine($"  orig: {images[i].orig.x}, {images[i].orig.y}");
            sb.AppendLine($"  offset: {images[i].offset.x}, {images[i].offset.y}");
            sb.AppendLine($"  index: {images[i].index}");

            File.WriteAllText(
                Path.Combine(Application.dataPath, rootPath, images[i].imageName, $"{images[i].imageName}.atlas.txt"),
                sb.ToString());

            ConvertSpriteToTexture2DToDefault(images[i].sprite, images[i].imageName);
            BuildSpineSkeleton(images[i].hash);
            sb.Clear();
        }

        //AssetDatabase.Refresh();
    }

    [Button(ButtonSizes.Large)]
    [GUIColor(0.4f, 0.8f, 1f)]
    public void ConvertTexture()
    {
        //to convert texture as spine export = templateTexture

        /*for (int i = 0; i < textures.Count; i++)
        {
            if (textures[i] == null)
            {
                Debug.LogError($"Texture is null at index {i}");
                break;
            }

            string path = AssetDatabase.GetAssetPath(textures[i]);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Single;
            textureImporter.spritePixelsPerUnit = 100;
            textureImporter.filterMode = FilterMode.Point;
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
            textureImporter.maxTextureSize = 2048;
            textureImporter.isReadable = true;
            textureImporter.mipmapEnabled = false;
            textureImporter.wrapMode = TextureWrapMode.Clamp;
            textureImporter.alphaIsTransparency = true;
            textureImporter.SaveAndReimport();
        }*/
        
        AssetDatabase.Refresh();
    }
    
    private void BuildSpineSkeleton(string skinName)
    {
        string atlasText = hydroAtlasAsset.text;
        SpineSkeleton spineSkeleton = SpineSkeleton.FromJson(atlasText);

        spineSkeleton.Slots[0].Name = skinName;
        spineSkeleton.Slots[0].Attachment = skinName;
        spineSkeleton.Skins[0].Name = "default";

        string json = spineSkeleton.ToJson();
        File.WriteAllText(Path.Combine(Application.dataPath, rootPath, skinName, $"{skinName}.json"), json);
    }

    private void ConvertSpriteToTexture2DToDefault(Sprite sprite, string imagePath)
    {
        Sprite newSprite = Sprite.Create(sprite.texture, sprite.rect, sprite.pivot);
        Texture2D texture2D = new Texture2D((int)newSprite.rect.width, (int)newSprite.rect.height);
        texture2D = BuildTextureToReadable(texture2D);
        texture2D.SetPixels(newSprite.texture.GetPixels((int)newSprite.rect.x, (int)newSprite.rect.y,
            (int)newSprite.rect.width, (int)newSprite.rect.height));

        byte[] bytes = texture2D.EncodeToPNG();
        File.WriteAllBytes(Path.Combine(Application.dataPath, rootPath, imagePath, $"{imagePath}.png"), bytes);
    }

    private Texture2D BuildTextureToReadable(Texture2D texture2D)
    {
        RenderTexture renderTexture = RenderTexture.GetTemporary(
            texture2D.width,
            texture2D.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);

        Graphics.Blit(texture2D, renderTexture);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTexture;
        Texture2D readableTexture = new Texture2D(texture2D.width, texture2D.height);
        readableTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        readableTexture.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTexture);
        return readableTexture;
    }

    /*private string CombinePath(params string[] paths)
    {
        string path = Application.dataPath;
        for (int i = 0; i < paths.Length; i++)
        {
            path = Path.Combine(path, paths[i]);
        }

        return path;
    }*/
}

#endif