using UnityEngine;
using UnityEngine.Tilemaps;

public class ColorTrail : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase redTile;
    public TileBase blueTile;
    public TileBase yellowTile;
    private PlayerController playerController;
    private TileBase targetTile;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //change tile color
        if (playerController.currentColorId == 1)
            targetTile = redTile;
        if (playerController.currentColorId == 2)
            targetTile = blueTile;
        if (playerController.currentColorId == 3)
            targetTile = yellowTile;


        foreach (ContactPoint2D contact in collision.contacts)
        {
            Vector3 point = contact.point;
            Vector3 normal = contact.normal;

            // 偏移一下，往 tilemap 内部取点
            Vector3 contactPoint = point + normal * -0.05f;

            Vector3Int cellPos = tilemap.WorldToCell(contactPoint);

            TileBase currentTile = tilemap.GetTile(cellPos);

            //deal with the logic before color changing

            //1. interaction between red and yellow


            //change tile color
            if (currentTile != null && currentTile != targetTile)
            {
                tilemap.SetTile(cellPos, targetTile);
            }
        }
    }
}
