using UnityEngine;

public class ObjectiveMarker : MonoBehaviour
{
    private ObjectiveManager manager;
    private Vector3 target;
    private bool isClamped;
    [SerializeField] private float bobHeight; 
    [SerializeField] private float bobSpeed;

    [SerializeField] private float maxXDistanceFromCam;
    [SerializeField] private float maxYDistanceFromCam;
    [SerializeField] private float margin;

    void Start()
    {
        manager = ObjectiveManager.Instance;
        manager.ObjectiveChanged += SetObjective;
        SetObjective();
    }

    void Update()
    {
        Vector3 trueTarget = GetTrueTarget();
        transform.position = trueTarget
        + new Vector3(
            0,
            isClamped ? 0 : Mathf.Sin(Time.time*bobSpeed)*bobHeight,
            0
        );
    }
    
    void SetObjective() {
        if (manager.HasObjective) {
            target = manager.Objectives[manager.CurrentObjective].transform.position;
        } else {

        }
    }

    Vector3 GetTrueTarget()
    {

        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new(0,0,0));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new(Camera.main.pixelWidth-1,Camera.main.pixelHeight-1,0));
        float topBorder = topRight.y - margin;
        float leftBorder = bottomLeft.x + margin;
        float bottomBorder = bottomLeft.y + margin;
        float rightBorder = topRight.x - margin;

        float x = target.x;
        float y = target.y;

        if (x > rightBorder)
        {
            x = rightBorder;
        } else if (x < leftBorder) {
            x = leftBorder;
        }
        if (y > topBorder)
        {
            y = topBorder;
        } else if (y < bottomBorder)
        {
            y = bottomBorder;
        }

        isClamped = x != target.x || y  != target.y;

        return new( x,y,0 );
    }
}
