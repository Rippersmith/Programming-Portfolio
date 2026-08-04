using UnityEngine;

//separate the values by 10 to make it easy to numerically sort them later on
public enum FaceDirection { XMin = 0, YMin = 1, ZMin = 2, XMax = 10, YMax = 11, ZMax = 12 }

//This class is just storage for the collider on each face of the matrix
public class MatrixFaceDirection : MonoBehaviour
{
    public FaceDirection faceDirValue;
    public Vector3 direction;
}
