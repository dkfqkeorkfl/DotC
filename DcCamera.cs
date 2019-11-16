using UnityEngine;
using System.Collections;
using UniRx;
public class DcCamera : MonoBehaviour, DC.Activator
{
    public Transform cameraTarget;
    public TouchScript.Gestures.TransformGestures.ScreenTransformGesture ManipulationGesture;

    private float x = 0.0f;
    private float y = 0.0f;

    public bool is_active { get; private set; }
    public float mouseXSpeedMod = 0.01f;
    public float mouseySpeedMod = 0.01f;

    public float maxViewDistance = 25.0f;
    public float minViewDistance = 1.0f;
    public int zoomRate = 30;
    public float lerpRate = 1.0f;
    private float distance = 3.0f;
    private float desiredDistance;
    private float correctedDistance;
    private float currentDistance;

    public float cameraTargetHeight = 1.0f;

    Vector3 gesture;

    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x += angles.x;
        y -= angles.y;

        currentDistance = distance;
        desiredDistance = distance;
        correctedDistance = distance;
    }

    void LateUpdate()
    {
        //if (Input.GetMouseButton(1))
        //{
        //    x += Input.GetAxis("Mouse X") * mouseXSpeedMod;
        //    y -= Input.GetAxis("Mouse Y") * mouseySpeedMod;
        //    Debug.Log(string.Format("x{0} y{1} z{2}", x, y, 0));
        //}

        //else if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        //{
        //    float targetRotationAngle = cameraTarget.eulerAngles.y;
        //    float cameraRotaionAngle = transform.eulerAngles.y;
        //    x = Mathf.LerpAngle(cameraRotaionAngle, targetRotationAngle, lerpRate * Time.deltaTime);
        //}

        //y = ClampAngle(y, -50, 50);

        //Quaternion rotation = Quaternion.Euler(y, x, 0);

        //if(Input.GetAxis("Mouse ScrollWheel") > Mathf.Epsilon || Input.GetAxis("Mouse ScrollWheel") < -Mathf.Epsilon)
        //    Debug.Log(string.Format("x{0} y{1} z{2}", x, y, Input.GetAxis("Mouse ScrollWheel")));
        //desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);


        if (ManipulationGesture.NumPointers == 2)
        {
            x += gesture.x * mouseXSpeedMod;
            y -= gesture.y * mouseySpeedMod;
        }
        else if(ManipulationGesture.NumPointers == 0)
        {
            is_active = false;
        }

        y = ClampAngle(y, -30, 30);

        Quaternion rotation = Quaternion.Euler(y, x, 0);

        desiredDistance -= gesture.z * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
        gesture.z = 0.0f;

        desiredDistance = Mathf.Clamp(desiredDistance, minViewDistance, maxViewDistance);
        correctedDistance = desiredDistance;

        Vector3 position = cameraTarget.position - (rotation * Vector3.forward * desiredDistance);

        RaycastHit collisionHit;
        Vector3 cameraTargetPosition = new Vector3(cameraTarget.position.x, cameraTarget.position.y + cameraTargetHeight, cameraTarget.position.z);

        bool isCorrected = false;
        if (Physics.Linecast(cameraTargetPosition, position, out collisionHit))
        {
            position = collisionHit.point;
            correctedDistance = Vector3.Distance(cameraTargetPosition, position);
            isCorrected = true;
        }

        currentDistance = !isCorrected || correctedDistance > currentDistance ? Mathf.Lerp(currentDistance, correctedDistance, Time.deltaTime * zoomRate) : correctedDistance;

        position = cameraTarget.position - (rotation * Vector3.forward * currentDistance + new Vector3(0, -cameraTargetHeight, 0));

        transform.rotation = rotation;
        transform.position = position;
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
        {
            angle += 360;
        }
        if (angle > 360)
        {
            angle -= 360;
        }
        return Mathf.Clamp(angle, min, max);
    }

    private void OnEnable()
    {
        is_active = false;
        ManipulationGesture.Transformed += manipulationTransformedHandler;
    }

    private void OnDisable()
    {
        is_active = false;
        ManipulationGesture.Transformed -= manipulationTransformedHandler;
    }

    private void manipulationTransformedHandler(object sender, System.EventArgs e)
    {
        is_active = true;
        var x = ManipulationGesture.ActivePointers[1].Position.x - ManipulationGesture.ActivePointers[0].Position.x;
        var y = ManipulationGesture.ActivePointers[1].Position.y - ManipulationGesture.ActivePointers[0].Position.y;
        x %= 360;
        y %= 360;

        gesture.x = x;
        gesture.y = y;
        gesture.z = (ManipulationGesture.DeltaScale - 1);
    }
}


//using TouchScript.Gestures.TransformGestures;

//namespace TouchScript.Examples.CameraControl
//{
//    /// <exclude />
//    public class CameraController : MonoBehaviour
//    {
//        public ScreenTransformGesture TwoFingerMoveGesture;
//        public ScreenTransformGesture ManipulationGesture;
//        public float PanSpeed = 200f;
//        public float RotationSpeed = 200f;
//        public float ZoomSpeed = 10f;

//        private Transform pivot;
//        private Transform cam;

//        private void Awake()
//        {
//            pivot = transform.Find("Pivot");
//            cam = transform.Find("Pivot/Camera");
//        }

//        private void OnEnable()
//        {
//            TwoFingerMoveGesture.Transformed += twoFingerTransformHandler;
//            ManipulationGesture.Transformed += manipulationTransformedHandler;
//        }

//        private void OnDisable()
//        {
//            TwoFingerMoveGesture.Transformed -= twoFingerTransformHandler;
//            ManipulationGesture.Transformed -= manipulationTransformedHandler;
//        }

//        private void manipulationTransformedHandler(object sender, System.EventArgs e)
//        {
//            var rotation = Quaternion.Euler(ManipulationGesture.DeltaPosition.y / Screen.height * RotationSpeed,
//                -ManipulationGesture.DeltaPosition.x / Screen.width * RotationSpeed,
//                ManipulationGesture.DeltaRotation);
//            pivot.localRotation *= rotation;
//            cam.transform.localPosition += Vector3.forward * (ManipulationGesture.DeltaScale - 1f) * ZoomSpeed;
//        }

//        private void twoFingerTransformHandler(object sender, System.EventArgs e)
//        {
//            pivot.localPosition += pivot.rotation * TwoFingerMoveGesture.DeltaPosition * PanSpeed;
//        }
//    }
//}