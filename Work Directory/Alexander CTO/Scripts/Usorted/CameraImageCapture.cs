using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class CameraImageCapture : MonoBehaviour
{
    public Camera cam;

    public Texture2D CaptureImage(int imageWidth, int imageHeight)
    {
        var tempRenderTexture = new RenderTexture(imageWidth, imageHeight, 24);
        tempRenderTexture.Create();
        
        RenderTexture.active = tempRenderTexture;
        cam.targetTexture = tempRenderTexture;
        cam.Render();
        
        var image = new Texture2D(imageWidth, imageHeight);
        image.ReadPixels(new Rect(0,0, imageWidth, imageHeight), 0, 0 );
        image.Apply();
        
        cam.targetTexture = null;
        RenderTexture.active = null;

        return image;
    }


}


public static class IconsProcessor
{
    public static void SaveImageAsPNG(Texture2D image, string savePath, string imageName)
    {
        var pngEncodedImage = image.EncodeToPNG();
        File.WriteAllBytes($"{savePath}/{imageName}.png", pngEncodedImage);
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }
    
    public static void AddIconToTexture(Texture2D texture, Texture2D icon, int iconIndex)
    {
        var pixels = icon.GetPixels();
        var (x, y) = MapIconPosition(texture.width, icon.width, iconIndex);
        
        texture.SetPixels(x, y, icon.width, icon.width, pixels);
    }

    private static (int x, int y) MapIconPosition(int textureSize, int iconSize, int iconNumber)
    {
        var rowsCount = textureSize / iconSize;
        var xPosition = iconNumber % rowsCount;
        var yPosition = rowsCount - (iconNumber / rowsCount) - 1;
        
        return (xPosition * iconSize, yPosition * iconSize);       
    }
}