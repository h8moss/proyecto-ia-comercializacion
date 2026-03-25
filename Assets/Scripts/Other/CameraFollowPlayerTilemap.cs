using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Camera))]
public class CameraFollowPlayerTilemap : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float lerp;
    [SerializeField] private Tilemap tilemap;

    private Transform player;
    private Camera cam;
    private Bounds mapBounds;

    void Start()
    {
        player = PlayerLocator.Player;
        cam = GetComponent<Camera>();
        mapBounds = ComputeTileBounds();
    }

    void LateUpdate()
    {
        Vector3 target = player.position + offset;
        Vector3 newPos = Vector3.Lerp(transform.position, target, lerp * Time.deltaTime);
        newPos = ClampToMap(newPos);
        transform.position = newPos;
    }

    Bounds ComputeTileBounds()
    {
        tilemap.CompressBounds();
        BoundsInt cells = tilemap.cellBounds;

        bool first = true;
        Bounds b = new Bounds();

        foreach (var pos in cells.allPositionsWithin)
        {
            if (!tilemap.HasTile(pos)) continue;

            // Get the four corners of this tile in world space
            Vector3 origin = tilemap.CellToWorld(pos);
            Vector3 size   = tilemap.cellSize;

            Vector3 tileMin = origin;
            Vector3 tileMax = origin + size;

            if (first)
            {
                b = new Bounds((tileMin + tileMax) * 0.5f, tileMax - tileMin);
                first = false;
            }
            else
            {
                b.Encapsulate(tileMin);
                b.Encapsulate(tileMax);
            }
        }

        return b;
    }

    Vector3 ClampToMap(Vector3 pos)
    {
        float halfH = cam.orthographicSize;
        float halfW = cam.orthographicSize * cam.aspect;

        pos.x = Mathf.Clamp(pos.x, mapBounds.min.x + halfW, mapBounds.max.x - halfW);
        pos.y = Mathf.Clamp(pos.y, mapBounds.min.y + halfH, mapBounds.max.y - halfH);

        return pos;
    }
}
