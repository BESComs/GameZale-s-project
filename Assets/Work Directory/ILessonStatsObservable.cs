
using System;

namespace Work_Directory
{
    public interface ILessonStatsObservable : ILessonTimeObservable, ILessonScoreObservable
    {
    }

    public interface ILessonScoreObservable
    {
        int MaxScore { get; set; }
        void RegisterAnswer(bool isAnswerRight);
    }


    public interface ILessonTimeObservable
    {
        void RegisterLessonStart();
        void RegisterLessonEnd();
    }
    
    
}