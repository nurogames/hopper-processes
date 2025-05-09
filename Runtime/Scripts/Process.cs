using UnityEngine;
using System;
using System.Collections;

namespace Nuro.Processes
{
    public class Process
    {
        // Enums
        public enum ProcessStatus
        {
            Default,
            Running,
            Succeeded,
            Canceled,
            Failed
        }

        // Public variables
        public Action<ProcessStatus> onFinished;
        // Protected variables
        protected ProcessStatus m_Status = ProcessStatus.Default;
        
        // Private variables
        private string m_Name = null;
        private Sprite m_Icon = null;
        private Coroutine m_HandleCoroutine = null;

        private float m_Progress = 0.0f;
        private float m_StartTime = 0.0f;
        private float m_EndTime = 0.0f;

        // Events
        public Action<float> onProgressChanged;

        // Getter
        public string Name => m_Name;
        public Sprite Icon => m_Icon;
        public ProcessStatus Status => m_Status;
        public Coroutine HandleCoroutine => m_HandleCoroutine;
        public virtual float Progress => m_Progress;

        public float StartTime => m_StartTime;
        public float EndTime => m_EndTime;
        public float Duration => m_EndTime - m_StartTime;

        // Constructor
        public Process( string name = null, Sprite icon = null )
        {
            m_Name = string.IsNullOrEmpty(name) ? "Unknown process" : name;
            m_Icon = icon;
        }

        // Public functions
        public bool IsFinished() => m_Status == ProcessStatus.Succeeded || m_Status == ProcessStatus.Canceled || m_Status == ProcessStatus.Failed;

        public void Start()
        {
            ProcessManager.Instance.AddProcess( this );
        }

        public virtual void Cancel()
        {
            ProcessManager.Instance.CancelProcess( this );
        }

        public IEnumerator ProcessManagerHandle()
        {
            m_StartTime = Time.time;
            m_Status = ProcessStatus.Running;

            yield return ProcessFunc();

            if(m_Status == ProcessStatus.Running)
                m_Status = ProcessStatus.Succeeded;

            m_Progress = 1.0f;

            m_EndTime = Time.time;
        }

        public void SetHandleCoroutine( Coroutine coroutine ) => m_HandleCoroutine = coroutine;

        public void HandleCancelation()
        {
            m_Status = ProcessStatus.Canceled;
            m_HandleCoroutine = null;
        }

        // Protected function
        protected void ChangeProgress( float progress )
        {
            m_Progress = progress;
            onProgressChanged?.Invoke( progress );
        }

        protected virtual IEnumerator ProcessFunc()
        {
            yield break;
        }
    }
}
