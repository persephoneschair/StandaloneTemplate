using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestionManager : SingletonMonoBehaviour<QuestionManager>
{
    public Questions LoadedPack = new Questions();

    public void DecompilePack()
    {

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

}

public class Question
{
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

    private string _questionText;
    public string QuestionText
    { 
        get { return _questionText; }
        set
        {
            _questionText = value;
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

