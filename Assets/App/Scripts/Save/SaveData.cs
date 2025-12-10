using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Données de sauvegarde - Ajoutez vos données ici
/// </summary>
[Serializable]
public class SaveData
{
    public SaveDataStruct SaveDataInfo = new SaveDataStruct();
}

public struct SaveDataStruct
{
    public string LastSaveTime;
}
