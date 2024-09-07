using UnityEngine;

/*************This class handles simple camera movement/panning**************/
public class CameraMovement : MonoBehaviour
{
    public Transform cameraParent;

    public float panSpeed = 100f;

    public float minPanZ;
    public float maxPanZ;
    public float minPanX;
    public float maxPanX;
    public bool edgeScreenPan = true;
    public float edgeDistance = 10f;
    public float zoomSpeed = 30f;
    public float zoomSensitivity = 1f;
    public float zoomLevel;
    public float zoomPosition;
    public float minZoomY;
    public float maxZoomY;
    

    // Update is called once per frame
    void Update()
    {

        Vector3 ParentCamPos = cameraParent.transform.position;
        Vector3 Pos = transform.position;


        //Pan up and down with keys
        ParentCamPos = PanWithKeyboard(ParentCamPos);
        if(edgeScreenPan)
        {
            ParentCamPos = EdgePan(ParentCamPos);
        }


        Pos = Scroll(Pos);

        //Clamps the camera
        ParentCamPos = ClampPositions(ParentCamPos);

        cameraParent.transform.position = ParentCamPos;
        transform.position = Pos;
    }

    private Vector3 Scroll(Vector3 pos)
    {
        zoomLevel += Input.mouseScrollDelta.y * zoomSensitivity;
        zoomLevel = Mathf.Clamp(zoomLevel, minZoomY, maxZoomY);
        zoomPosition = Mathf.MoveTowards(zoomPosition, zoomLevel, zoomSpeed * Time.deltaTime);
        pos = cameraParent.position + (transform.forward * zoomPosition);
        return pos;
    }

    private Vector3 EdgePan(Vector3 pos)
    {
        if(Input.mousePosition.x >= Screen.width - edgeDistance)
        {
            pos.x += panSpeed * Time.deltaTime;
        }

        if (Input.mousePosition.x <= 0 + edgeDistance)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }

        if (Input.mousePosition.y >= Screen.height - edgeDistance)
        {
            pos.z += panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y <= 0 + edgeDistance)
        {
            pos.z -= panSpeed * Time.deltaTime;
        }

        return pos;

    }

    private Vector3 PanWithKeyboard(Vector3 pos)
    {
        if (Input.GetKey("w"))
        {
            pos.z += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s"))
        {
            pos.z -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a"))
        {
            pos.x -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d"))
        {
            pos.x += panSpeed * Time.deltaTime;
        }

        return pos;
    }

    private Vector3 ClampPositions(Vector3 pos)
    {
        pos.z = Mathf.Clamp(pos.z, minPanZ, maxPanZ);
        pos.x = Mathf.Clamp(pos.x, minPanX, maxPanX);
        return pos;
    }
}
