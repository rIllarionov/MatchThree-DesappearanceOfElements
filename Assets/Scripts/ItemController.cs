using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public class ItemController : MonoBehaviour
{
    [SerializeField] private float _moveDuration = 1f;
    [SerializeField] private IndexProvider _indexProvider;
    [SerializeField] private MapBuilder _mapBuilder;
    [SerializeField] private MatchController _matchController;

    private GameObject _firstElement;
    private GameObject _firstParent;
    private Vector2Int _firstElementCoordinates;

    private GameObject _secondElement;
    private GameObject _secondParent;
    private Vector2Int _secondElementCoordinates;

    private bool _hasFirstElement;
    private bool _hasSecondElement;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _hasFirstElement == false) //захват первого элемента
        {
            _hasFirstElement = SelectElement(out _firstElement, out _firstParent);
        }

        if (Input.GetMouseButtonUp(0) && _hasFirstElement) //захват второго элемента
        {
            _hasSecondElement = SelectElement(out _secondElement, out _secondParent);

            if (!_hasSecondElement) //если не оказалось элемента под мышью то сбрасываем так же и первый элемент
            {
                _hasFirstElement = false;
            }
        }

        if (_hasFirstElement && _hasSecondElement) //если оба элемента есть
            //и выполняется условие проверки, то меняем их местами
        {
            if (CanMove())
            {
                ChangeElements();
            }
            else
            {
                _hasFirstElement = false;
                _hasSecondElement = false;
            }
        }
    }

    private void ChangeElements()
    {
        //сразу сбросили наличие элементов

        _hasFirstElement = false;
        _hasSecondElement = false;

        //меняем местами элементы - просто двигаем, пока не меняем в массиве карты

        var firstElementStartPosition = _firstElement.transform.position;
        var secondElementStartPosition = _secondElement.transform.position;

        _firstElement.transform
            .DOMove(secondElementStartPosition, _moveDuration)
            .SetEase(Ease.OutQuint);

        _secondElement.transform
            .DOMove(firstElementStartPosition, _moveDuration)
            .SetEase(Ease.OutQuint);

        ReversElements();

        if (!_matchController.CheckMatches(_secondElementCoordinates.y,
                _secondElementCoordinates.x)) //если по результатам передвижения нет совпадений 3 в ряд то меняем назад
        {
            _firstElement.transform
                .DOMove(firstElementStartPosition, _moveDuration)
                .SetEase(Ease.OutQuint);

            _secondElement.transform
                .DOMove(secondElementStartPosition, _moveDuration)
                .SetEase(Ease.OutQuint);

            ReversElements();
        }

        else
        {
            ClearElements(_matchController.MatchCoordinates);
        }
    }

    private void ClearElements(List<Vector2Int> coordinates)
    {
        //добавляем в список первый элемент (он уже стоит а новой позиции)
        coordinates.Add(new Vector2Int(_firstElementCoordinates.y,_firstElementCoordinates.x));

        for (int i = 1; i < coordinates.Count; i++)
        {
            //удаляем первый элемент и все совпадения, первый элемент пропускаем.
            //там записаны старые координаты первого элемента
            
            var currentElement = _mapBuilder.TileMap[coordinates[i].x, coordinates[i].y].transform.GetChild(0);
            currentElement
                .DOScale(1.2f, _moveDuration / 4)
                .SetDelay(_moveDuration / 2)
                .OnComplete(() => currentElement
                    .DOScale(0, _moveDuration)
                    .OnComplete(() => currentElement.gameObject.SetActive(false)));
        }
    }

    private void ReversElements()
    {
        var secondParent = _secondParent;
        var firstParent = _firstParent;

        _firstParent = secondParent;
        _secondParent = firstParent;

        _firstElement.transform.SetParent(secondParent.transform);
        _secondElement.transform.SetParent(firstParent.transform);

        var secondElement = _mapBuilder.TileMap[_secondElementCoordinates.y, _secondElementCoordinates.x];
        var firstElement = _mapBuilder.TileMap[_firstElementCoordinates.y, _firstElementCoordinates.x];

        _mapBuilder.TileMap[_secondElementCoordinates.y, _secondElementCoordinates.x] = firstElement;
        _mapBuilder.TileMap[_firstElementCoordinates.y, _firstElementCoordinates.x] = secondElement;
    }

    private bool SelectElement(out GameObject element, out GameObject parent)
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.Raycast(mousePosition, Vector2.one);

        if (hit.collider != null)
        {
            element = hit.collider.gameObject;
            parent = element.transform.parent.gameObject;
            return true;
        }
        else
        {
            element = null;
            parent = null;
            return false;
        }
    }

    private bool CanMove() //проверяем что объекты рядом и что двигать планируем не диагонально
    {
        _firstElementCoordinates = _indexProvider.GetIndex(_firstParent.transform.localPosition);
        _secondElementCoordinates = _indexProvider.GetIndex(_secondParent.transform.localPosition);

        // если индекс x отличается на единицу а у не отличается или если у отличается на единицу а х нет, значит объекты рядом
        if (_firstElementCoordinates.x == _secondElementCoordinates.x &&
            Math.Abs(_firstElementCoordinates.y - _secondElementCoordinates.y) == 1)
        {
            return true;
        }

        else if (_firstElementCoordinates.y == _secondElementCoordinates.y &&
                 Math.Abs(_firstElementCoordinates.x - _secondElementCoordinates.x) == 1)
        {
            return true;
        }

        return false;
    }
}