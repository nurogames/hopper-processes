using UnityEngine;
using System.Collections;

namespace Nuro.Processes
{

    public class WaitProcess : Process
    {
        // Private variables
        private float m_Duration = 0.0f;
        private float m_TimePassed = 0.0f;

        // Constructor
        public WaitProcess( string name, float duration )
            : base( name )
        {
            this.m_Duration = duration <= 0.0f ? 0.1f : duration;
        }

        protected override IEnumerator ProcessFunc()
        {
            while ( m_TimePassed < m_Duration )
            {
                m_TimePassed += Time.deltaTime;
                ChangeProgress( m_TimePassed / m_Duration );
                yield return null;
            }

            ChangeProgress( 1.0f );
        }
    }

}