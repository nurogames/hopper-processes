using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Nuro.Processes
{

    public class GetWebRequestProcess : Process
    {
        // Private variables
        private string m_URI = null;

        private UnityWebRequest m_WebRequest = null;
        
        // Getter
        public override float Progress => m_WebRequest == null ? 0.0f : m_WebRequest.downloadProgress + m_WebRequest.uploadProgress;
        public string URI => m_URI;

        #region Public

        // Constructor
        public GetWebRequestProcess( string name, string uri, Sprite icon = null) : base( name, icon )
        {
            m_URI = uri;
        }

        #endregion

        #region Protected

        protected override IEnumerator ProcessFunc()
        {
            m_WebRequest = UnityWebRequest.Get( m_URI );

            m_Status = ProcessStatus.Running;
            yield return m_WebRequest.SendWebRequest();

            if ( m_WebRequest.result == UnityWebRequest.Result.Success )
            {
                m_Status = ProcessStatus.Succeeded;
                OnSuccess( m_WebRequest.downloadHandler.text );
            }
            else
            {
                m_Status = ProcessStatus.Failed;
                OnError( m_WebRequest.result );
            }
        }

        protected virtual void OnSuccess( string requestResult )
        {

        }

        protected virtual void OnError( UnityWebRequest.Result errorResult )
        {

        }

        #endregion
    }

}
