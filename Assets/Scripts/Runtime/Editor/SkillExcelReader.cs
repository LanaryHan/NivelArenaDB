using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using EnumsNET;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Runtime.Business.Data;
using Runtime.Business.Data.Entry;
using UnityEditor;
using UnityEngine;

namespace Runtime.Editor
{
    public class SkillExcelReader : EditorWindow
    {
        [MenuItem("Tools/读取技能Excel")]

        public static void ShowWindow()
        {
            GetWindow<SkillExcelReader>("Excel读取工具");
        }

        void OnGUI()
        {
            if (GUILayout.Button("开始Excel"))
            {
                ReadExcel();
            }
        }

        private void ReadExcel()
        {
            var excelPath = Path.Combine(Application.dataPath,"Resources/Nivel Arena DB Skills.xlsx");
            if (!File.Exists(excelPath))
            {
                EditorUtility.DisplayDialog("错误", "文件不存在", "确定");
                return;
            }

            using var fs = new FileStream(excelPath, FileMode.Open, FileAccess.Read);
            var xssfWorkbook = new XSSFWorkbook(fs);
            var records = new List<SkillEntry>();
            var sheetCount = xssfWorkbook.NumberOfSheets;
            //0为Config
            for (int i = 1; i < sheetCount; i++)
            {
                var sheet = xssfWorkbook.GetSheetAt(i);
                var sheetName = sheet.SheetName;
                if (sheet.PhysicalNumberOfRows == 0)
                {
                    continue;
                }

                var headRow = sheet.GetRow(0);
                if (headRow == null)
                {
                    continue;
                }

                for (int rowIdx = 1; rowIdx <= sheet.LastRowNum; rowIdx++)
                {
                    var row = sheet.GetRow(rowIdx);
                    if (row == null)
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(row.GetCell(0)?.StringCellValue))
                    {
                        break;
                    }

                    var skillEntry = new SkillEntry
                    {
                        Id = int.Parse(row.GetCell(0).StringCellValue),
                        Key1 = Enum.Parse<KeyType>(sheetName),
                        Key2 = !string.IsNullOrEmpty(row.GetCell(1)?.StringCellValue)
                            ? Enum.Parse<KeyType>(row.GetCell(1).StringCellValue)
                            : null,
                        Description = row.GetCell(2)?.StringCellValue ?? string.Empty,
                    };
                    records.Add(skillEntry);
                }
            }

            
            var csvPath = Path.Combine(Application.dataPath, "Resources/skills.csv");
            using var sw = new StreamWriter(csvPath);
            var csvWriter = new CsvWriter(sw, CultureInfo.InvariantCulture);
            csvWriter.WriteRecords(records);
            EditorUtility.DisplayDialog("", "完成！", "确定");
            File.Delete(excelPath);
        }
    }
}