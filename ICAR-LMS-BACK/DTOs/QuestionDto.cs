using System.ComponentModel.DataAnnotations;

namespace LMS.Models.DTOs
{
    public class QuestionDto
    {

        public string QuestionText { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string CorrectOption { get; set; }
        public string DifficultyLevel { get; set; }
        public string Topic { get; set; }
        public int SuggestedMarks { get; set; }
    }   

        public class ExamCreateDto
        {
            public string Title { get; set; }
            public string ExamDate { get; set; }
            public int DurationMinutes { get; set; }
            public int totalmarks { get; set; }
            public int passingmarks { get; set; }
            public int CreatedBy { get; set; }
            public int ExaminationID { get; set; }
            public string Type { get; set; }
            public List<QuestionDto> Questions { get; set; }
        }

    public class PracticeExamCreateDto
    {
        public int UnitId { get; set; }
        public string Title { get; set; }
        public string ExamDate { get; set; }
        public int DurationMinutes { get; set; }
        public int totalmarks { get; set; }
        public int passingmarks { get; set; }
        public int CreatedBy { get; set; }
        public int ExaminationID { get; set; }
        public string Type { get; set; }
        public List<QuestionDto> Questions { get; set; }
    }


    public class ExamWithQuestionsDto
    {
        public int ExamId { get; set; }
        public string Title { get; set; }
        public string ExamDate { get; set; }
        public int DurationMinutes { get; set; }
        public int CreatedBy { get; set; }
        public int ExaminationID { get; set; }
        public string Type { get; set; }
        public List<QuestionDto> Questions { get; set; } = new();
    }
}
