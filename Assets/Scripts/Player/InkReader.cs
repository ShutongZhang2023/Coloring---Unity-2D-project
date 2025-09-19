using UnityEngine;

public class InkReader : MonoBehaviour
{
    public RenderTexture inkMap;
    private Texture2D inkBufferTex;
    public int currentInkColor { get; private set; }
    private PhysicsCheck physicsCheck;

    private Texture2D inkBufferTexPrevious;
    private int frameInterval = 997;

    void Start()
    {
        physicsCheck = GetComponent<PhysicsCheck>();
        inkBufferTex = new Texture2D(inkMap.width, inkMap.height, TextureFormat.RGBA32, false);
        currentInkColor = 0; // 初始颜色为无色

        inkBufferTexPrevious = new Texture2D(inkMap.width, inkMap.height, TextureFormat.RGBA32, false);
    }

    void Update()
    {
        if (Time.frameCount % 5 == 0)
        {
            RenderTexture.active = inkMap;
            GL.Flush(); // 确保 GPU 写入完成
            inkBufferTex.ReadPixels(new Rect(0, 0, inkMap.width, inkMap.height), 0, 0);
            inkBufferTex.Apply();
            RenderTexture.active = null;
        }

        if (Time.frameCount % frameInterval == 0)
        {
            // 将当前 buffer 拷贝到 previous 中
            inkBufferTexPrevious.SetPixels(inkBufferTex.GetPixels());
            inkBufferTexPrevious.Apply();
        }

        //if (Time.frameCount % 100 == 0) // 每 100 帧导出一次查看
        //{
        //    SaveInkBufferToFile();
        //}

        if (physicsCheck.isGround) { 
        
        CheckInkUnderPlayer();
        }
    }

    void CheckInkUnderPlayer()
    {
        Vector3 worldPos = transform.position;
        Vector2 uv = WorldToUV(worldPos);

        int px = Mathf.Clamp((int)(uv.x * inkMap.width), 0, inkMap.width - 1);
        int py = Mathf.Clamp((int)((uv.y) * inkMap.height), 0, inkMap.height - 1);

        Color c = inkBufferTex.GetPixel(px, py);
        //Debug.Log($"[墨迹检测] 世界坐标: {worldPos} → UV: {uv} → 像素坐标: ({px}, {py}) → 颜色: {c}");

        if (c.r - c.b < 0.01 && c.r - c.g < 0.01) {
            currentInkColor = 0;
            return;
        }

        if (c.r > 0.7f && c.r < 0.8) currentInkColor = 1;
        else if (c.b > 0.8f) currentInkColor = 2;
        else if (c.g > 0.5f) currentInkColor = 3;
    }

    Vector2 WorldToUV(Vector3 worldPos)
    {
        Vector2 uv = new Vector2(worldPos.x + 50f, worldPos.y + 50f);
        uv /= 50f;
        return uv;
    }

    //检测之前当前位置的颜色
    public int checkPreviousColor() {
        Vector3 worldPos = transform.position;
        Vector2 uv = WorldToUV(worldPos);

        int px = Mathf.Clamp((int)(uv.x * inkMap.width), 0, inkMap.width - 1);
        int py = Mathf.Clamp((int)((uv.y) * inkMap.height), 0, inkMap.height - 1);

        Color c = inkBufferTexPrevious.GetPixel(px, py);

        if (c.r - c.b < 0.01 && c.r - c.g < 0.01)
        {
            return 0;
        }

        if (c.r > 0.7f && c.r < 0.8) return 1;
        else if (c.b > 0.8f) return 2;
        else if (c.g > 0.5f) return 3;

        return -1;
    }

    void SaveInkBufferToFile()
    {
        byte[] bytes = inkBufferTex.EncodeToPNG();
        string path = Application.dataPath + "/InkBufferDebug.png";
        System.IO.File.WriteAllBytes(path, bytes);

        byte[] bytes1 = inkBufferTexPrevious.EncodeToPNG();
        string path1 = Application.dataPath + "/InkBufferPrevious.png";
        System.IO.File.WriteAllBytes(path1, bytes1);
        Debug.Log($"已保存墨迹贴图到：{path}");
    }
}
