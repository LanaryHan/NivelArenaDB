using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace Logic
{
    public class GameRuntimeLogic : MonoSingleton<GameRuntimeLogic>
    {
        private const int GroupN = 5;
        public InitTask.Runner InitTask { get; private set; } = new();
        
        private List<Type> _logicForCollected = new()
        {
            typeof(BuildDeckLogic)
        };

        protected Dictionary<Type, LogicBase> LogicDic = new();

        private void Start()
        {
            InitTask.OnFinishEvent = () =>
            {
#if UNITY_EDITOR
                Debug.Log("Init finish");
#endif
            };

            CollectTasks();
            InitTask.RunWith(this);
        }

        private void AddLogic(Type type)
        {
            var go = new GameObject(type.Name);
            go.transform.parent = transform;
            var logic = go.AddComponent(type) as LogicBase;
            LogicDic[type] = logic;
        }

        public T GetLogic<T>() where T : LogicBase
        {
            if (LogicDic.ContainsKey(typeof(T)))
            {
                return LogicDic[typeof(T)] as T;
            }

            return null;
        }
        
        private void CollectTasks()
        {
            for (int i = 0; i < _logicForCollected.Count; i += GroupN)
            {
                var index = i;
                InitTask.Add(() =>
                {
                    for (int n = 0; index + n < _logicForCollected.Count && n < GroupN; n++)
                    {
                        var logic = _logicForCollected[index + n];
                        AddLogic(logic);
                    }
                });
            }
        }
    }
}