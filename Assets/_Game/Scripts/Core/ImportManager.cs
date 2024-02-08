using SimpleFileBrowser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using QuestionManagement;
using System.Text;

public class ImportManager : SingletonMonoBehaviour<ImportManager>
{
    public Animator errorAnim;
    public TextMeshProUGUI errorMesh;

    public GameObject importSuccessBox;
    public RectTransform importSuccessRT;
    public RectTransform contentScrollerRT;
    public TextMeshProUGUI importSuccessReadout;
    private bool importSuccess = false;
    private bool rtSet = false;

    private const string importErrorAlert = "<color=#FF0000>IMPORT FAILED\n\nCLOSE THIS WINDOW AND TRY AGAIN";

    #region Importer

    public void OnClickImportQuestions()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Question Sheet", ".csv"));
        FileBrowser.SetDefaultFilter(".csv");
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    public IEnumerator ShowLoadDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, PersistenceManager.localDataPath + $"/Question Data", null, "Import Question Data", "Import");
        try
        {
            if (FileBrowser.Success)
                ImportQuestions(File.ReadAllText(FileBrowser.Result.FirstOrDefault()));
            else
                MainMenuManager.Get.OnClickMenuButton(MainMenuManager.NavigationButton.Home);
        }
        catch (Exception ex)
        {
            PrintFailedImportReport(ex);
        }
    }
    private void ImportQuestions(string data)
    {
        List<string[]> splitCsv = ParseCsv(data);
        QuestionManager.Get.LoadPack(splitCsv);
        PrintSuccessfulImportReport();
    }
    public void OnCloseSuccessWindow()
    {
        importSuccessBox.gameObject.SetActive(false);
        if (importSuccess)
        {
            importSuccess = false;
            HackboxManager.Get.DeployOperatorState(HackboxManager.OperatorState.ProgressGameplay);
        }
        else
            MainMenuManager.Get.OnClickMenuButton(MainMenuManager.NavigationButton.Home);
    }

    #endregion

    #region Reports

    private void PrintSuccessfulImportReport()
    {
        int lineBreaks = 4;
        importSuccessReadout.text =
            $"<color=green>IMPORT SUCCESS\nCHECK IMPORT BELOW</color>\n\n";

        var pk = QuestionManager.Get.LoadedPack;
        for (Question.QuestionType i = Question.QuestionType.R1; i <= Question.QuestionType.R5; i++)
        {

            var qsInRnd = pk.Where(x => x.QType == i);
            importSuccessReadout.text += $"<color=blue><u>{i}</u></color>\n";
            lineBreaks++;
            foreach (var q in qsInRnd)
            {
                importSuccessReadout.text += $"<color=orange>Q: {q.QuestionText}</color>\n";
                lineBreaks++;
                foreach (Answer a in q.Answers)
                {
                    importSuccessReadout.text += $"{(a.IsCorrect ? "<color=green>" : "<color=#FF0000>")}Ans: {a.AnswerText}</color>\n";
                    lineBreaks++;
                }
                importSuccessReadout.text += $"<color=yellow>Notes: {q.HostNotes}</color>\n\n";
                lineBreaks += 2;
            }
            importSuccessReadout.text += "\n";
            lineBreaks++;
        }
        importSuccessReadout.text += "\nCLOSING THIS WINDOW WILL PASS CONTROL TO THE OPERATOR";
        lineBreaks += 2;

        importSuccessRT.sizeDelta = new Vector2(1300, lineBreaks * 50);
        if (!rtSet)
        {
            contentScrollerRT.localPosition = new Vector3(0, -1500, 0);
            rtSet = true;
        }
        importSuccessBox.gameObject.SetActive(true);
        importSuccess = true;
    }

    private void PrintFailedImportReport(Exception ex)
    {
        importSuccessReadout.text = importErrorAlert;
        importSuccessRT.sizeDelta = new Vector2(1300, 150);
        if (!rtSet)
        {
            contentScrollerRT.localPosition = new Vector3(0, -1500, 0);
            rtSet = true;
        }
        importSuccessBox.gameObject.SetActive(true);
        importSuccess = false;
        DebugLog.Print(ex.Message, DebugLog.StyleOption.Bold, DebugLog.ColorOption.Red);
    }

    #endregion

    #region CSV Parsing

    static List<string[]> ParseCsv(string rawCsv)
    {
        List<string[]> parsedRows = new List<string[]>();

        using (var reader = new StringReader(rawCsv))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                parsedRows.Add(ParseCsvLine(line));
            }
        }

        return parsedRows;
    }

    static string[] ParseCsvLine(string line)
    {
        List<string> fields = new List<string>();
        bool inQuotes = false;
        string currentField = "";

        foreach (char c in line)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(currentField);
                currentField = "";
            }
            else
            {
                currentField += c;
            }
        }

        fields.Add(currentField);
        return fields.ToArray();
    }

    #endregion
}
