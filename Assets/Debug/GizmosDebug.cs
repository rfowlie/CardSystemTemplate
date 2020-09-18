using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmosDebug : MonoBehaviour
{
    public Color colour = Color.black;
    public enum DebugShape { CUBE, SPHERE, WIRECUBE, WIRESPHERE, WIRERECTANGLE }
    public DebugShape shape = DebugShape.CUBE;
    public float size = 1f;

    private void OnDrawGizmos()
    {
        Gizmos.color = colour;
        if(del != null)
        {           
            del();   
        }
    }

    //set delegate that runs in draw gizmos
    private delegate void DEL();
    private DEL del = null;
    private void Configure()
    {
        del = null;

        switch (shape)
        {
            case DebugShape.CUBE:
                del += () => Gizmos.DrawCube(transform.position, new Vector3(size, size, size));
                break;
            case DebugShape.SPHERE:
                del += () => Gizmos.DrawSphere(transform.position, size / 2f);
                break;
            case DebugShape.WIRECUBE:
                del += () => Gizmos.DrawWireCube(transform.position, new Vector3(size, size, size));
                break;
            case DebugShape.WIRESPHERE:
                del += () => Gizmos.DrawWireSphere(transform.position, size / 2f);
                break;
            default:
                Debug.LogError("DebugShape Doesn't Exist!!");
                break;
        }
    }

    //gets called only in EDITOR MODE and only when values in this script are updated
    private void OnValidate()
    {
        Configure();
    }
}
