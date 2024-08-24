using System;
using System.IO;
using UnityEngine;
using Utils;

namespace Management.Disk
{
    public class DiskMemoryManager : Singleton<DiskMemoryManager>
    {
        public bool ReadFile(string filename, out string result)
        {
            var path = Path.Combine(Application.persistentDataPath, filename);
            try
            {
                result = File.ReadAllText(path);
                return true;
            }
            catch (Exception)
            {
                result = "";
                return false;
            }
        }

        public bool WriteFile(string filename, string content)
        {
            var path = Path.Combine(Application.persistentDataPath, filename);

            try
            {
                File.WriteAllText(path, content);
                return true;
            }
            catch (Exception)
            {
                Debug.Log($"Failed to write to file {filename}");
                return false;
            }
        }
    }
}