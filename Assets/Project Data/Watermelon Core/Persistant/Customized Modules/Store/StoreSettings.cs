using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Store Settings", menuName = "Content/Store Settings")]
    public class StoreSettings : ScriptableObject
    {
        public StoreProduct[] products;

        public void Init()
        {
            for (int i = 0; i < products.Length; i++)
            {
                products[i].Init();
            }
        }
    }
}
