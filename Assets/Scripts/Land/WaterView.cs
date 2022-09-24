using Cat;
using UnityEngine;

namespace Land
{
    public class WaterView: MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D col)
        {
            ICatView cat = col.GetComponent<ICatView>();
            cat?.Kill();
        }
    }
}