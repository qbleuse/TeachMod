using System;
using System.Collections.Generic;
using UnityEngine;

/* MCQ stands for Multiple Choice Questions.
 * It also handles the case of a single choice.
 * Those can be asked or not at the end of all sequences, 
 * depending on the POI. 
 * The choice of a class over a struct is because a POI can not have those
 * so we can put null in those case. */
[System.Serializable]
public class MCQ : ScriptableObject, IComparable<MCQ>
{
    /*==== SETTINGS ====*/
    /* The question asked. the question should be asked and should propose the different answer. 
     * ex:
     * 
     * "What does POI stand for ?
     * A : Possible Original Intellect
     * B : Probably Oarsmanships Irresponsibility
     * C : Point Of Interest
     * "
     * 
     * as you can see, the question and the answers are written.
     */
    [TextArea] public string _question = null;

    /* The nb of answer your MCQ offers. in the above example it would be 3. */
    [Range(2, 5)] public int _answerNb = 0;

    /* the numbers that corrsponds to the right answers, beginning from 0.
     * in the above example, there is only one, it is 2 for C */
    [HideInInspector] public List<int> _rightAnswerNb = new List<int>();

    /* is the mcq an mcq where only one answer is allowed */
    public bool _singleAnswer = false;

    /* does the mcq pause or not */
    public bool _pause = false;
    /* wich sequence does the mcq stops */
    public int _sequence = 0;
    /* when does it pause if it does */
    public float _timestamp = 0.0f;
    /* additional comment */
    [TextArea] public string _comment = null;

    /*==== STATE ====*/
    [HideInInspector] public int _serialID = -1;

    [HideInInspector] public bool _answered = false;

    public int CompareTo(MCQ other)
    {
        if (_sequence < other._sequence || (_sequence == other._sequence && _timestamp < other._timestamp))
        {
            return -1;
        }

        return 1;
    }
}
