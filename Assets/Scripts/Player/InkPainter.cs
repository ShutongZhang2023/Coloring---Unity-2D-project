using System.Collections.Generic;
using UnityEngine;


public class InkPainter : MonoBehaviour
{
    [System.Serializable]
    public class InkMapEntry
    {
        public Vector2Int index;
        public RenderTexture inkMap;
    }

    public List<InkMapEntry> inkMapList = new();
    private Dictionary<Vector2Int, RenderTexture> inkMaps = new(); //use dictionary for fast lookup

    public Material brushMaterial;
    public int brushSize = 32;

    private float cellWidth = 50f; 
    private float cellHeight = 30f; 

    public int currentInkColor; //Reader

    private RenderTexture currentInkMap;
    private Texture2D readBuffer; //Reader
    private Texture2D previousBuffer;
    private Vector2Int currentIndex;
    private int readInterval = 5;
    
    private PhysicsCheck physicsCheck;
    private int frameInterval = 997;


    private void Start()
    {
        physicsCheck = GetComponent<PhysicsCheck>();

        // initalize inkMaps dictionary (fast look up)
        foreach (var entry in inkMapList)
        {
            if (entry != null && entry.inkMap != null)
            {
                inkMaps[entry.index] = entry.inkMap;
            }
        }

        // 获取角色初始所在区域
        currentIndex = GetInkMapIndex(transform.position);
        inkMaps.TryGetValue(currentIndex, out currentInkMap);

        // 初始化读取 buffer
        if (currentInkMap != null)
        {
            readBuffer = new Texture2D(currentInkMap.width, currentInkMap.height, TextureFormat.RGBA32, false); // === Reader ===
            previousBuffer = new Texture2D(currentInkMap.width, currentInkMap.height, TextureFormat.RGBA32, false);
        }
    }

    void Update()
    {
        Vector2Int indexNow = GetInkMapIndex(transform.position);

        // update current index and current ink map if necessary
        if (indexNow != currentIndex)
        {
            currentIndex = indexNow;
            inkMaps.TryGetValue(currentIndex, out currentInkMap);

            if (currentInkMap != null)
            {
                readBuffer = new Texture2D(currentInkMap.width, currentInkMap.height, TextureFormat.RGBA32, false); //读进read buffer
            }
        }

        if (currentInkMap == null)
            return;

        // draw
        Vector2 uv = LocalToUV(transform.position, currentIndex);
        DrawAtUV(currentInkMap, uv);

        //read
        if (Time.frameCount % readInterval == 0)
        {
            RenderTexture.active = currentInkMap;
            GL.Flush(); // ensure all rendering commands are executed
            readBuffer.ReadPixels(new Rect(0, 0, currentInkMap.width, currentInkMap.height), 0, 0);
            readBuffer.Apply();
            RenderTexture.active = null;
        }

        if (Time.frameCount % frameInterval == 0)
        {
            Graphics.CopyTexture(readBuffer, previousBuffer);
        }

        if (physicsCheck.isGround)
        {
            CheckInkUnderPlayer();
        }
    }

    //计算当前所在的inkmap索引
    Vector2Int GetInkMapIndex(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / cellWidth);
        int y = Mathf.FloorToInt(worldPos.y / cellHeight);
        return new Vector2Int(x, y);
    }


    //绘制的部分
    //计算局部UV坐标
    Vector2 LocalToUV(Vector3 worldPos, Vector2Int index)
    {
        float localX = worldPos.x - index.x * cellWidth;
        float localY = worldPos.y - index.y * cellHeight;

        float uvX = Mathf.Clamp01(localX / cellWidth);
        float uvY = Mathf.Clamp01(localY / cellHeight);

        return new Vector2(uvX, uvY);
    }

    void DrawAtUV(RenderTexture inkMap, Vector2 uv)
    {
        PlayerBasic playerBasic = GetComponent<PlayerBasic>();
        switch (playerBasic.currentColorId) {
            case 1:
                brushMaterial.color = new Color(0.9f, 0.23f, 0.25f, 1f);
                break;
            case 2:
                brushMaterial.color = new Color(0.39f, 0.51f, 1f, 1f);
                break;
            case 3:
                brushMaterial.color = new Color(0.94f, 0.75f, 0.35f, 1f);
                break;
            default:
                brushMaterial.color = Color.white;
                break;
        }
        //RenderTexture activeRT = RenderTexture.active;
        RenderTexture.active = inkMap;

        GL.PushMatrix();
        GL.LoadPixelMatrix(0, inkMap.width, inkMap.height, 0);

        brushMaterial.SetPass(0);

        float px = uv.x * inkMap.width;
        float py = (1f - uv.y) * inkMap.height;

        Graphics.DrawTexture(
            new Rect(px - brushSize / 2, py - brushSize / 2, brushSize, brushSize),
            Texture2D.whiteTexture,
            brushMaterial
        );

        GL.PopMatrix();
        RenderTexture.active = null;
    }

    //public void ClearInkMap()
    //{
    //    RenderTexture.active = inkMap;
    //    GL.Clear(true, true, new Color(0, 0, 0, 0));
    //    RenderTexture.active = null;
    //}

    void CheckInkUnderPlayer()
    {
        Vector2 uv = LocalToUV(transform.position, currentIndex);

        int px = Mathf.Clamp((int)(uv.x * readBuffer.width), 0, readBuffer.width - 1);
        int py = Mathf.Clamp((int)((uv.y) * readBuffer.height), 0, readBuffer.height - 1);

        Color c = readBuffer.GetPixel(px, py);

        if (c.r - c.b < 0.01 && c.r - c.g < 0.01 && c.r - c.g > 0)
        {
            currentInkColor = 0;
            return;
        }

        if (c.r > 0.7f && c.r < 0.8) currentInkColor = 1;
        else if (c.b > 0.8f) currentInkColor = 2;
        else if (c.g > 0.5f) currentInkColor = 3;
    }

    public int checkPreviousColor()
    {
        Vector2 uv = LocalToUV(transform.position, currentIndex);

        int px = Mathf.Clamp((int)(uv.x * readBuffer.width), 0, readBuffer.width - 1);
        int py = Mathf.Clamp((int)((uv.y) * readBuffer.height), 0, readBuffer.height - 1);

        Color c = previousBuffer.GetPixel(px, py);

        if (c.r - c.b < 0.01 && c.r - c.g < 0.01 && c.r - c.g > 0)
        {
            return 0;
        }

        if (c.r > 0.7f && c.r < 0.8) return 1;
        else if (c.b > 0.8f) return 2;
        else if (c.g > 0.5f) return 3;

        return -1;
    }
}
