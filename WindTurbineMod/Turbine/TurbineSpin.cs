using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindTurbinesMod.WindTurbine
{
    public class TurbineSpin : MonoBehaviour
    {
        public float spinSpeed = 0f;

        public void Update()
        {
            transform.Rotate(new Vector3(-100f * spinSpeed * Time.deltaTime, 0, 0));
        }
    }
}
