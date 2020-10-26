using UnityEngine;
using ItemFinder.Views;
using ItemFinder.Managers;

namespace ItemFinder.Controllers
{
    public class GameController : MonoBehaviour
    {
        #region --------------------- Serialized Private Fields --------------------- 
        [SerializeField] private GameView gameView;
        #endregion
        #region --------------------- Public Properties --------------------- 
        public int ClicksLeft { get; set; }
        public int ItemsRevealed { get; set; }
        public int ItemsFound { get; set; }
        public int TotalItems { get; set; }
        public int RowCount { get; set; }
        public int ColCount { get; set; }
        #endregion
        #region --------------------- Monobehaviour Methods --------------------- 
        void Start()
        {
            Init();
            gameView.Init();
        }
        #endregion
        #region --------------------- Private Methods ---------------------  
        private void Init()
        {
            ItemsRevealed = 0;
            ItemsFound = 0;
            ClicksLeft = GameManager.Instance.GetTotalClicks();
            TotalItems = GameManager.Instance.GetTotalItems();
            RowCount = GameManager.Instance.GetRowCount();
            ColCount = GameManager.Instance.GetColCount();
        }
        #endregion
        #region --------------------- Public Methods --------------------- 
        public void GameOver()
        {
            gameView.GameOver();
        }
        public void PlayAgain()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        }
        #endregion
    }
}
