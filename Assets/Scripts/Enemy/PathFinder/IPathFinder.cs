using System.Collections.Generic;
using UnityEngine;

public interface IPathFinder 
{
    List<Vector2> CalculatePath(Vector2 start, Vector2 end);
}
