using UnityEngine;

public class MoveObject : MonoBehaviour
{

    [SerializeField] GameObject sync_camera;
    [SerializeField] GameObject sync_light;
    [SerializeField] GameObject sync_pointlight;

    readonly Vector3 offset = new Vector3(0, 0.5f, 0);
    readonly Vector4 movesize = new Vector4(1, 1, 1, 90);

    void Awake()
    {
        if (sync_camera == null)
        {
            sync_camera = new GameObject("sync_camera", typeof(Camera));
        }
        if (sync_light == null)
        {
            sync_light = new GameObject("sync_light", typeof(Light));
        }
        if (sync_pointlight == null)
        {
            sync_pointlight = new GameObject("sync_pointlight", typeof(Light));
        }
        {
            sync_camera.transform.localPosition = new Vector3(0, offset.y/2, offset.y*-0.9f);
            sync_camera.GetComponent<Camera>().farClipPlane = 5.0f;
            sync_camera.GetComponent<Camera>().backgroundColor = Color.black;
            sync_camera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            sync_camera.transform.parent = transform;
        }
        {
            sync_light.transform.localPosition = Vector3.zero;
            sync_light.transform.localRotation = Quaternion.Euler(90, 0, 0);
            sync_light.GetComponent<Light>().type = LightType.Directional;
            sync_light.GetComponent<Light>().intensity = 1f;
            sync_light.GetComponent<Light>().lightmapBakeType = LightmapBakeType.Baked;
            sync_light.transform.parent = transform;
        }

        {
            sync_pointlight.transform.localPosition = offset;
            sync_pointlight.GetComponent<Light>().type = LightType.Point;
            sync_pointlight.GetComponent<Light>().intensity = 4f;
            sync_pointlight.GetComponent<Light>().lightmapBakeType = LightmapBakeType.Baked;
            sync_pointlight.transform.parent = transform;
        }


    }

    async void Start()
    {

#if !UNITY_WEBGL
        Application.targetFrameRate=20;
#endif
        UpdatePosText();

        var ret = "";
        while (ret != "P")
        {
            ret = await KeyGetter.Get();

            if (ret == "_U") await transform.TickMove(new Vector4(0, 0, 1, 0), movesize); //foward
            else if (ret == "_D") await transform.TickMove(new Vector4(0, 0, -1, 0), movesize);//back
            else if (ret == "L") await transform.TickMove(new Vector4(-1, 0, 0, 0), movesize);//left
            else if (ret == "R") await transform.TickMove(new Vector4(1, 0, 0, 0), movesize);//right
            else if (ret == "_L") await transform.TickMove(new Vector4(0, 0, 0, -1), movesize);//role the left
            else if (ret == "_R") await transform.TickMove(new Vector4(0, 0, 0, 1), movesize);//role the right

            UpdatePosText();
        }

    }
    void UpdatePosText() => Debug.Log(transform.ToPos(offset));


}//class
