using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using NaughtyAttributes;

namespace QuestionManagement
{
    public class QuestionManager : SingletonMonoBehaviour<QuestionManager>
    {
        Dictionary<string, Question.QuestionType> typeMapping = new Dictionary<string, Question.QuestionType>
        {
            {"R1", Question.QuestionType.R1},
            {"1", Question.QuestionType.R1},
            {"Round 1", Question.QuestionType.R1},

            {"R2", Question.QuestionType.R2},
            {"2", Question.QuestionType.R2},
            {"Round 2", Question.QuestionType.R2},

            {"R3", Question.QuestionType.R3},
            {"3", Question.QuestionType.R3},
            {"Round 3", Question.QuestionType.R3},

            {"R4", Question.QuestionType.R4},
            {"4", Question.QuestionType.R4},
            {"Round 4", Question.QuestionType.R4},

            {"R5", Question.QuestionType.R5},
            {"5", Question.QuestionType.R5},
            {"Round 5", Question.QuestionType.R5},
        };

        public Questions LoadedPack = new Questions();

        #region Inspector Preview

        [Header("Question Preview")]
        [ShowOnly] public string roundPreview;
        [ShowOnly] public string questionTextPreview;
        [ShowOnly] public List<string> answerTextPreview;
        [ShowOnly] [TextArea(2,4)] public string hostNotesPreview;

        private int _currentlyPreviewingIndex = 0;
        public int CurrentlyPreviewingIndex
        {
            get { return _currentlyPreviewingIndex; }
            set
            {
                _currentlyPreviewingIndex = value;
                RefreshPreview();
            }
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void ShowPreviousQuestion()
        {
            if (LoadedPack == null || LoadedPack.Count == 0)
                return;
            CurrentlyPreviewingIndex = (CurrentlyPreviewingIndex + LoadedPack.Count - 1) % LoadedPack.Count;
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void ShowNextQuestion()
        {
            if (LoadedPack == null || LoadedPack.Count == 0)
                return;
            CurrentlyPreviewingIndex = (CurrentlyPreviewingIndex + 1) % LoadedPack.Count;
        }

        private void RefreshPreview()
        {
            if (LoadedPack == null || LoadedPack.Count == 0)
                return;
            roundPreview = LoadedPack[CurrentlyPreviewingIndex].QType.ToString();
            questionTextPreview = LoadedPack[CurrentlyPreviewingIndex].QuestionText;
            answerTextPreview.Clear();
            foreach (Answer a in LoadedPack[CurrentlyPreviewingIndex].Answers)
                answerTextPreview.Add($"{a.AnswerText} [{(a.IsCorrect ? "Correct" : "Incorrect")}]");
            hostNotesPreview = LoadedPack[CurrentlyPreviewingIndex].HostNotes;
        }

        #endregion

        public void LoadPack(List<string[]> qs)
        {
            ///Custom pack construction should happen in here
            ///The import manager class shouldn't need to be touched
            ///Your example .csv should be defined in Persistence Manager
            ///You may wish to redefine the typeMapping Dictionary at the top of this class for rounds with specific names
            
            //Clear out any existing pack
            LoadedPack.Clear();
            
            //Remove the top row of our .csv (definitions)
            qs.RemoveAt(0);
            foreach (string[] sArr in qs)
            {
                //First column has data - new question
                if(!string.IsNullOrEmpty(sArr.FirstOrDefault()))
                {
                    Question q = new Question();
                    if (typeMapping.TryGetValue(sArr.FirstOrDefault(), out Question.QuestionType qType))
                        q.QType = qType;
                    q.QuestionText = sArr[1];
                    q.Answers.Add(new Answer(sArr[2], sArr[3].ToUpperInvariant() == "TRUE"));
                    q.HostNotes = sArr[4];
                    LoadedPack.Add(q);
                }
                //No data in first column - this is an answer to be added onto the previous question
                else
                    LoadedPack.LastOrDefault().Answers.Add(new Answer(sArr[2], sArr[3].ToUpperInvariant() == "TRUE"));
            }
            RefreshPreview();
        }

        public int GetRoundQCount()
        {
            return 0;
        }

        public Question GetQuestion(int qNum)
        {
            return null;
        }
    }

    public class Questions : List<Question>
    {
        public Questions()
        {

        }

        public Questions(string[] data)
        {

        }
    }

    public class ImportedRawQuestion
    {
        public string[] qDataRaw;
    }

    public class Question
    {
        /// <summary>
        /// Any bespoke question properties should be defined in here
        /// </summary>

        public enum QuestionType
        {
            Unknown,
            R1,
            R2,
            R3,
            R4,
            R5
        };

        public Question()
        {
            ID = Guid.NewGuid();
        }

        private Guid _id;
        public Guid ID
        {
            get { return _id; }
            set
            {
                _id = value;
            }
        }

        private QuestionType _qType;
        public QuestionType QType
        {
            get { return _qType; }
            set
            {
                _qType = value;
            }
        }

        private string _questionText;
        public string QuestionText
        {
            get { return _questionText; }
            set
            {
                _questionText = value;
            }
        }

        private string _hostNotes;
        public string HostNotes
        {
            get { return _hostNotes; }
            set
            {
                _hostNotes = value;
            }
        }

        private List<Answer> _answers = new List<Answer>();
        public List<Answer> Answers
        {
            get { return _answers; }
            set
            {
                _answers = value;
            }
        }
    }

    public class Answer
    {
        public Answer(string answerText, bool isCorrect)
        {
            AnswerText = answerText;
            IsCorrect = isCorrect;
        }

        private string _answerText;
        public string AnswerText
        {
            get { return _answerText; }
            set
            {
                _answerText = value;
            }
        }

        private bool _isCorrect;
        public bool IsCorrect
        {
            get { return _isCorrect; }
            set
            {
                _isCorrect = value;
            }
        }
    }
}



