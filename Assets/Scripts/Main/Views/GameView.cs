using UnityEngine;
using UnityEngine.UI;
using ItemFinder.Models;
using System.Collections;
using ItemFinder.Controllers;
using System.Collections.Generic;

namespace ItemFinder.Views
{
    public class GameView : MonoBehaviour
    {
        #region --------------------- Serialized Private Fields --------------------- 
        [Header("Controllers")]
        [SerializeField] private GameController gameController;
        [Header("GameObjects")]
        [SerializeField] private GameObject gamePanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject tile;
        [SerializeField] private GameObject item;
        [Header("Transforms")]
        [SerializeField] private Transform tileParent;
        [SerializeField] private Transform itemParent;
        [Header("UI")]
        [SerializeField] private Button replayBtn;
        [SerializeField] private Text itemLocationText;
        [SerializeField] private Text itemsFoundText;
        [SerializeField] private Text clicksLeftText;
        #endregion
        #region --------------------- Private Fields --------------------- 
        private TileModel[] tiles;
        private List<int> itemLocations;
        private RaycastHit hit;
        #endregion
        #region --------------------- Monobehaviour Methods --------------------- 
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var isHit = Physics.Raycast(ray, out hit);
                if (isHit)
                {
                    if (itemLocations.Contains(hit.collider.gameObject.GetComponent<TileModel>().currentPositionId) && !hit.collider.gameObject.GetComponent<TileModel>().isItemShown)
                    {
                        Debug.Log("Item found at tile :" + hit.collider.gameObject.name);
                        hit.collider.gameObject.GetComponent<TileModel>().isItemShown = true;
                        hit.collider.gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TileMatLit");
                        ShowItem(hit.collider.gameObject.transform, itemLocations.IndexOf(hit.collider.gameObject.GetComponent<TileModel>().currentPositionId));
                        ++gameController.ItemsRevealed;
                        ++gameController.ItemsFound;
                    }
                    else
                    {
                        CheckNearbyItemsI(hit.collider.gameObject);
                        // CheckNearbyItemsII(hit.collider.gameObject);
                    }
                    --gameController.ClicksLeft;
                    CheckGameOver();
                    // Debug.Log(hit.collider.gameObject.name);
                    // Debug.DrawRay(Camera.main.ScreenToWorldPoint(Input.mousePosition), hit.collider.gameObject.transform.position, Color.red);
                }
            }
        }
        #endregion
        #region --------------------- Private Methods --------------------- 
        private void GenerateItemLocations()
        {
            itemLocations = new List<int>();
            for (int i = 0; i < gameController.TotalItems; i++)
            {
                var itemLocation = Random.Range(0, (gameController.RowCount * gameController.ColCount));
                if (!itemLocations.Contains(itemLocation))
                {
                    itemLocations.Add(itemLocation);
                    itemLocationText.text += itemLocation.ToString() + " , ";
                    continue;
                }
                i -= 1;
            }
        }
        ////////////////////Using OverapBox (collider) approach - Method I
        private void CheckNearbyItemsI(GameObject tile)
        {
            Debug.Log("Missed Item Tile");
            Collider[] hitColliders = Physics.OverlapBox(tile.transform.position, tile.transform.localScale, tile.transform.rotation);
            int i = 0;
            while (i < hitColliders.Length)
            {
                Debug.Log("Tile in Range : " + hitColliders[i].name);
                if (itemLocations.Contains(hitColliders[i].gameObject.GetComponent<TileModel>().currentPositionId) && (!hitColliders[i].gameObject.GetComponent<TileModel>().isItemShown))
                {
                    hitColliders[i].gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TileMatLit");
                    hitColliders[i].gameObject.GetComponent<TileModel>().isItemShown = true;
                    ++gameController.ItemsRevealed;
                    Debug.Log("Item in nearby tile : " + hitColliders[i].name);
                    Debug.Log("Total Revealed Items: " + gameController.ItemsRevealed);
                }
                i++;
            }
        }
        ////////////////////Using TileId compare approach - Method II
        private void CheckNearbyItemsII(GameObject tile)
        {
            Debug.Log("Missed Item Tile");
            var tileId = tile.GetComponent<TileModel>().currentPositionId;
            var i = tileId / gameController.RowCount;
            var j = tileId % gameController.ColCount;
            Debug.Log("i:" + i + " j:" + j);
            if (i - 1 >= 0)
            {
                CheckTileForItem(i - 1, j);
                if (j + 1 < gameController.ColCount)
                    CheckTileForItem(i - 1, j + 1);
                if (j - 1 >= 0)
                    CheckTileForItem(i - 1, j - 1);
            }
            if (i + 1 < gameController.ColCount)
            {
                CheckTileForItem(i + 1, j);
                if (j + 1 < gameController.ColCount)
                {
                    CheckTileForItem(i + 1, j + 1);
                    CheckTileForItem(i, j + 1);
                }
                if (j - 1 >= 0)
                {
                    CheckTileForItem(i + 1, j - 1);
                    CheckTileForItem(i, j - 1);
                }
            }
        }
        private void CheckTileForItem(int i, int j)
        {
            Debug.Log("Checking for tile i:" + i + " j:" + j);
            var tileId = i * gameController.RowCount + j;
            Debug.Log("TileId:" + tileId);
            if (itemLocations.Contains(tileId) && !(tiles[tileId].gameObject.GetComponent<TileModel>().isItemShown))
            {
                tiles[tileId].gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TileMatLit");
                tiles[tileId].gameObject.GetComponent<TileModel>().isItemShown = true;
                ++gameController.ItemsRevealed;
                Debug.Log("Item in nearby tile : " + tiles[tileId].name);
                Debug.Log("Total Revealed Items: " + gameController.ItemsRevealed);
            }
        }
        private void CheckGameOver()
        {
            Debug.Log("Checking Game Over");
            if (gameController.ClicksLeft == 0 || gameController.ItemsRevealed == gameController.TotalItems)
            {
                Debug.Log("Game Over , Clicks Left: " + gameController.ClicksLeft + " Items Revealed: " + gameController.ItemsRevealed + " TotalItems: " + gameController.TotalItems);
                gameController.GameOver();
            }
        }
        private void ShowItem(Transform itemLocation, int itemNumber)
        {
            var itemRevealed = Instantiate(item, itemLocation.position, itemLocation.rotation);
            itemRevealed.transform.GetComponentInChildren<Text>().text += itemNumber.ToString();
            StartCoroutine(PositionItem(itemRevealed, new Vector3(itemRevealed.transform.position.x, itemRevealed.transform.position.y, -8f)));
            itemRevealed.transform.SetParent(itemParent.transform);
        }
        private IEnumerator PositionItem(GameObject itemRevealed, Vector3 finalPos)
        {
            var t = 0f;
            while (t < 3f)
            {
                itemRevealed.transform.position = Vector3.Lerp(itemRevealed.transform.position, finalPos, t);
                t += 0.5f * Time.deltaTime;
                yield return null;
            }
        }
        #endregion
        #region --------------------- Public Methods --------------------- 
        public void Init()
        {
            gameOverPanel.SetActive(false);
            replayBtn.onClick.AddListener(gameController.PlayAgain);
            GenerateGrid();
        }
        public void GenerateGrid()
        {
            tiles = new TileModel[gameController.RowCount * gameController.ColCount];
            GenerateItemLocations();
            var tileSize = tile.GetComponent<MeshRenderer>().bounds.size.x;
            for (int i = 0; i < gameController.RowCount; i++)
            {
                for (int j = 0; j < gameController.ColCount; j++)
                {
                    var tileId = i * gameController.RowCount + j;
                    var tileObj = Instantiate(tile);
                    tileObj.name = "Tile" + tileId;
                    tileObj.transform.position = new Vector3(j * tileSize, i * tileSize, -10);
                    tileObj.transform.SetParent(tileParent);
                    tileObj.GetComponent<TileModel>().currentPositionId = tileId;
                    tiles[tileId] = tileObj.GetComponent<TileModel>();
                }
            }
        }
        public void GameOver()
        {
            gameOverPanel.SetActive(true);
            gamePanel.SetActive(false);
            clicksLeftText.text += gameController.ClicksLeft.ToString();
            itemsFoundText.text += gameController.ItemsFound.ToString();
        }
        #endregion
    }
}