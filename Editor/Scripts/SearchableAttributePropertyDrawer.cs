using Paulsams.MicsUtil.FromRuntimeCreateScript;
using Paulsams.MicsUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
[CustomPropertyDrawer(typeof(SearchableAttribute))]
public class SearchableAttributePropertyDrawer : PropertyDrawer
{
    private const string _nameClass = "NamesConvertersToArrayString";
    private const string _localPathToDirectoryForUtilities = "Utilities/SearchableAttribute";

    private readonly SearchableDrawer _searchableAttributeDrawer = new SearchableDrawer();
    private readonly static Dictionary<string, IConvertToArrayString> _converters = new Dictionary<string, IConvertToArrayString>();

    static SearchableAttributePropertyDrawer()
    {
        CreateFile();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SearchableAttribute searchableAttribute = attribute as SearchableAttribute;

        var converter = searchableAttribute.ConverterTypeName == null ? null : _converters[searchableAttribute.ConverterTypeName];
        _searchableAttributeDrawer.Draw(position, property, label, new SearchableAttributeParameters(converter: converter));
    }

    private static void CreateFile()
    {
        var typesConverters = ReflectionUtilities.GetFinalAssignableTypesFromAllTypes(typeof(IConvertToArrayString));
        for (int i = 0; i < typesConverters.Count; ++i)
        {
            var type = typesConverters[i];
            _converters.Add(type.Name, Activator.CreateInstance(type) as IConvertToArrayString);
        }

        StringBuilder script = new StringBuilder();
        int tabIndex = 0;

        script.AppendLine("using System;");

        script.AppendLine($"public static class {_nameClass}", tabIndex);
        script.AppendOpeningBrace(ref tabIndex);
        {
            foreach (var typeConverter in typesConverters)
            {
                script.AppendLine($"public const string {typeConverter.Name.Replace("ToArrayString", "")} = \"{typeConverter.Name}\";", tabIndex);
            }
        }
        script.AppendBreakingBrace(ref tabIndex);

        script.AppendLine();

        string pathToDirectory = $"{Application.dataPath}/{_localPathToDirectoryForUtilities}";

        string pathToRuntimeFolder = $"{pathToDirectory}/Runtime";
        if (Directory.Exists(pathToRuntimeFolder) == false)
            Directory.CreateDirectory(pathToRuntimeFolder);

        string pathToEditorFolder = $"{pathToDirectory}/Editor";

        string pathToCustomConverters = $"{pathToEditorFolder}/Custom Converters";
        if (Directory.Exists(pathToCustomConverters) == false)
            Directory.CreateDirectory(pathToCustomConverters);

        File.WriteAllText($"{pathToRuntimeFolder}/{_nameClass}.cs", script.ToString());
    }
}