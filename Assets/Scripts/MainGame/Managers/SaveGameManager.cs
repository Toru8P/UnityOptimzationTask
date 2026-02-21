using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace MainGame.Managers
{
    public class SaveGameManager : MonoBehaviour
    {
        private const string SAVE_FILE_NAME = "/Save.dat";
        [SerializeField] private GameManager gameManager;
        [SerializeField] private UIManager uiManager;

        private SerializedSaveGame _serializedSaveGame;

        [ContextMenu("Save!")]
        public void SaveGame()
        {
            _serializedSaveGame = new SerializedSaveGame();
            // serializedSaveGame.playerPosition = gameManager.playerCharacterController.transform.position;
            // serializedSaveGame.playerRotation = gameManager.playerCharacterController.transform.eulerAngles;
            _serializedSaveGame.playerPositionX = gameManager.playerCharacterController.transform.position.x;
            _serializedSaveGame.playerPositionY = gameManager.playerCharacterController.transform.position.y;
            _serializedSaveGame.playerPositionZ = gameManager.playerCharacterController.transform.position.z;
        
            _serializedSaveGame.playerRotationX = gameManager.playerCharacterController.transform.eulerAngles.x;
            _serializedSaveGame.playerRotationY = gameManager.playerCharacterController.transform.eulerAngles.y;
            _serializedSaveGame.playerRotationZ = gameManager.playerCharacterController.transform.eulerAngles.z;
        
            _serializedSaveGame.playerHPNew = gameManager.playerCharacterController.Hp;
            _serializedSaveGame.currentWaypointIndex = gameManager.playerCharacterController.CurrentWaypointIndex;
        
            //  SaveToJson();
            SaveToBinary();
        }

        [ContextMenu("Load!")]
        public void LoadGame()
        {
            // LoadFromJson();

            LoadFromBinary();
        
            // gameManager.playerCharacterController.transform.position = serializedSaveGame.playerPosition;
            // gameManager.playerCharacterController.transform.eulerAngles = serializedSaveGame.playerRotation;
            gameManager.playerCharacterController.transform.position = new Vector3(_serializedSaveGame.playerPositionX,
                _serializedSaveGame.playerPositionY, _serializedSaveGame.playerPositionZ);
            gameManager.playerCharacterController.transform.eulerAngles = new Vector3(_serializedSaveGame.playerRotationX,
                _serializedSaveGame.playerRotationY, _serializedSaveGame.playerRotationZ);
            gameManager.playerCharacterController.Hp = _serializedSaveGame.playerHPNew;
        
            gameManager.playerCharacterController.CurrentWaypointIndex = _serializedSaveGame.currentWaypointIndex;
            //
            uiManager.RefreshHPText(gameManager.playerCharacterController.Hp);
            gameManager.playerCharacterController.SetDestination(gameManager.playerCharacterController.CurrentWaypointIndex);
        }

        private void SaveToJson()
        {
            string jsonString = JsonUtility.ToJson(_serializedSaveGame, true);

            File.WriteAllText(Application.persistentDataPath + SAVE_FILE_NAME, jsonString);
        }

        private void LoadFromJson()
        {
            string jsonString = File.ReadAllText(Application.persistentDataPath + SAVE_FILE_NAME);
            _serializedSaveGame = JsonUtility.FromJson<SerializedSaveGame>(jsonString);
        }

        private void SaveToBinary()
        {
            FileStream fileStream = new FileStream(Application.persistentDataPath
                                                   + SAVE_FILE_NAME, FileMode.Create);
            BinaryFormatter converter = new BinaryFormatter();
            converter.Serialize(fileStream, _serializedSaveGame);
            fileStream.Close();
        }

        private void LoadFromBinary()
        {
            if (File.Exists(Application.persistentDataPath + SAVE_FILE_NAME))
            {
                FileStream fileStream = new FileStream(Application.persistentDataPath + SAVE_FILE_NAME, FileMode.Open);

                BinaryFormatter converter = new BinaryFormatter();
                _serializedSaveGame = converter.Deserialize(fileStream) as SerializedSaveGame;

                fileStream.Close();
            }
        }
    }
}