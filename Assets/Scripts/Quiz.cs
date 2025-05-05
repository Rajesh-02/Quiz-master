using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Quiz : MonoBehaviour
{
    [Header("Questions")]
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] List<QuestionSO> questions = new List<QuestionSO>();
    List<QuestionSO> remainingQuestions;
     QuestionSO currentQuestion;

    [Header("Answers")]
    [SerializeField] GameObject[] answerButtons;
    int corrextAnswerIndex;

    bool hasAnsweredEarly = true;

    [Header("Button Colors")]
    [SerializeField] Sprite defaultAnswerSprite;
    [SerializeField] Sprite correctAnswerSprite;

    [Header("Timer")]
    [SerializeField] Image timerImage;
    Timer timer;

    [Header("Scoring")]
    [SerializeField] TextMeshProUGUI scoreText;
    ScoreKeeper scoreKeeper;

    [Header("PrgressBar")]
    [SerializeField] Slider progressBar;

    public bool isComplete;

    void Awake()
    {
        timer = FindObjectOfType<Timer>();
        remainingQuestions = new List<QuestionSO>(questions);
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        progressBar.maxValue = questions.Count;
        progressBar.value = 0;
        
    }

    void Update()
    {
        timerImage.fillAmount = timer.fillFraction;
        if(timer.loadNextQuestion){
         if(progressBar.value == progressBar.maxValue)
         {
            isComplete = true;
            return;
         }
            hasAnsweredEarly= false;
            GetNextQuestion();
            timer.loadNextQuestion =false;
        }
        else if(!hasAnsweredEarly && !timer.isAnsweringQuestion)
        {
            DisplayAnswer(-1);
            SetButtonState(false);
        }
    }

    public void OnAnswerSelected(int index)
    {
        hasAnsweredEarly = true;
        DisplayAnswer(index);
        SetButtonState(false);
        timer.CancelTimer();
        scoreText.text = "Score: " + scoreKeeper.CalculateScore() + " % ";
       
    }

    void DisplayAnswer(int index)
    {
        Image buttonImage;
        if(index == currentQuestion.GetCorrectAnswerIndex())
        {
            questionText.text = "Correct!";
            buttonImage = answerButtons[index].GetComponent<Image>(); 
            buttonImage.sprite = correctAnswerSprite;
            scoreKeeper.IncrementCorrectAnswers();
        }
        else{
            corrextAnswerIndex = currentQuestion.GetCorrectAnswerIndex();
            string correctAnswer = currentQuestion.GetAnswer(corrextAnswerIndex);
            questionText.text = "Sorry, the correct answer was; \n" + correctAnswer;
            buttonImage = answerButtons[corrextAnswerIndex].GetComponent<Image>();
            buttonImage.sprite = correctAnswerSprite;    
            }
    }
    void GetNextQuestion()
    {
        if(questions.Count > 0)
        {
        SetButtonState(true);
        SetDefaultButtonSprites();
        GetRandomQuestion();
        DisplayQuestion();
        progressBar.value++;
        scoreKeeper.IncrementQuestionsSeen();
        }
        
    }

    void GetRandomQuestion()
    {
        // int index = Random.Range(0, questions.Count);
        // currentQuestion = questions[index];
        // if(questions.Contains(currentQuestion))
        // {
        //     questions.Remove(currentQuestion);
        // }
    int index = Random.Range(0, remainingQuestions.Count);
    currentQuestion = remainingQuestions[index];
    remainingQuestions.RemoveAt(index);
    }
   void DisplayQuestion()
    {
          if (currentQuestion == null)
    {
        Debug.LogError("Current question is null!");
        return;
    }
        questionText.text = currentQuestion.GetQuestion();

        for(int i=0; i< answerButtons.Length; i++){
        TextMeshProUGUI buttonText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = currentQuestion.GetAnswer(i);
        }
        else
        {
            Debug.LogWarning($"Missing TextMeshProUGUI on button {i}");
        }
        }
    }

    void SetButtonState(bool state)
    {
        for(int i=0; i<answerButtons.Length; i++)
        {
            Button button = answerButtons[i].GetComponent<Button>();
            button.interactable = state;
        }
    }

    void SetDefaultButtonSprites()
    {
        for(int i=0; i<answerButtons.Length; i++)
        {
            Image buttonImage = answerButtons[i].GetComponent<Image>();
            buttonImage.sprite = defaultAnswerSprite;
        }
    }
}