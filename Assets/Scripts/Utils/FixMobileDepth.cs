using UnityEngine;

namespace Amir.Utils
{
    [ExecuteInEditMode]
    public class FixMobileDepth : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
        }
    }
}
