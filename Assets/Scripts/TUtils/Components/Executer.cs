// Copyright (c) 2020 Matteo Beltrame

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TUtils.Components
{
    /// <summary>
    ///   Asynchronous -&gt; Synchronous
    /// </summary>
    public class Executer : MonoBehaviour
    {
        #region Singleton

        private static Executer instance;

        /// <summary>
        ///   Get the Executer singleton instance
        /// </summary>
        public static Executer GetInstance() { return instance; }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        #endregion Singleton

        private Queue<Action> jobs = new Queue<Action>();

        private void Update()
        {
            while (jobs.Count > 0)
            {
                jobs.Dequeue().Invoke();
            }
        }

        /// <summary>
        ///   Enqueue a job to be performed, the job must have the following declaration:
        ///   <code>void F() </code>
        /// </summary>
        public void AddJob(Action job)
        {
            jobs.Enqueue(job);
        }
    }
}