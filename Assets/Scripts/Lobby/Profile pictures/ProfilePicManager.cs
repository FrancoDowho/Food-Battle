using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ProfilePicManager : NetworkBehaviour
{
    public static ProfilePicManager instance;
    public string[] allPossibleNames;
    public Sprite[] allPossibleImages;
    Dictionary<PlayerControll, ProfilePicMark> _playersPortraitsMarks = new();
    public List<ProfilePicMark> allProfilePicsMarks = new List<ProfilePicMark>();
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ChangeAvatar()
    {
        //int randomAvatarIndex = Random.Range(0, allPossibleImages.Length);
        //BasicSpawner.instance.localPlayer.playerpfpIndex = randomAvatarIndex;

        BasicSpawner.instance.localPlayer.RequestRandomAvatar();
    }

    public void ChangeName()
    {
        //int RandomName = Random.Range(0, allPossibleNames.Length);
        //BasicSpawner.instance.localPlayer.playerNameIndex = RandomName;

        BasicSpawner.instance.localPlayer.RequestRandomName();
    }

    public void UpdateUI()
    {
        UpdatePortraitsDictionary(BasicSpawner.instance.playersControlls, allProfilePicsMarks);

        foreach (var kvp in _playersPortraitsMarks)
        {
            ProfilePicMark mark = kvp.Value;
            PlayerControll player = kvp.Key;
            mark.txtname.text = allPossibleNames[player.playerNameIndex].ToString();
            mark.pfp.sprite = allPossibleImages[player.playerpfpIndex];

        }
    }
    public void ConnectedToLobby()
    {
        UpdatePortraitsDictionary(BasicSpawner.instance.playersControlls, allProfilePicsMarks);
        UpdateUI();

    }

    public void UpdatePortraitsDictionary(List<PlayerControll> players, List<ProfilePicMark> profilesMarks)
    {
        _playersPortraitsMarks = new();

        if (BasicSpawner.instance == null)
            return;

        if (BasicSpawner.instance.localPlayer == null)
            return;

        if (players == null || profilesMarks == null || profilesMarks.Count == 0)
            return;

        _playersPortraitsMarks.Add(BasicSpawner.instance.localPlayer, profilesMarks[0]);

        int count = 1;

        foreach (var player in players)
        {
            if (player == null || player == BasicSpawner.instance.localPlayer)
                continue;

            if (count >= profilesMarks.Count)
                break;

            _playersPortraitsMarks.Add(player, profilesMarks[count]);
            count++;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
