using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocationLoad : MonoBehaviour
{


    private static PlayerLocationLoad instance;
    public static PlayerLocationLoad Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Instantiate(Resources.Load<PlayerLocationLoad>("PlayerLocationSave"));
                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.Log("dest!");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }



    // Saved Data
    private Vector3 Position;
    private int Direction;

    [SerializeField]
    private GameObject player;


    void Start()
    {
        player = GameObject.Find("Wizard");
        SaveLocation(FindObjectOfType<PlayerBehaviourStateMachine>());
        Debug.Log("Saved");
    }



    public static void SaveLocation(PlayerBehaviourStateMachine player)
    {
        instance.Position = player.transform.position;
        instance.Direction = player.direction;
    }

    public static void LoadLocation(PlayerBehaviourStateMachine player)
    {
        player.transform.position = instance.Position;
        player.direction = instance.Direction;
    }


    public void SavePlayerPosition()
    {

        PlayerPrefs.SetFloat("playerPosX", player.transform.position.x);
        PlayerPrefs.SetFloat("playerPosY", player.transform.position.y);
        PlayerPrefs.SetFloat("playerPosZ", player.transform.position.z);
        PlayerPrefs.Save();

    }

    public void LoadPlayerPosition()
    {
        float playerPosX = PlayerPrefs.GetFloat("playerPosX");
        float playerPosY = PlayerPrefs.GetFloat("playerPosY");
        float playerPosZ = PlayerPrefs.GetFloat("playerPosZ");

        player.transform.position = new Vector3(playerPosX,playerPosY,playerPosZ);
    }

}
