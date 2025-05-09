using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Nuro.Processes
{

    public class ContainerProcess : Process
    {
        private class ProcessData
        {
            public Process m_Process;
            public Process[] m_ProcessesToBeFinishedSuccesfully;
        }

        List < ProcessData > m_SubProcesses = new List < ProcessData >();

        public ContainerProcess( string name, Sprite icon = null ) : base( name, icon )
        {

        }
        
        public void Add(Process process, Process processToBeFinishedSuccesfully)
        {
            Add(
                process,
                processToBeFinishedSuccesfully == null
                    ? null
                    : new Process[] { processToBeFinishedSuccesfully } );
        }

        public void Add( Process process, List < Process > processesToBeFinishedSuccesfully )
        {
            Add( process, 
                processesToBeFinishedSuccesfully == null || processesToBeFinishedSuccesfully.Count == 0 ?
                null : processesToBeFinishedSuccesfully.ToArray() );
        }

        public void Add( Process process, Process[] processesToBeFinishedSuccesfully = null )
        {
            ProcessData data = new ProcessData
            {
                m_Process = process, m_ProcessesToBeFinishedSuccesfully = processesToBeFinishedSuccesfully
            };

            m_SubProcesses.Add( data );
        }

        public override void Cancel()
        {
            base.Cancel();

            CancelAllSubProcesses();
        }

        protected override IEnumerator ProcessFunc()
        {
            foreach ( ProcessData processData in m_SubProcesses )
                ProcessManager.Instance.AddProcess( processData.m_Process, false );

            do
            {
                float totalProgress = 0.0f;

                foreach ( ProcessData processData in m_SubProcesses )
                {
                    Process process = processData.m_Process;

                    Process causeProgress = null;

                    if ( CanStartProcess( processData, ref causeProgress ) && causeProgress == null )
                        ProcessManager.Instance.StartProcess( process );

                    if ( causeProgress != null )
                    {
                        CancelAllSubProcesses();
                        m_Status = causeProgress.Status;

                        yield break;
                    }

                    totalProgress += process.Progress;
                }

                ChangeProgress( m_SubProcesses.Count > 0 ? totalProgress / m_SubProcesses.Count : 1.0f );

                yield return null;
            }
            while ( !AllProcessesFinished() );

            ChangeProgress( 1.0f );
        }

        protected List<Process> GetAllProcesses()
        {
            List<Process> list = new List<Process>();
            foreach(ProcessData processData in m_SubProcesses)
                list.Add(processData.m_Process );
            return list;
        }

        bool AllProcessesFinished()
        {
            for ( int index = 0; index < m_SubProcesses.Count; index++ )
            {
                if ( !m_SubProcesses[index].m_Process.IsFinished() )
                    return false;
            }

            m_Status = ProcessStatus.Succeeded;
			for (int index = 0; index < m_SubProcesses.Count; index++)
			{
                Process.ProcessStatus status = m_SubProcesses[index].m_Process.Status;
				if (status == Process.ProcessStatus.Failed || status ==  Process.ProcessStatus.Canceled)
                {
                    m_Status = status;
                    break;
                }
			}

			return true;
        }

        bool CanStartProcess( ProcessData processData, ref Process causeProcess )
        {
            if ( processData.m_Process.Status != ProcessStatus.Default )
                return false;

            if ( processData.m_ProcessesToBeFinishedSuccesfully == null )
                return true;

            foreach ( Process conditionProcess in processData.m_ProcessesToBeFinishedSuccesfully )
            {
                if (conditionProcess == null) 
                    continue;

                if ( conditionProcess.Status == ProcessStatus.Canceled ||
                     conditionProcess.Status == ProcessStatus.Failed )
                {
                    causeProcess = conditionProcess;
                    Debug.Log(processData.m_Process.Name + " | " + conditionProcess.Name + " | " + conditionProcess.Status);
                }

                if ( !conditionProcess.IsFinished() )
                {
                    return false;
                }
            }

            return true;
        }

        void CancelAllSubProcesses()
        {
            foreach ( ProcessData processData in m_SubProcesses )
            {
                Process process = processData.m_Process;

                if ( !process.IsFinished() )
				{
					process.Cancel();
				}
            }
        }
    }


}