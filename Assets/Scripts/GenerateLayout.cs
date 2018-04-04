using System.Collections.Generic;
using UnityEngine;
using shapegrammar;

public class GenerateLayout : MonoBehaviour {
    Shape rootShape;
    List<Shape> unprocessedChildren;

	// Use this for initialization
	void Start () {
        unprocessedChildren = new List<Shape>();
        rootShape = new Shape(Vector3.zero, RandomStartingBoundary(), null, ShapeType.start, 0);
        unprocessedChildren.Add(rootShape);
        // Decompose shapes into children
        if (unprocessedChildren.Count != 0)
        {
            // Extract highest priority shape from unprocessed list
            Shape nextShape = unprocessedChildren[0];
            unprocessedChildren.RemoveAt(0);
            // Shape's children are unprocessed
            unprocessedChildren.AddRange(nextShape.GenerateChildShapes());
        }
	}

    public List<Vector3> RandomStartingBoundary()
    {
        List<Vector3> boundary = new List<Vector3>();
        float xSize = Random.Range(3.0f, 10.0f);
        float zSize = Random.Range(3.0f, 9.0f);

        boundary.Add(new Vector3(-xSize * 0.5f, 0, zSize * 0.5f));
        boundary.Add(new Vector3(xSize * 0.5f, 0, zSize * 0.5f));
        boundary.Add(new Vector3(xSize * 0.5f, 0, -zSize * 0.5f));
        boundary.Add(new Vector3(-xSize * 0.5f, 0, -zSize * 0.5f));

        return boundary;
    }

    public List<Vector3> SimpleStartingBoundary()
    {
        List<Vector3> boundary = new List<Vector3>();
        float xSize = 10f;
        float zSize = 6f;

        boundary.Add(new Vector3(-xSize * 0.5f, 0, zSize * 0.5f));
        boundary.Add(new Vector3(xSize * 0.5f, 0, zSize * 0.5f));
        boundary.Add(new Vector3(xSize * 0.5f, 0, -zSize * 0.5f));
        boundary.Add(new Vector3(-xSize * 0.5f, 0, -zSize * 0.5f));
        return boundary;
    }

}