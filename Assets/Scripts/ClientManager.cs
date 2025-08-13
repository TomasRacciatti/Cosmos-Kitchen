using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    public static ClientManager instance;
    [SerializeField] GameObject _camera;
    [SerializeField] GameObject _clientOrderPrefab;
    [SerializeField] SpawnpointSetter[] _clientSpawnpoint;
    [SerializeField] Transform _clientOrderList;

    [Header("Eorth Client Prefab")]
    [SerializeField] GameObject[] _clientPrefab;

    [SerializeField] bool _isCriticEorthUnlocked;
    //[SerializeField] bool _isCriticDesertUnlocked;
    //[SerializeField] bool _isCriticOceanUnlocked;
    //[SerializeField] bool _isCriticFrozenUnlocked;
    //[SerializeField] bool _isCriticVolcanoUnlocked;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        } else { Destroy(this); }

            int arraysize;

        if (_isCriticEorthUnlocked)
        {
            arraysize = _clientSpawnpoint.Length;
        }
        else
        {
            arraysize = _clientSpawnpoint.Length-1;
        }

        for (int i = 0; i < arraysize; i++)
        {
            ClientScript client = Instantiate(_clientPrefab[i], _clientSpawnpoint[i].transform).GetComponent<ClientScript>();
            client.SetAnimation(_clientSpawnpoint[i]._initialAnimationState);
        }
    }

    public ClientOrder MakeClientOrder(string name, string description)
    {
        GameObject _newclientorder = Instantiate(_clientOrderPrefab);
        ClientOrder _newclientscript = _newclientorder.GetComponent<ClientOrder>();
        _newclientscript.SetOrder(description);
        _newclientscript.SetClient(name);
        _newclientorder.transform.SetParent(_clientOrderList);
        _newclientorder.transform.localScale = Vector3.one;

        return _newclientscript;
    }

    public void SetSpawnpoint(SpawnpointSetter spawnpoint)
    {
        _clientSpawnpoint[spawnpoint._clientNumber] = spawnpoint;
    }

    public void DeleteSpawnpoint()
    {
        _clientSpawnpoint = null;
    }

    public void UnlockCritic(string planetname)
    {

        switch (planetname)
        {
            case "Eorth":
                _isCriticEorthUnlocked = true;
                ClientScript client = Instantiate(_clientPrefab[6], _clientSpawnpoint[6].transform).GetComponent<ClientScript>();
                client.SetAnimation(_clientSpawnpoint[6]._initialAnimationState);
                break;
            case "Desert":
                //_isCriticDesertUnlocked = true;
                break;
            case "Volcano":
                //_isCriticVolcanoUnlocked = true;
                break;
            case "Frozen":
                //_isCriticFrozenUnlocked = true;
                break;
            case "Ocean":
                //_isCriticOceanUnlocked = true;
                break;
            default:
                break;
        }
    }
}
