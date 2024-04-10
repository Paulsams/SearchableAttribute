using Paulsams.MicsUtils.CodeGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paulsams.SearchableAttributeDrawer.Editor
{
    [InitializeOnLoad]
    [CustomPropertyDrawer(typeof(SearchableAttribute))]
    public class SearchableAttributePropertyDrawer : PropertyDrawer
    {
        private const string _nameClass = "NamesConvertersToArrayString";
        private const string _localPathToDirectoryForUtilities = "Utilities/SearchableAttribute";

        private static readonly Dictionary<string, IConvertToArrayString> _converters =
            new Dictionary<string, IConvertToArrayString>();

        static SearchableAttributePropertyDrawer()
        {
            CreateFile();
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property) =>
            SearchableDrawer.UIToolkit.Create(property, property.displayName, CreateParameters());

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) =>
            SearchableDrawer.OnGUI.Draw(position, property, label, CreateParameters());

        private SearchableAttributeParameters CreateParameters()
        {
            SearchableAttribute searchableAttribute = attribute as SearchableAttribute;

            var converter = searchableAttribute.ConverterTypeName == null
                ? null
                : _converters[searchableAttribute.ConverterTypeName];

            return new SearchableAttributeParameters(
                converter: converter,
                windowType: searchableAttribute.WindowType
            );
        }

        private static void CreateFile()
        {
            string GetScriptForAssemblyReference(string nameAssembly) =>
                $"{{\n \"reference\": \"{nameAssembly}\"\n}}";

            const string nameAssemblyForRuntime = "Paulsams.SearchableAttribute";
            const string nameAssemblyForEditor = "Paulsams.SearchableAttribute.Editor";

            var typesConverters =
                TypeCache.GetTypesDerivedFrom<IConvertToArrayString>(nameAssemblyForEditor);
            foreach (var type in typesConverters.Where(type => type.IsAbstract == false && type.IsInterface == false))
                _converters.Add(type.Name, Activator.CreateInstance(type) as IConvertToArrayString);

            StringBuilder script = new StringBuilder();
            int tabIndex = 0;

            script.AppendLine("using System;");

            script.AppendLine($"public static class {_nameClass}", tabIndex);
            script.AppendOpeningBrace(ref tabIndex);
            {
                foreach (var typeConverter in typesConverters)
                {
                    script.AppendLine(
                        $"public const string {typeConverter.Name.Replace("ToArrayString", "")} = \"{typeConverter.Name}\";",
                        tabIndex);
                }
            }
            script.AppendBreakingBrace(ref tabIndex);

            string pathToDirectory = $"{Application.dataPath}/{_localPathToDirectoryForUtilities}";

            string pathToRuntimeFolder = $"{pathToDirectory}/Runtime";
            if (Directory.Exists(pathToRuntimeFolder) == false)
                Directory.CreateDirectory(pathToRuntimeFolder);

            string pathToEditorFolder = $"{pathToDirectory}/Editor";

            string pathToCustomConverters = $"{pathToEditorFolder}/Custom Converters";
            if (Directory.Exists(pathToCustomConverters) == false)
                Directory.CreateDirectory(pathToCustomConverters);

            string localPathToAsmrefForRuntime = $"{nameAssemblyForRuntime}.asmref";
            string fullPathToAsmrefForRuntime = $"{pathToRuntimeFolder}/{localPathToAsmrefForRuntime}";
            if (File.Exists(fullPathToAsmrefForRuntime) == false)
                File.WriteAllText(fullPathToAsmrefForRuntime, GetScriptForAssemblyReference(nameAssemblyForRuntime));

            string localPathToAsmrefForEditor = $"{nameAssemblyForEditor}.asmref";
            string fullPathToAsmrefForEditor = $"{pathToEditorFolder}/{localPathToAsmrefForEditor}";
            if (File.Exists(fullPathToAsmrefForEditor) == false)
                File.WriteAllText(fullPathToAsmrefForEditor, GetScriptForAssemblyReference(nameAssemblyForEditor));

            var pathToNamesClass = $"{pathToRuntimeFolder}/{_nameClass}.cs";
            using (var streamNamesClass = new FileStream(pathToNamesClass, FileMode.OpenOrCreate))
            {
                byte[] readBuffer = new byte[streamNamesClass.Length];
                byte[] writeBuffer = Encoding.Default.GetBytes(script.ToString());

                int readCount = 0;
                do
                    readCount += streamNamesClass.Read(readBuffer, readCount,
                        (int)(streamNamesClass.Length - readCount));
                while (readCount != readBuffer.Length);

                if (readBuffer.SequenceEqual(writeBuffer) == false)
                {
                    streamNamesClass.SetLength(0);
                    streamNamesClass.Write(writeBuffer);
                    AssetDatabase.ImportAsset("Assets" + pathToDirectory.Substring(Application.dataPath.Length));
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}