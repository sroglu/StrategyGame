using mehmetsrl.MVC.core;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    //Singleton
    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }
        private set
        {
            if (_instance == null)
                _instance = value;
            else
                Destroy(value);
        }
    }

    #region Properties
    public enum gameState { gameboard }
    //GamePages
    [SerializeField]
    Dictionary<gameState, IController> pages;
    public IController CurrentPage { get { return pages[State]; } }
    #endregion

    #region EditorBindings
    [SerializeField]
    GameData gameData;
    [SerializeField]
    gameState state = gameState.gameboard;
    #endregion

    #region Accesors
    public gameState State { get { return state; } private set { state = value; } }
    public GameData GameData { get { return gameData; } }
    #endregion


    private void Awake()
    {
        Instance = this;
        pages = new Dictionary<gameState, IController>();
        State=gameState.gameboard;
    }

    void Start()
    {
        InitPages();
        pages[State].GetView().Show();
    }

    private void InitPages()
    {
        pages.Add(gameState.gameboard, new GamePageController(new GamePageModel(gameData)));
    }
}
