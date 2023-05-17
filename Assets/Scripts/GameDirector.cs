using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Fusion;

public class GameDirector : NetworkBehaviour
{
    public static GameDirector instance = null;
    public NetworkRunner runner;

    public AudioClip music;

    private void Awake()
    {
        instance = this;
        runner = GameObject.Find("NetworkRunner").GetComponent<NetworkRunner>();
    }

    Color redTeam = new Color(255 / 255f, 165 / 255f, 165 / 255f);
    Color blueTeam = new Color(165 / 255f, 165 / 255f, 255 / 255f);

    [Header("Chatting")]
    public GameObject ScrollView;
    public GameObject ChatInput;
    public bool chatInputFlag;
    public float chatTime;
    

    [SerializeField] NetworkPrefabRef roomInfo;
    [SerializeField] NetworkPrefabRef Lampadati; // ½ºÆ÷Ã÷Ä«
    [SerializeField] NetworkPrefabRef Benefacter; // ¹ê
    [SerializeField] NetworkPrefabRef Alvani; // ½Â¿ëÂ÷
    [SerializeField] NetworkPrefabRef Pegasi; // ½´ÆÛÄ«

    [SerializeField] NetworkPrefabRef Howitzer; // °î»çÆ÷
    [SerializeField] NetworkPrefabRef Vulcan; // ¹ßÄ­
    [SerializeField] NetworkPrefabRef Shoker; // ¼îÄ¿
    [SerializeField] NetworkPrefabRef Canon; // Ä³³í

    [Header("DefaultData")]
    public Camera MainCamera;
    public GameObject RoomInfoObj;
    int nowPlayer;
    int adminChange;
    public GameObject mainLight;
    public GameObject mainCamera;
    public GameObject MiniMapCamera;
    public GameObject infoBox;
    public GameObject SpawnBox;
    public GameObject PlayerObjList;

    public GameObject ExitPanel;
    public GameObject BrokenPanel;
    public GameObject LoadingPanel;
    public int loadPlayer = 0;
    public bool loadComplate = false;

    [Header("PlayerInfo")]
    GameObject[] PlayerCarList;
    GameObject[] PlayerGunList;
    public NetworkObject myCar;
    public NetworkObject myGun;
    public NetworkObject myInfoDes;
    public GameObject myInfoObj;

    void Start()
    {
        // ¹æ ¼³Á¤
        RoomInfoObj = GameObject.Find("RoomInfo(Clone)");

        // ÇÃ·¹ÀÌ¾î »ý¼º
        infoBox = GameObject.FindGameObjectWithTag("InfoBox");
        Transform[] infoList = infoBox.GetComponentsInChildren<Transform>();
        for (int i = 1; i < infoList.Length; i++)
        {
            if (infoList[i].GetComponent<PlayerInfo>().id.Equals(ENB.id))
            {
                myInfoDes = infoList[i].gameObject.GetComponent<NetworkObject>();
                myInfoObj = infoList[i].gameObject;
                CarSpawn(myInfoObj.GetComponent<PlayerInfo>().car, myInfoObj.GetComponent<PlayerInfo>().roomNum);
                MiniMapCamera.GetComponent<SmoothFollow>().target = myCar.gameObject.transform;
                GunSpawn(myInfoObj.GetComponent<PlayerInfo>().gun, myInfoObj.GetComponent<PlayerInfo>().roomNum);
            }
        }

        int mapTime = RoomInfoObj.GetComponent<RoomInfo>().mapTime;
        if (mapTime == 1) mainLight.transform.rotation = Quaternion.Euler(25, 0, 0);
        else if (mapTime == 2) mainLight.transform.rotation = Quaternion.Euler(45, 0, 0);
        else if (mapTime == 3) mainLight.transform.rotation = Quaternion.Euler(90, 0, 0);
        else if (mapTime == 4) mainLight.transform.rotation = Quaternion.Euler(135, 0, 0);
        else if (mapTime == 5) mainLight.transform.rotation = Quaternion.Euler(155, 0, 0);
    }

    void Update()
    {
        if (loadComplate == false)
        {
            Invoke("loadingPlayer", 0.3f);
        }

        if (RoomInfoObj == null) RoomInfoObj = GameObject.FindGameObjectWithTag("RoomInfo");

        if (RoomInfoObj.GetComponent<RoomInfo>().ingameExitAdmin == true) Destroy(RoomInfoObj);

        if (chatTime >= 0.01)
        {
            chatTime -= Time.deltaTime;
            ScrollView.SetActive(true);
        }
        else
        {
            if (chatInputFlag == false)
            {
                chatTime = 0;
                ScrollView.SetActive(false);
            }
            else chatTime = 5;
        }
    }

