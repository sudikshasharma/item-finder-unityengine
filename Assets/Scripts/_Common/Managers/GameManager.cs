using UnityEngine;

namespace ItemFinder.Managers
{
    public class GameManager : MonoBehaviour
    {
        #region --------------------- Singleton Class Object --------------------- 
        public static GameManager Instance;
        #endregion
        #region --------------------- Serialized Private Fields --------------------- 
        [SerializeField] private int RowSize;
        [SerializeField] private int ColSize;
        [SerializeField] private int totalClicks;
        [SerializeField] private int totalItems;
        #endregion
        #region --------------------- Monobehaviour Methods --------------------- 
        void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
        }
        #endregion
        #region --------------------- Public Methods --------------------- 
        public int GetRowCount()
        {
            return RowSize;
        }
        public int GetColCount()
        {
            return ColSize;
        }
        public int GetTotalClicks()
        {
            return totalClicks;
        }
        public int GetTotalItems()
        {
            return totalItems;
        }
        #endregion
    }
}
