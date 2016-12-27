using UnityEngine;
using System.Collections.Generic;

namespace AI.Brain
{
    public class GlobalAILogicScheduler : MonoBehaviour {
        static GlobalAILogicScheduler instance;
        public static GlobalAILogicScheduler Instance { get { return instance; } }

        List<IAILogic> _aiLogics;

        void Awake()
        {
            instance = this;
            _aiLogics = new List<IAILogic>();
        }

        void Update()
        {
            foreach (var logic in _aiLogics)
                logic.Think();
        }

        public void AddAILogic(IAILogic logic)
        {
            _aiLogics.Add(logic);
        }

        public void RemoveAILogic(IAILogic logic)
        {
            _aiLogics.Remove(logic);
        }
    }
}
