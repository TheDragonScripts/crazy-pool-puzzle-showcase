using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class ActualPlayerData
{
    private static FileStream _currentStream;
    private const string DataFileName = "save.dat";
    public static PlayerData Data { get; private set; }

    public static void SaveGame()
    {
        if (Data == null)
        {
            throw new NullReferenceException("Player data is corrupted and can not be saved");
        }
        string path = Application.persistentDataPath + "/" + DataFileName;
        _currentStream = File.OpenWrite(path);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(_currentStream, Data);
        _currentStream.Close();
    }

    public static void LoadGame()
    {
        Data = new PlayerData();
        string path = Application.persistentDataPath + "/" + DataFileName;

        if (File.Exists(path))
        {
            _currentStream = File.OpenRead(path);
            BinaryFormatter formatter = new BinaryFormatter();
            PlayerData deserializedData = formatter.Deserialize(_currentStream) as PlayerData;
            int[] usedBallsOnLevels = SetupLevelsArray(deserializedData.UsedBallsOnLevel, Data.UsedBallsOnLevel);
            deserializedData.UsedBallsOnLevel = usedBallsOnLevels;
            Data = deserializedData;
            _currentStream.Close();
        }
    }

    private static int[] SetupLevelsArray(int[] oldArr, int[] rawArray)
    {
        if (oldArr.Length == rawArray.Length)
        {
            return oldArr;
        }
        CSDL.Log($"Levels array incompatibility detected. Working on it. " +
            $"Old array size is {oldArr.Length}. " +
            $"New array size is {rawArray.Length}");
        int[] newArr = TransferArray(oldArr, rawArray.Length);
        return newArr;
    }

    private static T[] TransferArray<T>(T[] arrayToTransfer, int newSize)
    {
        T[] result = new T[newSize];
        for (int i = 0; i < arrayToTransfer.Length; i++)
        {
            if (i >= newSize)
            {
                break;
            }
            result[i] = arrayToTransfer[i];
        }
        return result;
    }
}
