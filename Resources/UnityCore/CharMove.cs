
using UnityEngine;
public class CharMove : MonoBehaviour
{
    CharacterController ctr;
    void Start(){
#if !UNITY_WEBGL
        Application.targetFrameRate = 20;
#endif
        if(ctr==null){
            gameObject.AddComponent<CharacterController>();
            ctr = gameObject.GetComponent<CharacterController>();
            ctr.height = 0.9f;
            ctr.skinWidth = ctr.height * 0.1f;
            ctr.radius = 0.4f;
            ctr.stepOffset = ctr.radius * 0.1f;
            ctr.minMoveDistance = 0.001f;
            ctr.slopeLimit = 45f;
        }
            ctr = gameObject.GetComponent<CharacterController>();
    }
    void Update()
    {
        //カメラをこのオブジェクトの下に加えると追従。
        float speed = 10.0f / 2;
        float rotspeed = 360f / 1;
        float gravity =9.8f;
        float ty = ctr.isGrounded?0:-1*gravity*gravity*Time.deltaTime;
        float tz = ctr.isGrounded?Input.GetAxis("Vertical") * speed * Time.deltaTime:0;
        float ry = ctr.isGrounded?Input.GetAxis("Horizontal") * rotspeed * Time.deltaTime:0;

        //transform.Translate(0, 0, tz);
        ctr.Move( transform.TransformDirection(0,ty,tz)); 
        transform.Rotate(0, ry, 0);

        if (Input.GetKeyDown("space")){
            transform.Round().RoundAngleY();
            //Debug.Log(ctr.isGrounded);
        }
    }
}//class

static class transformRoundExtension
{
    public static Transform Round(this Transform @this, bool yesX = true, bool yesY = false, bool yesZ = true)
    {
        var demi = 0.001f;//just 0.5 is upper;
        var p = @this.localPosition;
        var x = yesX ? Mathf.Round(p.x + demi) : p.x;
        var y = yesY ? Mathf.Round(p.y + demi) : p.y;
        var z = yesZ ? Mathf.Round(p.z + demi) : p.z;
        @this.localPosition = new Vector3(x, y, z);
        return @this;
    }
    public static Transform RoundAngleY(this Transform @this, float angle = 90)
    {
        var demi = angle / 2 + 0.001f;//just 0.5 is upper;
        var q = @this.localRotation.eulerAngles;
        var y = (int)(Mathf.Round(q.y + demi) / angle) * angle;
        @this.localRotation = Quaternion.Euler(q.x, y, q.z);
        return @this;
    }

}//class

