using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Logic
{
    public class InitTask
    {
        private InitTask()
        {
            
        }
        
        private Action _action;
        private Func<IEnumerator> _func;
        private string _name;

        private IEnumerator RunAction()
        {
            if (_action != null)
            {
                Profiler.BeginSample($"init task {_name}");
                _action();
                Profiler.EndSample();
                yield return null;
            }
            else
            {
                yield return _func();
            }
        }
        
        public class Runner
        {
            public bool Pause { get; set; }
            public Action OnFinishEvent;
            public string CurrentTask;
            
            private Queue<InitTask> _tasks = new();

            public void Add(Action action, string name = null)
            {
                _tasks.Enqueue(new InitTask
                {
                    _action = action,
                    _name = name
                });
            }

            public void AddFunc(Func<IEnumerator> func, string name = null)
            {
                _tasks.Enqueue(new InitTask
                {
                    _func = func,
                    _name = name
                });
            }

            public void RunWith(MonoBehaviour owner)
            {
                owner.StartCoroutine(Run());
            }

            private IEnumerator Run()
            {
                while (_tasks.Count > 0)
                {
                    if (Pause)
                    {
                        yield return null;
                        continue;
                    }
                    
                    var task = _tasks.Dequeue();
                    CurrentTask = task._name;
                    yield return task.RunAction();
                }
                
                OnFinishEvent?.Invoke();
                CurrentTask = string.Empty;
            }
        }
    }
}