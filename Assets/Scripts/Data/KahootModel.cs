using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Kahoot
{
    public string id;
    public string title;
    public string description;
    public int timePerQuestion = 15;
    public List<Question> questions;
}

[Serializable]
public class Question
{
    public int questionId;
    public string text;
    public List<string> options;
    public int correctIndex;
}
