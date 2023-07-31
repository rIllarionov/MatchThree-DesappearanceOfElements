using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapBuilder : MonoBehaviour
{
    [SerializeField] private RectTransform _map;
    [SerializeField] private IndexProvider _indexProvider;
    [SerializeField] private MatchController _matchController;

    [SerializeField] private int _countItemsRow;
    [SerializeField] private int _countItemsColumn;

    [SerializeField] private Tile _tilePrefab;
    public Tile[,] TileMap => _tileMap;
    private Tile[,] _tileMap;
    private void Awake()
    {
        _indexProvider.Initialize(_countItemsColumn, _countItemsRow);
        InitializeMap();
        SetItems();
    }

    private void InitializeMap()
    {
        _tileMap = new Tile [_countItemsRow, _countItemsColumn];

        for (int i = 0; i < _countItemsRow; i++)
        {
            for (int j = 0; j < _countItemsColumn; j++)
            {
                _tileMap[i, j] = Instantiate(_tilePrefab, _map, false);
                _tileMap[i, j].transform.localPosition = _indexProvider.GetCoordinates(j, i);
            }
        }
    }

    private void SetItems()
    {
        for (int i = 0; i < _countItemsRow; i++)
        {
            for (int j = 0; j < _countItemsColumn; j++)
            {
                //запускаем поиск рандомного элемента и когда находим инсталим его в текущую клетку
                _tileMap[i, j].Initialize(_matchController.GetRandomElement(true, i, j));
            }
        }
    }
}