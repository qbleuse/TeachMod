using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* MCQ stands for Multiple Choice Questions.
 * Those can be asked or not at the end of all sequences, 
 * depending on the POI. 
 * The choice of a class over a struct is because a POI can not have those
 * so we can put null in those case. */
[System.Serializable]
public class MCQ
{
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
    [Multiline]     public string   _question = null;

    /* The nb of answer your MCQ offers. in the above example it would be 3. */
    [Range(2,4)]    public uint     _answerNb = 0;

    /* the number that corrsponds to the right answer, beginning from 0.
     * in the above example it is 2 for C */
    public uint _rightAnswerNb = 0;

    public bool answered = false;
}