    public override void FixedUpdateNetwork()
    {

    }

    void loadingPlayer()
    {
        int playerCount;
        playerCount = GameObject.FindGameObjectsWithTag("PlayerCar").Length;
        if (playerCount == RoomInfoObj.GetComponent<RoomInfo>().playerCount)
        {
            SetPlayerObject(playerCount);
            Invoke("PlayerCarId", 0.3f);
            LoadingPanel.SetActive(false);
            loadComplate = true;
        }
    }

    void CarSpawn(string name, int spNum)
    {
        GameObject SpawnPoint;
        Transform[] spList = SpawnBox.GetComponentsInChildren<Transform>();
        SpawnPoint = spList[spNum + 1].gameObject;

        if (name.Equals("¶÷ÆÄ´ÙÆ¼ Ä«½ºÄÚ"))
            myCar = runner.Spawn(Lampadati, SpawnPoint.transform.position, SpawnPoint.transform.rotation, ENB.myNO.GetComponent<PlayerInfo>().pid);
        else if (name.Equals("º£³×ÆÑÅÍ ´þ½ºÅ¸"))
            myCar = runner.Spawn(Benefacter, SpawnPoint.transform.position, SpawnPoint.transform.rotation, ENB.myNO.GetComponent<PlayerInfo>().pid);
        else if (name.Equals("¾Ë¹Ù´Ï ÇÁ¸®¸ð"))
            myCar = runner.Spawn(Alvani, SpawnPoint.transform.position, SpawnPoint.transform.rotation, ENB.myNO.GetComponent<PlayerInfo>().pid);
        else if (name.Equals("Æä°¡½Ã Á¨Åä¸£³ë"))
            myCar = runner.Spawn(Pegasi, SpawnPoint.transform.position, SpawnPoint.transform.rotation, ENB.myNO.GetComponent<PlayerInfo>().pid);

        myCar.gameObject.GetComponent<GamePlayerInfo>().createId = ENB.id;
        myCar.gameObject.GetComponent<GamePlayerInfo>().spNum = spNum + 1;

        MainCamera.GetComponent<SmoothFollow>().target = myCar.transform;
    }

    void GunSpawn(string name, int spNum)
    {
        GameObject gunSpawnPoint = null;
        Transform[] spList = myCar.GetComponentsInChildren<Transform>();
        for (int i = 0; i < spList.Length; i++) if (spList[i].name.Equals("GunSpawn"))
            {
                gunSpawnPoint = spList[i].gameObject;
                break;
            }

        if (name.Equals("°î»çÆ÷"))
            myGun = runner.Spawn(Howitzer, gunSpawnPoint.transform.position, gunSpawnPoint.transform.rotation, ENB.myNO.GetComponent<PlayerInfo>().pid);
        else if (name.Equals("¹ßÄ­"))
            myGun = runner.Spawn(Vulcan, gunSpawnPoint.transform.position, gunSpawnPoint.transform.rotation, ENB.myNO.GetComponent<PlayerInfo>().pid);
        else if (name.Equals("¼îÄ¿"))
            myGun = runner.Spawn(Shoker, gunSpawnPoint.transform.position, gunSpawnPoint.transform.rotation, ENB.myNO.GetComponent<PlayerInfo>().pid);
        else if (name.Equals("Ä³³í"))
            myGun = runner.Spawn(Canon, gunSpawnPoint.transform.position, gunSpawnPoint.transform.rotation, ENB.myNO.GetComponent<PlayerInfo>().pid);

        myGun.gameObject.GetComponent<GamePlayerInfo>().createId = ENB.id;
        myGun.gameObject.GetComponent<GamePlayerInfo>().spNum = spNum + 1;
    }

