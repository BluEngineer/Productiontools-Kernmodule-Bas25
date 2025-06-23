using TMPro;
using UnityEngine;
using System.Collections.Generic;

public static class EntityDropdownHelper
{
    public static void PopulateDropdown<T>(TMP_Dropdown dropdown, List<T> entityList)
        where T : EntityData
    {
        dropdown.ClearOptions();
        var options = new List<TMP_Dropdown.OptionData> {
            new TMP_Dropdown.OptionData("-- None --")
        };

        foreach (var entity in entityList)
        {
            options.Add(new TMP_Dropdown.OptionData(entity.DisplayName));
        }

        dropdown.AddOptions(options);
    }

    public static void SetDropdownValue<T>(TMP_Dropdown dropdown, string entityId, List<T> entityList)
        where T : EntityData
    {
        dropdown.value = 0;
        if (string.IsNullOrEmpty(entityId)) return;

        for (int i = 0; i < entityList.Count; i++)
        {
            if (entityList[i].ID == entityId)
            {
                dropdown.value = i + 1;
                return;
            }
        }
    }

    public static string GetSelectedID<T>(TMP_Dropdown dropdown, List<T> entityList)
        where T : EntityData
    {
        if (dropdown.value == 0) return null;
        int index = dropdown.value - 1;
        return (index < entityList.Count) ? entityList[index].ID : null;
    }
}