using System.Collections.Generic;
using UnityEngine;

namespace shapegrammar
{
    public class Shape
    {
        // Dimensions
        List<Vector3> boundary = new List<Vector3>();
        // GameObject representation in scene
        GameObject go;
        // Parent shape
        Shape parentShape;
        // Type, used to sub-divide
        ShapeType shapeType;
        // Shapes created from this one
        List<Shape> children;
        // Track how many ancestors shape has
        int ancestryDepth;

        public Shape(Vector3 centre, List<Vector3> _boundary, Shape _parent, ShapeType _shapeType, int _ancestryDepth)
        {
            boundary = _boundary;
            ancestryDepth = _ancestryDepth;
            parentShape = _parent;
            shapeType = _shapeType;
            children = new List<Shape>();
            // Create renderable game object for leaf shapes
            if (shapeType == ShapeType.final)
            {
                go = new GameObject();
                go.transform.position = MakeBoundaryRelative(boundary);
                go.name = "Shape" + ancestryDepth;
                ConstructMesh(go, _boundary, go.transform.position);
                // Debug scaling
                go.transform.localScale = new Vector3(0.99f, 0.99f, 0.99f);
            }
        }

        public void ConstructMesh(GameObject g, List<Vector3> _boundary, Vector3 centrePoint)
        {
            g.AddComponent<MeshFilter>();
            g.AddComponent<MeshRenderer>();
            Mesh mesh = g.GetComponent<MeshFilter>().mesh;

            mesh.Clear();

            List<Vector3> verts = new List<Vector3>();
            verts.Add(centrePoint);
            verts.AddRange(_boundary);

            mesh.vertices = verts.ToArray();
            List<int> tris = new List<int>();
            for (int i = 3; i < verts.Count; i++)
            {
                tris.Add(0);
                tris.Add(i - 1);
                tris.Add(i);
            }
            mesh.triangles = tris.ToArray();

            /*
            if (_boundary.Count == 3)
            {
                // shape is a triangle
                mesh.triangles = new int[] { 0, 1, 2 };
                mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) };
            }
            else if (_boundary.Count == 4)
            {
                // shape is a rectangle
                mesh.triangles = new int[] { 0, 1, 2, 2, 3, 0 };
                mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 1) };

            }
            else
            {
                // shape is a pentagon
                mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4};
                mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 0.5f), new Vector2(0.5f, 1), new Vector2(1, 0.5f), new Vector2(1, 0) };
            }
            */
        }

        public List<Shape> GenerateChildShapes()
        {
            if (shapeType != ShapeType.final)
            {
                // Just halve them in either dimension for now
                int edgeStartingSplit = (int)Random.Range(0, boundary.Count-1);
                float tStartingSplit = Random.Range(0f, 1.0f);
                int edgeEndingSplit = (int)Random.Range(edgeStartingSplit+1, boundary.Count);
                float tEndingSplit = Random.Range(0f, 1.0f);
                // Perform split
                foreach (var childBoundary in SplitBoundaryAlongLengths(edgeStartingSplit, tStartingSplit, edgeEndingSplit, tEndingSplit))
                {
                    ShapeType childType = (ShapeType)(int)Random.Range(0, (int)ShapeType.COUNT);
                    Debug.Log("Childtype: " + childType);
                    children.Add(new Shape(Vector3.zero, childBoundary, this, childType, ancestryDepth+1));
                }
            }
            return children;
        }

        public List<List<Vector3>> SplitBoundaryAlongLengths(int startSplitAfterPoint, float percentageFirstSplit, int endSplitAfterPoint, float percentageSecondSplit)
        {
            List<Vector3> childOneBoundary = new List<Vector3>();
            List<Vector3> childTwoBoundary = new List<Vector3>();
            bool beforeSplit = true;
            for (int i = 0; i < boundary.Count; i++)
            {
                // Add next point
                if (beforeSplit)
                {
                    childOneBoundary.Add(boundary[i]);
                    if (startSplitAfterPoint == i)
                    {
                        int nextIndex = (i + 1) % boundary.Count;
                        Vector3 diffV = (boundary[nextIndex] - boundary[i]) * percentageFirstSplit;
                        Vector3 midPoint = boundary[i] + diffV;
                        childOneBoundary.Add(midPoint);
                        childTwoBoundary.Add(midPoint);
                        beforeSplit = false;
                    }
                }
                else
                {
                    childTwoBoundary.Add(boundary[i]);
                    if (endSplitAfterPoint == i)
                    {
                        int nextIndex = (i + 1) % boundary.Count;
                        Vector3 diffV = (boundary[nextIndex] - boundary[i]) * percentageSecondSplit;
                        Vector3 midPoint = boundary[i] + diffV;
                        childOneBoundary.Add(midPoint);
                        childTwoBoundary.Add(midPoint);
                        beforeSplit = true;
                    }
                }
            }
            List<List<Vector3>> childBoundaries = new List<List<Vector3>>();
            childBoundaries.Add(childOneBoundary);
            childBoundaries.Add(childTwoBoundary);
            return childBoundaries;
        }

        public Vector3 MakeBoundaryRelative(List<Vector3> b)
        {
            Vector3 summedPoints = new Vector3();
            foreach (var point in b)
            {
                summedPoints += point;
            }
            Vector3 centrePoint = summedPoints / b.Count;
            for (int i = 0; i < b.Count; i++)
            {
                b[i] = b[i] - centrePoint;
            }
            return centrePoint;
        }
    }

    public enum ShapeType
    {
        final = 0,
        start = 1,
        COUNT = 2
    }
}