    void SetPlayerObject(int count)
    {
        string carCreateId;
        string gunCreateId;
        PlayerCarList = GameObject.FindGameObjectsWithTag("PlayerCar");
        PlayerGunList = GameObject.FindGameObjectsWithTag("PlayerGun");
        Transform[] spList = SpawnBox.GetComponentsInChildren<Transform>();

        for (int i = 0; i < count; i++)
        {
            carCreateId = PlayerCarList[i].GetComponent<GamePlayerInfo>().createId;

            for (int j = 0; j < count; j++)
            {
                gunCreateId = PlayerGunList[j].GetComponent<GamePlayerInfo>().createId;

                if (carCreateId.Equals(gunCreateId))
                {
                    Transform[] CarChildList = PlayerCarList[i].GetComponentsInChildren<Transform>();
                    GameObject gunSpawnPoint = null;
                    for (int k = 0; k < CarChildList.Length; k++) if (CarChildList[k].name.Equals("GunSpawn"))
                        {
                            gunSpawnPoint = CarChildList[k].gameObject;
                            break;
                        }

                    PlayerCarList[i].transform.parent = PlayerObjList.transform;
                    PlayerGunList[j].transform.localScale = new Vector3(1, 1, 1);
                    PlayerGunList[j].transform.position = gunSpawnPoint.transform.position;
                    PlayerGunList[j].transform.parent = gunSpawnPoint.transform;
                }
            }
        }
    }
    void PlayerCarId()
    {
        infoBox = GameObject.FindGameObjectWithTag("InfoBox");
        Transform[] infoList = infoBox.GetComponentsInChildren<Transform>();
        PlayerCarList = GameObject.FindGameObjectsWithTag("PlayerCar");

        for (int i = 0; i < PlayerCarList.Length; i++)
        {
            string id = PlayerCarList[i].GetComponent<GamePlayerInfo>().createId;
            string team = "";

            for (int j = 1; j < infoList.Length; j++)
            {
                if (infoList[j].GetComponent<PlayerInfo>().id.Equals(id))
                {
                    team = infoList[j].GetComponent<PlayerInfo>().team;
                    break;
                }
            }
            GameObject myCarID = PlayerCarList[i].GetComponentsInChildren<Transform>()[6].gameObject;
            myCarID.GetComponent<TextMesh>().text = id;

            if (team.Equals("r")) myCarID.GetComponent<TextMesh>().color = redTeam;
            else if (team.Equals("b")) myCarID.GetComponent<TextMesh>().color = blueTeam;
            else myCarID.GetComponent<TextMesh>().color = Color.white;
        }
    }

    public void enableSuspension()
    {
        int WC;
        Transform[] tmp = myCar.GetComponentsInChildren<Transform>();
        Transform[] carTmp = tmp[1].GetComponentsInChildren<Transform>();
        for (WC = 0; WC < carTmp.Length; WC++)
            if (carTmp[WC].name.Equals("WC"))
                break;
        WC++;

        JointSpring js = new JointSpring();
        js.spring = 350;
        js.damper = 4500;
        js.targetPosition = 0.5f;
        for (int i = 1; i < carTmp.Length; i++)
        {
            carTmp[i].gameObject.GetComponent<WheelCollider>().suspensionDistance = 0.3f;
            carTmp[i].gameObject.GetComponent<WheelCollider>().suspensionSpring = js;
        }
    }
    public void ExitOkBtn() => myObjDest();
    public void ExitCancleBtn()
    {
        ExitPanel.SetActive(false);
    }

