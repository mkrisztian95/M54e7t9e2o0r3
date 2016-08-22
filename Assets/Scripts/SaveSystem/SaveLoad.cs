﻿using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad
{
    public static string fileName;
    public static string fileExtention;

    private static Game savedGame;

    public static void Save()
    {
        Game.current.timeStamp = DateTime.Now;
        savedGame = Game.current;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + fileName + "." + fileExtention);
        bf.Serialize(file, savedGame);
        file.Close();
        Debug.Log("OpenSavedGame from SaveLoad.Save()");
        GPGController.OpenSavedGame(Game.defaultName, OpenMode.Save);
    }

    public static bool Load()
    {
        bool localLoadSuccess = LoadLocal();
        //if (Game.current != null)
        Debug.Log("OpenSavedGame from SaveLoad.Load()");
        GPGController.OpenSavedGame(Game.defaultName, OpenMode.Load);
        //Game.current = Game.local;
        return localLoadSuccess;
    }

    public static bool LoadLocal()
    {
        if (File.Exists(Application.persistentDataPath + "/" + fileName + "." + fileExtention))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + fileName + "." + fileExtention, FileMode.Open);
            savedGame = (Game)bf.Deserialize(file);
            Game.local = savedGame;
            //if (GPGController.NoGPGMode)
            //{
            Game.current = savedGame;

            //}
            file.Close();
            return true;
        }
        else { return false; }
    }

    public static void LoadCloud(byte[] data, bool success)
    {
        if (success)
        {
            BinaryFormatter bf = new BinaryFormatter();
            File.WriteAllBytes(Application.persistentDataPath + "/" + fileName + "_cloud_backup." + fileExtention, data);
            FileStream file = File.Open(Application.persistentDataPath + "/" + fileName + "_cloud_backup." + fileExtention, FileMode.Open);
            savedGame = (Game)bf.Deserialize(file);
            Game.cloud = savedGame;
            file.Close();
            Debug.Log("Loading was successful");
        }
        else
        {
            Debug.Log("Loading failed");
        }
        //ChooseSavedGame();
        //GameObject.FindGameObjectWithTag("GameController").SendMessage("LoadStats");
    }

    public static void ChooseSavedGame()
    {
        if (Game.local == null)
        {
            Game.current = new Game();
            Game.local = Game.current;
        }
        if (Game.local != null && Game.cloud != null)
        {
            int i = DateTime.Compare(Game.local.timeStamp, Game.cloud.timeStamp);
            if (i > 0)
            {
                Game.current = Game.local;
                Debug.Log("i chose local");
            }
            else
            {
                Game.current = Game.cloud;
                Debug.Log("i chose cloud");
            }
        }
        else
        {
            Game.current = Game.local;
            Debug.Log("i chose local");
        }
        Save();
    }
}
