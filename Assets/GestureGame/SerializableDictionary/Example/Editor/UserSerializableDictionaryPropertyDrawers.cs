using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(StringStringDictionary))]
[CustomPropertyDrawer(typeof(ObjectColorDictionary))]
[CustomPropertyDrawer(typeof(DifficultySymbolsLevelDictionary))]
[CustomPropertyDrawer(typeof(DifficultyMemoryLevelDictionary))]
[CustomPropertyDrawer(typeof(AreaStatusColorDictionary))]
[CustomPropertyDrawer(typeof(AreaStatusSpriteDictionary))]
[CustomPropertyDrawer(typeof(StringImageDictionary))]


public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer {}
