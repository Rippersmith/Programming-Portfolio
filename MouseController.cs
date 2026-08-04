using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
    public static MouseController instance;

    GameManager gm;
    MainGameplayManager mainGM;

    [SerializeField] Transform cubeRotationPoint;
    [SerializeField] float rotationSpeed = 50f;
    Vector3 lastMousePosition;

    [SerializeField] public int scrollDepthValue = 0;

    MatrixCubeScript currHilitedCube = null;
    MatrixCubeScript lastHilitedCube = null;

    [SerializeField] LayerMask cubeLayer;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.instance;
        mainGM = MainGameplayManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return; // Stop execution; the user clicked the UI
        }

        RaycastHit hit;
        Ray rayOrigin = Camera.main.ScreenPointToRay(Input.mousePosition);
    
        //if our mouse is over a cube & it isn't highlighted, then this code grabs that cube reference & changes its material to being highlighted
        if (Physics.Raycast(rayOrigin, out hit, Mathf.Infinity, cubeLayer) && hit.collider.GetComponent<MatrixCubeScript>() != null)
        {
            currHilitedCube = hit.collider.GetComponent<MatrixCubeScript>();
            currHilitedCube.ChangeToHilitedMaterial(true);
        }

        //if we DO have a cube highlighted & our mouse isn't over that cube, it changes the material back to normal
        if (lastHilitedCube != null &&
            (!Physics.Raycast(rayOrigin, out hit, Mathf.Infinity, cubeLayer) || currHilitedCube != lastHilitedCube))
        {
            lastHilitedCube.ChangeToHilitedMaterial(false);
        }

        lastHilitedCube = currHilitedCube;

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(rayOrigin, out hit, Mathf.Infinity, cubeLayer))
            {
                //Debug.Log(hit.collider.gameObject.name);

                MatrixCubeScript newMatrixCube = hit.collider.GetComponent<MatrixCubeScript>();

                if (newMatrixCube != null && newMatrixCube.claimedPlayerValue == PlayerValue.NULL)
                {
                    newMatrixCube.SetNewPlayerValue(MainGameplayManager.instance.currPlayerTurn);
                    MainGameplayManager.instance.PlayerClaimsNewCube(newMatrixCube.cubeLocValue, newMatrixCube.claimedPlayerValue);
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
            lastMousePosition = Input.mousePosition;

        else if (Input.GetMouseButton(1))
        {
            // Calculate how far the mouse has moved since the last frame
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;

            // Map mouse movement to rotation axes
            float rotationX = mouseDelta.x * rotationSpeed * Time.deltaTime;
            float rotationY = mouseDelta.y * rotationSpeed * Time.deltaTime;

            // Rotate around World Up (Y) and Camera Right (X) to feel natural
            cubeRotationPoint.Rotate(Vector3.up, -rotationX, Space.World);
            cubeRotationPoint.Rotate(Camera.main.transform.right, rotationY, Space.World);

            // Store current position for the next frame
            lastMousePosition = Input.mousePosition;
        }

        //Keep the scroll value between 0 & matrix side length - 1
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && 
            scrollDepthValue < GameManager.instance.matrixSideLength - 1)
        {
            scrollDepthValue++;
            OnChangeScrollDepth();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && 
            scrollDepthValue > 0 )
        {
            scrollDepthValue--;
            OnChangeScrollDepth();
        }
    }

    void OnChangeScrollDepth()
    {
        mainGM.ChangeAllCubeMaterials(MatrixFaceDetector.instance.detectedFaceValues, scrollDepthValue);
    }

    //Whenever we update cube materials, we also have to reset the Hilited cube - 
    //I've had some troubles changing its layer otherwise
    public void ResetHilitedCube()
    {
        currHilitedCube = null;
        if (lastHilitedCube != null)
        {
            lastHilitedCube.ChangeCubeTransparency(false);
            lastHilitedCube.ChangeToHilitedMaterial(false);
            lastHilitedCube = null;
        }
    }
}