    void roomInfoSync(NetworkObject obj)
    {
        RoomInfoObj.GetComponent<RoomInfo>().playerCount = nowPlayer;
        RoomInfoObj.GetComponent<RoomInfo>().map = MyRoomInfo.Instance.map;
        RoomInfoObj.GetComponent<RoomInfo>().mapTime = MyRoomInfo.Instance.mapTime;
        RoomInfoObj.GetComponent<RoomInfo>().s0 = MyRoomInfo.Instance.s0;
        RoomInfoObj.GetComponent<RoomInfo>().s1 = MyRoomInfo.Instance.s1;
        RoomInfoObj.GetComponent<RoomInfo>().s2 = MyRoomInfo.Instance.s2;
        RoomInfoObj.GetComponent<RoomInfo>().s3 = MyRoomInfo.Instance.s3;
        RoomInfoObj.GetComponent<RoomInfo>().s4 = MyRoomInfo.Instance.s4;
        RoomInfoObj.GetComponent<RoomInfo>().s5 = MyRoomInfo.Instance.s5;
        RoomInfoObj.GetComponent<RoomInfo>().s6 = MyRoomInfo.Instance.s6;
        RoomInfoObj.GetComponent<RoomInfo>().s7 = MyRoomInfo.Instance.s7;

        RoomInfoObj.GetComponent<RoomInfo>().WorldMinute = MyRoomInfo.Instance.WorldMinute;
        RoomInfoObj.GetComponent<RoomInfo>().WorldSecond = MyRoomInfo.Instance.WorldSecond;
        RoomInfoObj.GetComponent<RoomInfo>().WorldMilli = MyRoomInfo.Instance.WorldMilli;
        RoomInfoObj.GetComponent<RoomInfo>().EndRace = MyRoomInfo.Instance.EndRace;
        RoomInfoObj.GetComponent<RoomInfo>().EndTime = MyRoomInfo.Instance.EndTime;

        RoomInfoObj.GetComponent<RoomInfo>().Top1 = MyRoomInfo.Instance.Top1;
        RoomInfoObj.GetComponent<RoomInfo>().Top2 = MyRoomInfo.Instance.Top2;
        RoomInfoObj.GetComponent<RoomInfo>().Top3 = MyRoomInfo.Instance.Top3;
        RoomInfoObj.GetComponent<RoomInfo>().Top4 = MyRoomInfo.Instance.Top4;
        RoomInfoObj.GetComponent<RoomInfo>().Top5 = MyRoomInfo.Instance.Top5;
        RoomInfoObj.GetComponent<RoomInfo>().Top6 = MyRoomInfo.Instance.Top6;
        RoomInfoObj.GetComponent<RoomInfo>().Top7 = MyRoomInfo.Instance.Top7;
        RoomInfoObj.GetComponent<RoomInfo>().Top8 = MyRoomInfo.Instance.Top8;
    }
    public void IG_leftPlayer() => playerLeft();
    public void playerLeft()
    {
        Transform[] localPlayerList = infoBox.GetComponentsInChildren<Transform>();
        nowPlayer = localPlayerList.Length - 1;
        adminChange = 1;

        if (runner.IsSharedModeMasterClient)
        {
            if (!RoomInfoObj.GetComponent<RoomInfo>().roomAdmin.Equals(ENB.id))
            {
                if (adminChange == 1)
                {
                    Destroy(RoomInfoObj);

                    createRoomObj();
                    adminChange = 0;
                }
            }
            else adminChange = 0;
        }

        GameObject[] RoomInfoObjects = GameObject.FindGameObjectsWithTag("RoomInfo");
        for (int i = 0; i < RoomInfoObjects.Length; i++)
        {
            if (RoomInfoObjects[i].GetComponent<NetworkObject>().StateAuthority.IsNone) Destroy(RoomInfoObjects[i].gameObject);
            if (RoomInfoObjects[i].GetComponent<NetworkObject>().StateAuthority != RoomInfoObjects[i].GetComponent<NetworkObject>().InputAuthority) Destroy(RoomInfoObjects[i].gameObject);
        }

        for (int i = 1; i < localPlayerList.Length; i++)
        {
            if (localPlayerList[i].GetComponent<NetworkObject>().StateAuthority.IsNone) Destroy(localPlayerList[i].gameObject);
            if (localPlayerList[i].GetComponent<NetworkObject>().StateAuthority != localPlayerList[i].GetComponent<NetworkObject>().InputAuthority) Destroy(localPlayerList[i].gameObject);
        }

        GameObject[] localPlayerCarList = GameObject.FindGameObjectsWithTag("PlayerCar");
        for (int i = 0; i < localPlayerCarList.Length; i++)
        {
            if (localPlayerCarList[i].GetComponent<NetworkObject>().StateAuthority.IsNone) Destroy(localPlayerCarList[i].gameObject);
            if (localPlayerCarList[i].GetComponent<NetworkObject>().StateAuthority != localPlayerCarList[i].GetComponent<NetworkObject>().InputAuthority) Destroy(localPlayerCarList[i].gameObject);
        }
    }

    void createRoomObj()
    {
        NetworkObject obj = runner.Spawn(roomInfo, gameObject.transform.position, Quaternion.identity, ENB.myNO.GetComponent<PlayerInfo>().pid);
        obj.GetComponent<RoomInfo>().roomAdmin = ENB.id;
        obj.GetComponent<RoomInfo>().ingameExitAdmin = false;
        roomInfoSync(obj);
        DontDestroyOnLoad(obj);
    }

    void myObjDest()
    {
        if (runner.IsSharedModeMasterClient) RoomInfoObj.GetComponent<RoomInfo>().ingameExitAdmin = true;

        Destroy(infoBox);
        Destroy(RoomInfoObj);

        runner.Despawn(myCar);
        runner.Despawn(myGun);
        runner.Despawn(myInfoDes);

        ENB.gamePlay = false;

        GameManager.instance.U_InviReset();
        ENB.gameinfo.room = "x";

        runner.Shutdown();

        GameManager.instance.U_LRChange("x");
        SceneManager.LoadScene("Loading");
    }
}