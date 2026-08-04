using UnityEngine;

public class MatrixFaceDetector : MonoBehaviour
{
    public static MatrixFaceDetector instance;

    private Camera cam;

    MainGameplayManager mainGM;

    [SerializeField] LayerMask detectedFaceLayer;

    [SerializeField] BoxCollider _currentDetectedFace;

    int matrixSideLength = 0;

    BoxCollider currentDetectedFace 
    {
        get
        {
            return _currentDetectedFace;
        }
        set
        {
            _currentDetectedFace = value;
            Debug.Log("Changed Value");
            detectedFaceValues = _currentDetectedFace.GetComponent<MatrixFaceDirection>();
            if (value != null && mainGM != null)
            {
                mainGM.ChangeAllCubeMaterials(detectedFaceValues, MouseController.instance.scrollDepthValue);
            }
        }
    }

    public MatrixFaceDirection detectedFaceValues;

    [SerializeField] BoxCollider[] faceColliders;

    Ray ray;
    RaycastHit hit;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
    }

    void Start()
    {
        mainGM = MainGameplayManager.instance;

        cam = Camera.main;

        //I find that the player can get slightly better views when the actual
        //ray is shot slightly above the center of the screen
        ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.4f, 0));

        matrixSideLength = GameManager.instance.matrixSideLength;

        //adding the code to readjust the faceColliders here just so that I don't have to be referencing them
        //elsewhere in the code
        for (int i = 0; i < faceColliders.Length; i++)
        {
            //Debug.Log("New Size: " + new Vector3(matrixSideLength, 0.001f, matrixSideLength).ToString());

            faceColliders[i].size = new Vector3(matrixSideLength, 0.001f, matrixSideLength);

            //this is fancy but fragile way to dynamically determine where the face colliders whould be moved to
            //when the game starts
            //faceColliders[0] = XMin side, [1] = XMax, [2] = YMin, etc.
            //using "%" against 0 will cause the game to crash - the +1 will prevent that from happening
            if ((i + 1) % 2 == 1)
                faceColliders[i].center = Vector3.down * (matrixSideLength / 2f);
            else
                faceColliders[i].center = Vector3.up * (matrixSideLength / 2f);
        }
    }

    void Update()
    {
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, detectedFaceLayer) &&
            currentDetectedFace != hit.collider)
        {
            currentDetectedFace = hit.collider as BoxCollider;
        }
    }
}
