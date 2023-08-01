using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
#endif

namespace Assets._Project.SaveLoad
{
    public class Storage
    {
        private BinaryFormatter _binaryFormatter;
        private string _path;
        private string _saveData;
#if UNITY_ANDROID
        private bool _isSaving;
#endif
        private Action<string> _loadingFinishedCallback;

        public Storage()
        {
            _binaryFormatter = new BinaryFormatter();
            _path = Application.persistentDataPath + "/";
        }

        public void SaveCloud(string fileName, object data)
        {
            _saveData = JsonUtility.ToJson(data);
#if UNITY_ANDROID
            _isSaving = true;
#endif
            Debug.Log($"Save system | Trying to save this : {_saveData}");
#if UNITY_ANDROID
            OpenFile(fileName);
#endif
        }

        public void Save(string fileName, object data)
        {
            string dataJSON = JsonUtility.ToJson(data);
            string path = _path + fileName;
            Debug.Log($"Save system | Trying to save this : {dataJSON}");
            FileStream fileStream = File.Create(path);
            _binaryFormatter.Serialize(fileStream, dataJSON);
            fileStream.Close();
        }

        public void Load(string fileName, object defaultData, Action<string> loadingFinishedCallback)
        {
            string defaultDataJSON = JsonUtility.ToJson(defaultData);
            string path = _path + fileName;

            if (File.Exists(path))
            {
                FileStream fileStream = File.Open(path, FileMode.Open);
                string deserializedData = (string)_binaryFormatter.Deserialize(fileStream);
                loadingFinishedCallback?.Invoke(deserializedData);
                fileStream.Close();
            }
            else
            {
                loadingFinishedCallback?.Invoke(defaultDataJSON);
            }
        }

        public void LoadCloud(string fileName, object defaultData, Action<string> loadingFinishedCallback)
        {
            _saveData = JsonUtility.ToJson(defaultData);
            _loadingFinishedCallback = loadingFinishedCallback;
#if UNITY_ANDROID
            _isSaving = false;
#endif
            Debug.Log($"Save system | Trying to load this : {_saveData}");

            if (Social.localUser.authenticated == false)
            {
                loadingFinishedCallback?.Invoke(_saveData);
                return;
            }

#if UNITY_ANDROID
            OpenFile(fileName);
#endif
        }

#if UNITY_ANDROID
        private void OpenFile(string fileName)
        {
            PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution
            (
                fileName,
                DataSource.ReadNetworkOnly,
                ConflictResolutionStrategy.UseLongestPlaytime,
                OnFileOpened
            );
        }

        private void OnFileOpened(SavedGameRequestStatus status, ISavedGameMetadata metadata)
        {
            Debug.Log($"Save system | Status : {status}");

            if (status == SavedGameRequestStatus.Success)
            {
                if (_isSaving)
                {
                    string updatedData = JsonUtility.ToJson(_saveData);
                    byte[] updatedBinaryData = Encoding.UTF8.GetBytes(updatedData);
                    SavedGameMetadataUpdate updatedMetadata = new SavedGameMetadataUpdate.Builder()
                        .WithUpdatedDescription($"Saved at {DateTime.Now}").Build();

                    PlayGamesPlatform.Instance.SavedGame.CommitUpdate(metadata, updatedMetadata, updatedBinaryData, OnComitUpdated);
                }
                else
                {
                    PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(metadata, OnReadCompleted);
                }
            }
        }

        private void OnComitUpdated(SavedGameRequestStatus status, ISavedGameMetadata metadata)
        {
            if (status == SavedGameRequestStatus.Success)
            {
                Debug.Log("Save system | Successfully commited update");
            }
            else
            {
                Debug.LogWarning("Save system | Failed to commit update");
            }
        }

        private void OnReadCompleted(SavedGameRequestStatus status, byte[] data)
        {
            if (status == SavedGameRequestStatus.Success)
            {
                string loadedData = Encoding.UTF8.GetString(data);
                Debug.Log($"Save system | Loaded this : {loadedData}");
                _loadingFinishedCallback?.Invoke(loadedData);
            }
            else
            {
                Debug.LogWarning("Save system | Failed to read file");
                _loadingFinishedCallback?.Invoke(_saveData);
            }
        }
#endif
    }
}