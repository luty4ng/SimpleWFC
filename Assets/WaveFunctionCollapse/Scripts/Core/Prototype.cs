using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class Prototype : MonoBehaviour
{
    public float weight = 1.0f;
    public HorizontalFaceDetails Left;
    public VerticalFaceDetails Down;
    public HorizontalFaceDetails Back;
    public HorizontalFaceDetails Right;
    public VerticalFaceDetails Up;
    public HorizontalFaceDetails Forward;
    public FaceDetails[] Faces
    {
        get
        {
            return new FaceDetails[] {
                this.Left,
                this.Down,
                this.Back,
                this.Right,
                this.Up,
                this.Forward
            };
        }
    }

    public Mesh GetMesh(bool createEmptyFallbackMesh = true)
    {
        var meshFilter = this.GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            return meshFilter.sharedMesh;
        }
        if (createEmptyFallbackMesh)
        {
            var mesh = new Mesh();
            return mesh;
        }
        return null;
    }
    public bool CompareRotatedVariants(int r1, int r2)
    {
        if (!(this.Faces[Orientations.UP] as VerticalFaceDetails).Invariant || !(this.Faces[Orientations.DOWN] as VerticalFaceDetails).Invariant)
        {
            return false;
        }

        for (int i = 0; i < 4; i++)
        {
            var face1 = this.Faces[Orientations.Rotate(Orientations.HorizontalDirections[i], r1)] as HorizontalFaceDetails;
            var face2 = this.Faces[Orientations.Rotate(Orientations.HorizontalDirections[i], r2)] as HorizontalFaceDetails;

            if (face1.Socket != face2.Socket)
            {
                return false;
            }

            if (!face1.Symmetric && !face2.Symmetric && face1.Flipped != face2.Flipped)
            {
                return false;
            }
        }
        return true;
    }

#if UNITY_EDITOR
    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    static void DrawGizmo(Prototype prototype, GizmoType gizmoType)
    {
        for (int i = 0; i < 6; i++)
        {
            var face = prototype.Faces[i];
            Handles.Label(prototype.transform.position + Orientations.Rotations[i] * AbstractMap.BLOCK_SIZE / 2f, face.ToString());
        }
    }
#endif
}