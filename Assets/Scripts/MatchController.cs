using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector2 = System.Numerics.Vector2;

public class MatchController : MonoBehaviour
{
    [SerializeField] private ScriptableItemsHolder _itemsHolder;
    [SerializeField] private MapBuilder _mapBuilder;
    [SerializeField] private IndexProvider _indexProvider;

    public List <Vector2Int> MatchCoordinates => _matchCoordinates;
    private ScriptableItemSettings[] _itemsSettings;
    private List <Vector2Int> _matchCoordinates;


    private void Awake()
    {
        _itemsSettings = _itemsHolder.Items;
    }

    public ScriptableItemSettings GetRandomElement(bool isItemGood, int rowIndex, int columnIndex)
    {
        //предпологаем что элемент нам сразу подходит В процессе меняем флаг на фолс если элемент не подходит
        //генерируем рандомный элемент
        var randomElement = _itemsSettings[Random.Range(0, _itemsSettings.Length)];

        //если слева есть более двух элементов то проверяем эти два элемента
        if (columnIndex > 1 && isItemGood)
        {
            var currentType = randomElement.Type;
            var firstElement = _mapBuilder.TileMap[rowIndex, columnIndex - 1].Type;
            var secondElement = _mapBuilder.TileMap[rowIndex, columnIndex - 2].Type;

            if (firstElement == secondElement) //если предыдущие два элемента равны, то сравниваем с текущим
            {
                if (currentType == firstElement)
                {
                    isItemGood = false; //если они все три равны то помечаем элемент как неподходящий
                }
            }
        }

        //далее сравниваем по столбцу (если по строкам не нашли совпадений)
        if (rowIndex > 1 && isItemGood)
        {
            var currentType = randomElement.Type;
            var firstElement = _mapBuilder.TileMap[rowIndex - 1, columnIndex].Type;
            var secondElement = _mapBuilder.TileMap[rowIndex - 2, columnIndex].Type;

            if (firstElement == secondElement)
            {
                if (currentType == firstElement)
                {
                    isItemGood = false;
                }
            }
        }

        //если по результатам проверки рандомный элемент не совпал с предыдущими двумя элементами
        //по горизнтали и по вертикали, то он нам подходит
        if (isItemGood)
        {
            return randomElement;
        }

        //иначе запускаем новую итерацию метода
        else
        {
            return GetRandomElement(true, rowIndex, columnIndex);
        }
    }

    public bool CheckMatches(int y, int x)
    {
        return CheckColumn(y, x) || CheckRow(y, x);
    }

    private bool CheckColumn(int y, int x)
    {
        _matchCoordinates = new List<Vector2Int>();
        
        _matchCoordinates.Add(new Vector2Int(y, x));

        var typeOfElement = _mapBuilder.TileMap[y, x].Type;

        //проверяем совпадения справа
        for (int i = y + 1; i < _indexProvider.ColumnCount; i++)
        {
            var typeOfCurrentElement = Convert.ToInt32(_mapBuilder.TileMap[i, x].Type);

            if (typeOfElement == typeOfCurrentElement)
            {
                _matchCoordinates.Add(new Vector2Int(i, x));
            }

            else
            {
                break;
            }
        }

        //проверяем слева
        for (int i = y - 1; i >= 0; i--)
        {
            var typeOfCurrentElement = Convert.ToInt32(_mapBuilder.TileMap[i, x].Type);

            if (typeOfElement == typeOfCurrentElement)
            {
                _matchCoordinates.Add(new Vector2Int(i, x));
            }

            else
            {
                break;
            }
        }

        return _matchCoordinates.Count > 2;
    }

    private bool CheckRow(int y, int x)
    {
        var matchesCount = 1; //единицей помечаем первое совпадение - это текущий элемент
        
        _matchCoordinates = new List<Vector2Int>();
        
        _matchCoordinates.Add(new Vector2Int(y, x));

        var typeOfElement = _mapBuilder.TileMap[y, x].Type;

        //проверяем совпадения сверху
        for (int i = x + 1; i < _indexProvider.RowCount; i++)
        {
            var typeOfCurrentElement = _mapBuilder.TileMap[y, i].Type;

            if (typeOfElement == typeOfCurrentElement)
            {
                _matchCoordinates.Add(new Vector2Int(y, i));
            }

            else
            {
                break;
            }
        }

        //проверяем вниз
        for (int i = x - 1; i >= 0; i--)
        {
            var typeOfCurrentElement = _mapBuilder.TileMap[y, i].Type;

            if (typeOfElement == typeOfCurrentElement)
            {
                _matchCoordinates.Add(new Vector2Int(y, i));
            }

            else
            {
                break;
            }
        }

        return _matchCoordinates.Count > 2;
    }
}