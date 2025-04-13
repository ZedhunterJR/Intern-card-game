using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : Singleton<DataManager>
{
    private PlayerData.Player playerData => PlayerData.Instance.playerData;

    private void Start()
    {
        LoadPlayerData();
    }


    private void LoadPlayerData()
    {
        if (DDOLoad.Instance.gameInit == true) return;

        DDOLoad.Instance.gameInit = true;

        Debug.Log("Loaded");

        if (PlayerPrefs.HasKey("IsFirstTime"))
        {
            PlayerData.Instance.LoadPlayerData();
        }
        else
        {
            // Lần đầu chạy game -> Khởi tạo dữ liệu mặc định rồi lưu lại
            PlayerPrefs.SetInt("IsFirstTime", 1); // Đánh dấu đã khởi tạo dữ liệu
            PlayerPrefs.Save();

            PlayerData.Instance.BaseData();
            PlayerData.Instance.SavePlayerData();

        }
    }
}

[System.Serializable]
public class PlayerData
{
    private static PlayerData instance;

    public static PlayerData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PlayerData();
            }
            return instance;
        }
    }

    public class Player
    {
        public int gold;
        public int tileLevel;
        public int maxTileBuilding;
        public int heartLevel;
        public int maxHeart;
        public int bonusAttackLevel;
        public float bonusAttack;
        public int bonusAttackSpeedLevel;
        public float bonusAttackSpeed;
    }

    public Player playerData;
    public readonly string PlayerDataKey = "PlayerData";

    public void SavePlayerData()
    {

        string json = JsonUtility.ToJson(playerData);
        PlayerPrefs.SetString(PlayerDataKey, json);
    }

    public void LoadPlayerData()
    {
        string json = PlayerPrefs.GetString(PlayerDataKey);
        playerData = JsonUtility.FromJson<Player>(json);
        Debug.Log(playerData == null);
    }

    public void BaseData() // Reset Data Default, Dont use 
    {
        playerData = new Player()
        {
            gold = 0,
            tileLevel = 1,
            maxTileBuilding = 6,
            heartLevel = 1,
            maxHeart = 3,
            bonusAttackLevel = 1,
            bonusAttack = 0,
            bonusAttackSpeedLevel = 1,
            bonusAttackSpeed = 0,
        };
    }
}
