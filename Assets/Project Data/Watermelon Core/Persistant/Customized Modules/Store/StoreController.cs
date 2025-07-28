using System;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class StoreController : MonoBehaviour
    {
        private static StoreController instance;

        public StoreSettings storeData;

        private SaveData savedData;

        /// <summary>
        /// Store data file name
        /// </summary>
        public const string FILE_NAME = "StoreData.dat";

        /// <summary>
        /// Event called when new product has added
        /// </summary>
        public static StoreCallback onChange;

        private bool isRequiredSave = false;

        private void Awake()
        {
            instance = this;

            storeData.Init();

            Load();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!focus && isRequiredSave)
                Save();
        }

        private void OnDestroy()
        {
            Save();
        }

        public static int BoughtProductsAmount()
        {
            StoreProduct[] storeProducts = GetProducts();
            int boughtProducsAmount = 0;

            for (int i = 0; i < storeProducts.Length; i++)
            {
                if (storeProducts[i].IsOpened())
                    boughtProducsAmount++;
            }

            return boughtProducsAmount;
        }

        public static int GetProductsAmount()
        {
            return instance.storeData.products.Length;
        }

        /// <summary>
        /// Get products by type
        /// </summary>
        public static StoreProduct[] GetProducts()
        {
            return instance.storeData.products;
        }

        /// <summary>
        /// Get active products by type
        /// </summary>
        public static StoreProduct[] GetActiveProducts()
        {
            return Array.FindAll(instance.storeData.products, x => x.IsActive());
        }

        /// <summary>
        /// Get product by id
        /// </summary>
        public static T GetProduct<T>(int id) where T : StoreProduct
        {
            int productIndex = Array.FindIndex(instance.storeData.products, x => x.ID == id);

            if (productIndex != -1)
            {
                return (T)instance.storeData.products[productIndex];
            }

            return null;
        }

        /// <summary>
        /// Get product by id
        /// </summary>
        public static StoreProduct GetProduct(int id)
        {
            int productIndex = Array.FindIndex(instance.storeData.products, x => x.ID == id);

            if (productIndex != -1)
            {
                return instance.storeData.products[productIndex];
            }

            return null;
        }

        /// <summary>
        /// Buy product by id (if requirments are done)
        /// </summary>
        public static void BuyProduct(int id)
        {
            StoreProduct product = GetProduct(id);
            if (product != null)
            {
                //Check if requirments are done
                if (product.Check())
                {
                    //Call end method
                    product.Buy();

                    //Add id to saved data
                    instance.savedData.AddBoughtItem(id);

                    if (onChange != null)
                        onChange.Invoke(product);

                    //Save saved data to file
                    instance.Save();

#if SHOW_PROTOTYPE_LOGS
                Debug.Log("[Store]: Product unlocked - " + product.productName);
#endif
                }
            }
#if SHOW_PROTOTYPE_LOGS
        else
        {
            Debug.LogError("[Store]: Wrong product id!");
        }
#endif
        }

        /// <summary>
        /// Buy product (if requirments are done)
        /// </summary>
        public static void BuyProduct(StoreProduct product)
        {
            //Check if requirments are done
            if (product.Check())
            {
                //Call end method
                product.Buy();

                //Add id to saved data
                instance.savedData.AddBoughtItem(product.ID);

                if (onChange != null)
                    onChange.Invoke(product);

                //Save saved data to file
                instance.Save();

#if SHOW_PROTOTYPE_LOGS
            Debug.Log("[Store]: Product unlocked - " + product.productName);
#endif
            }
        }

        /// <summary>
        /// Buy product without checking requirments
        /// </summary>
        public static void OpenProduct(int id)
        {
            StoreProduct product = GetProduct(id);
            if (product != null)
            {
                if (instance.savedData.HasBoughtItem(id))
                    return;

                product.Buy();

                instance.savedData.AddBoughtItem(id);

                if (onChange != null)
                    onChange.Invoke(product);

                instance.Save();

#if SHOW_PROTOTYPE_LOGS
            Debug.Log("[Store]: Product unlocked - " + product.productName);
#endif
            }
#if SHOW_PROTOTYPE_LOGS
        else
        {
            Debug.LogError("[Store]: Wrong product id!");
        }
#endif
        }

        public static void PlaySkin(int id)
        {
            if (!instance.savedData.HasPlayedItem(id))
            {
                instance.savedData.AddPlayedItem(id);

                int playerSkins = instance.savedData.playerItems.Count;

                AchievementManager.IncrementProgress(15, 1); //Play with 5 different skins
                AchievementManager.IncrementProgress(16, 1); //Play with 10 different skins
                AchievementManager.IncrementProgress(17, 1); //Play with 15 different skins
            }
        }

        /// <summary>
        /// Check if product is purchased
        /// </summary>
        public static bool HasProduct(int id)
        {
            return instance.savedData.HasBoughtItem(id);
        }

        private void Save()
        {
#if SHOW_PROTOTYPE_LOGS
        Debug.Log("[Store]: Saving!");
#endif

            Serializer.SerializeToPDP(savedData, FILE_NAME);

            isRequiredSave = false;
        }

        private void Load()
        {
#if SHOW_PROTOTYPE_LOGS
        Debug.Log("[Store]: Loading!");
#endif

            if (Serializer.FileExistsAtPDP(FILE_NAME))
            {
                savedData = Serializer.DeserializeFromPDP<SaveData>(FILE_NAME);
            }
            else
            {
                savedData = new SaveData();
            }
        }

        /// <summary>
        /// Store saved data
        /// </summary>
        [System.Serializable]
        public class SaveData
        {
            public List<int> boughtItems = new List<int>();
            public List<int> playerItems = new List<int>();

            public bool HasBoughtItem(int id)
            {
                return boughtItems.FindIndex(x => x == id) != -1;
            }

            public void AddBoughtItem(int id)
            {
                if (boughtItems.FindIndex(x => x == id) == -1)
                    boughtItems.Add(id);
            }

            public bool HasPlayedItem(int id)
            {
                return playerItems.FindIndex(x => x == id) != -1;
            }

            public void AddPlayedItem(int id)
            {
                if (playerItems.FindIndex(x => x == id) == -1)
                    playerItems.Add(id);
            }
#if UNITY_EDITOR
            public void ClearItems()
            {
                boughtItems.Clear();
            }

            public void RemoveItem(int id)
            {
                int itemIndex = boughtItems.FindIndex(x => x == id);

                if (itemIndex != -1)
                    boughtItems.RemoveAt(itemIndex);
            }
#endif
        }

        public delegate void StoreCallback(StoreProduct product);
    }
}