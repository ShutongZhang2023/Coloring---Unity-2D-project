using UnityEngine;
using System.IO;

public class PlatformMaskGenerator : MonoBehaviour
{
    public Camera renderCam;
    public RenderTexture maskTexture;
    public string savePath = "Assets/Material/Ink/PlatformMaskFromView.png";
    private void Start()
    {
        CaptureTilemapAsMask();
    }
    void CaptureTilemapAsMask()
    {
        RenderTexture.active = maskTexture;
        renderCam.targetTexture = maskTexture;
        renderCam.Render();

        Texture2D tex = new Texture2D(maskTexture.width, maskTexture.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, maskTexture.width, maskTexture.height), 0, 0);
        tex.Apply();

        for (int y = 0; y < tex.height; y++)
        {
            for (int x = 0; x < tex.width; x++)
            {
                Color c = tex.GetPixel(x, y);
                float g = c.grayscale;
                tex.SetPixel(x, y, new Color(g, g, g, 1));
            }
        }

        byte[] png = tex.EncodeToPNG();
        File.WriteAllBytes(savePath, png);
        Debug.Log($"平台遮罩已捕捉并保存: {savePath}");

        RenderTexture.active = null;
        renderCam.targetTexture = null;
    }
}
