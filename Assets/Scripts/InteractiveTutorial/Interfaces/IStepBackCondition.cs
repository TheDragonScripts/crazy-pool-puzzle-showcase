using System.Collections;
using System.Collections.Generic;

namespace InteractiveTutorial
{
    public interface IStepBackCondition
    {
        List<BallStepBackInfo> BallsInfo { get; }
        bool Check();
        IEnumerator StepBack();
        void SetupStepBackInfo();
    }
}