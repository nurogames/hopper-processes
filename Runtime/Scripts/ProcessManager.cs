using UnityEngine;
using System;
using System.Collections.Generic;

namespace Nuro.Processes
{

    public class ProcessManager : MonoBehaviour
    {
        // Singleton
        public static ProcessManager Instance { get; private set; }

        // Private variables
        List <Process> m_Processes = new List <Process> ();
        
        // Events
        public event Action < Process > OnProcessAdded;
        public event Action OnUpdateProgressUI;
        public event Action < Process > OnProcessFinished;

        // Getter
        public Process[] GetAllRunningProcesses => m_Processes.ToArray();

        // Awake function
        private void Awake()
        {
            Instance = this;
        }

        // Public functions
        public void AddProcess( Process process, bool start = true )
        {
            if( process == null )
                return;

            if( m_Processes.Contains(process) )
                return;

            m_Processes.Add ( process );

            OnProcessAdded?.Invoke(process);

            if ( start )
                StartProcess( process );
        }

        public void StartProcess(Process process)
        {
            if( process.Status != Process.ProcessStatus.Default )
                return;

            Coroutine coroutine = StartCoroutine(process.ProcessManagerHandle());
            process.SetHandleCoroutine(coroutine);
        }
        
        public void CancelProcess( Process process )
        {
            if( process == null )
                return;

            if ( process.HandleCoroutine != null )
            {
                StopCoroutine( process.HandleCoroutine );
                process.HandleCancelation();
            }
        }

        // Update
        private void Update()
        {
            if( m_Processes.Count == 0 )
                return;

            for( int index = m_Processes.Count - 1; index >= 0; index-- )
            {
                Process process = m_Processes[index];
                if ( process.IsFinished() )
                {
                    m_Processes.RemoveAt( index );
                    process.onFinished?.Invoke( process.Status );
                    OnProcessFinished?.Invoke( process );
                }
            }

            OnUpdateProgressUI?.Invoke();
        }
    }

}