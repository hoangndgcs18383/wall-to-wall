using System;
using UnityEngine;
using System.Collections;
using GoogleSheetsToUnity;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// example script to show realtime updates of multiple items
/// </summary>
public class AnimalManager : MonoBehaviour
{
    public enum SheetStatus
    {
        PUBLIC,
        PRIVATE
    }

    public SheetStatus sheetStatus;

    public string associatedSheet = "1GVXeyWCz0tCjyqE1GWJoayj92rx4a_hu4nQbYmW_PkE";
    public string associatedWorksheet = "Stats";

    public List<AnimalObject> animalObjects = new List<AnimalObject>();
    public AnimalContainer container;


    public bool updateOnPlay;

    void Awake()
    {
        if (updateOnPlay)
        {
            UpdateStats();
        }
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 30), "Update Stats"))
        {
            UpdateStats();
        }

        if (GUI.Button(new Rect(10, 50, 100, 30), "Post Stats"))
        {
            PostStats();
        }
    }

    void PostStats()
    {
        if (sheetStatus == SheetStatus.PRIVATE)
        {
            int row = 1;

            string[] cells = new string[]
            {
                "A1",
                "A2",
                "A3",
                "A4"
            };

            foreach (Animal animal in container.allAnimals)
            {
                List<string> list = new List<string>
                {
                    animal.name,
                    animal.health.ToString(),
                    animal.attack.ToString(),
                    animal.defence.ToString()
                };

                SpreadsheetManager.Write(
                    new GSTU_Search(associatedSheet, associatedWorksheet, cells[row++]),
                    new ValueRange(list),
                    null);
            }
        }
    }

    void UpdateStats()
    {
        if (sheetStatus == SheetStatus.PRIVATE)
        {
            SpreadsheetManager.Read(new GSTU_Search(associatedSheet, associatedWorksheet), UpdateAllAnimals);
        }
        else if (sheetStatus == SheetStatus.PUBLIC)
        {
            SpreadsheetManager.ReadPublicSpreadsheet(new GSTU_Search(associatedSheet, associatedWorksheet),
                UpdateAllAnimals);
        }
    }

    void UpdateAllAnimals(GstuSpreadSheet ss)
    {
        foreach (Animal animal in container.allAnimals)
        {
            animal.UpdateStats(ss);
        }

        foreach (AnimalObject animalObject in animalObjects)
        {
            animalObject.BuildAnimalInfo();
        }
    }
}