using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    public static Game game;
    public static GameData data;

    //AudioClips
    public AudioClip sound_fx_coin;

    //Player audio
    public AudioSource[] player_audio_sources;

    //Prefabs
    public GameObject prefab_coin;
    public GameObject prefab_wrench;



    private void Awake()
    {
        game = this;
        data = new GameData();
    }

    //Player Audio System
    int current_audio_source = -1;
    public void PlayNextSound(AudioClip c)
    {
        current_audio_source++;
        if (current_audio_source == player_audio_sources.Length)
            current_audio_source = 0;

        player_audio_sources[current_audio_source].clip = c;
        player_audio_sources[current_audio_source].Play();
    }

    //Data of current game
    public class GameData
    {
        public int coins = 0;
        public int life = 0;
        public int max_lifes = 3;
    }

    private void OnGUI()
    {
        string s = "Coins: " + data.coins;
        int size = (int) GUI.skin.label.CalcSize(new GUIContent(s)).x;
        GUI.Label(new Rect(Screen.width - size - 10, 10, size, 20), s);
    }

}
