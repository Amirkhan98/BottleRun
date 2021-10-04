using UnityEngine;

namespace Amir.Level
{
    public class LevelManager : MonoBehaviour
    {
        public LevelProgression levelProgression;
        public GameObject currentLevel;
        public GameObject currentPlayer;

        private void Start()
        {
            //LoadLevelFromResources(); //For first time debug
        }

        /// <summary>
        /// Loading level from Resources/Levels/ by current Level ID.
        /// Level will be instanced in zero position world coordinates.
        /// 
        /// Loading player from Resources/PlayerController/.
        /// Player will be instanced in zero position world coordinates.
        /// </summary>
        public void LoadLevelFromResources()
        {
            if (currentLevel != null) Destroy(currentLevel);
            if (currentPlayer != null) Destroy(currentPlayer);

            GameObject _instance;
            _instance = Instantiate(Resources.Load("Levels/Level_" + StaticManager.GetLevelID(), typeof(GameObject))) as GameObject;
            ResetTransform(_instance.transform);
            currentLevel = _instance;

            LevelSettings levelSettings = _instance.GetComponent<LevelSettings>();

            GameObject playerInstance;
            playerInstance = Instantiate(Resources.Load("Player/PlayerController", typeof(GameObject))) as GameObject;
            ResetTransform(playerInstance.transform);
            currentPlayer = playerInstance;

            levelProgression.startPoint = levelSettings.startLevelPoint;
            levelProgression.endPoint = levelSettings.endLevelPoint;
            levelProgression.playerTransform = playerInstance.transform;
            levelProgression.SetDistance();
        }

        /// <summary>
        /// Reset transform position, scale and rotation to zero.
        /// </summary>
        /// <param name="_transform">Transform for reset</param>
        private void ResetTransform(Transform _transform)
        {
            _transform.position = Vector3.zero;
            _transform.localScale = Vector3.one;
            _transform.rotation = Quaternion.identity;
        }

        /// <summary>
        /// Called after started play level.
        /// </summary>
        internal void StartLevel()
        {
           //Make what you need when game/level started
        }
    }
}