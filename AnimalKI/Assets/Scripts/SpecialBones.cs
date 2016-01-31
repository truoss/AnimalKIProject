using UnityEngine;
using System.Collections;

public class SpecialBones : MonoBehaviour
{
    public Transform Hip;
    public Transform Head;
    public Transform HandL;
    public Transform HandR;
    public Transform FootL;
    public Transform FootR;
    public Transform TailTip;

    [ContextMenu("FindBones")]
    public void FindBones()
    {
        Debug.LogWarning("Not yet implemented!");
    }
}
