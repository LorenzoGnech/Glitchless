using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace ProSave.Scripts {
    public class SaveHandle<T> where T : class {
        private readonly string _name;

        public SaveHandle(string name) {
            _name = name;
        }

        public T LoadData() {
            var filePath = Application.persistentDataPath + "/" + _name + ".pro";
            if (!File.Exists(filePath)) return null;

            var binaryFormatter = new BinaryFormatter();
            var fileStream = new FileStream(filePath, FileMode.Open);
            var data = binaryFormatter.Deserialize(fileStream) as T;
            fileStream.Close();
            return data;
        }

        public void SaveData(T value) {
            var binaryFormatter = new BinaryFormatter();
            var filePath = Application.persistentDataPath + "/" + _name + ".pro";
            var fileStream = new FileStream(filePath, FileMode.Create);
            binaryFormatter.Serialize(fileStream, value);
            fileStream.Close();
        }
    }
}