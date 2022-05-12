# SearchableAttribute
Этот пакет позволяет Вам использовать атрибут [Searchable], который реализует функционал всплывающего окна вместе с строкой поиска, которая позволяет быстро найти нужный элемент.

## Добавление в проект
Чтобы добавить данный пакет в проект, нужно выполнить следующие шаги:
1) Откройте PackageManager;
2) Выберите "Add package from get URL";
3) Вставьте ссылки на пакеты, которые являются зависимостями данного пакета:
    + `https://github.com/Paulsams/MiscUtilities.git`
3) Вставьте ссылку на данный пакет: `https://github.com/Paulsams/SearchableAttribute.git`

## Зависимости
- Использует:
    + MicsUtilities: https://github.com/Paulsams/MiscUtilities.git
	
## Возможности
1) Работает с перечислениями безо всякой дополнительной настройки.
```cs
[SerializeField, Searchable] private KeyCode _keyCode;
```

![image](https://github.com/Paulsams/SearchableAttribute/blob/master/Documentation~/Enum%20Example.gif)

2) Позволяет писать свои кастомные преобразования в список строк из которых можно выбирать и присваивать через SerializeProperty значение полю.

Подробнее:
при импорте данного пакета у Вас должны будут создаться папки по пути: `Utilities/SearchableAttribute`. Файл "NamesConvertersToArrayString" (`Runtime/NamesConvertersToArrayString.cs`) нужен для того, чтобы не хардкодить названия классов при написании их аргументом в атрибут. Папка `Editor/Custom Converters` может служить просто папкой для создания ваших кастомных конвертеров, но это не запрещает Вам создавать их в любом месте в проекте. Пример конверетера есть ниже. Имя своему кастоному конвертеру лучше давать с суффиксом `ToArrayString`, потому что на стороне моего кода именно это словосочетание обрывается.

```cs
[SerializeField, Searchable(NamesConvertersToArrayString.ItemTypes)] private string _itemType;
```

```cs
using System.Linq;
using UnityEditor;
using Paulsams.MicsUtils;
using UnityEngine;

public class ItemTypesToArrayString : IConvertToArrayString
{
    private readonly IConvertToArrayString.Element[] _items;

    public ItemTypesToArrayString()
    {
        string pathToJson = $"{Application.dataPath}/Samples/SearchableAttribute/Example Enum and One Custom Converter/Runtime/ItemTypes/ItemsTypes.json";

        if (JsonSerializerUtility.TryDeserialize(out ItemType[] itemTypes, pathToJson))
        {
            _items = itemTypes.Select((itemType) => new IConvertToArrayString.Element(itemType.Name, itemType.Category)).ToArray();
        }
        else
        {
            Debug.LogError($"The file with item types was not found on the path: {pathToJson}");
        }
    }

    public IConvertToArrayString.Element[] Convert(SerializedProperty property) => _items;

    public int GetIndex(SerializedProperty property) => System.Array.FindIndex(_items, (element) => element.Name == property.stringValue);

    public void SetIndex(SerializedProperty property, int index) => property.stringValue = _items[index].Name;
}
```

![image](https://github.com/Paulsams/SearchableAttribute/blob/master/Documentation~/Enum%20Example.gif)

## Конструкторы
```cs
SearchableAttribute(string converterTypeName = null)
```

## Примеры
Чтобы скачать примеры к данному пакету:
1) Выберите данный пакет в PackageManager;
2) Раскройте справа вкладку "Samples";
3) И нажмите кнопку "Import" на интересующем вас примере.
