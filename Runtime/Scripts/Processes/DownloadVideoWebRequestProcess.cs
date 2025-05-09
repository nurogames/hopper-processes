using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Nuro.Processes;

namespace WorldBuilder.Core.Processes
{
public class DownloadVideoWebRequestProcess : Process
{
    // Private variables
    private string m_URI = null;

    private UnityWebRequest m_WebRequest = null;

    private byte[] m_ResultData = null;

    // Getter
    public override float Progress =>
        m_WebRequest == null ? 0.0f : m_WebRequest.downloadProgress + m_WebRequest.uploadProgress;

    public string URI => m_URI;


    #region Public

    // Constructor
    public DownloadVideoWebRequestProcess(string name, string uri) : base(name)
    {
        m_URI = uri;

        Debug.Log( "Starting Loading of " + uri + " . . ." );
    }

    public byte[] ResultData => m_ResultData;


    #endregion

    #region Protected

    protected override IEnumerator ProcessFunc()
    {
        m_WebRequest = UnityWebRequest.Get(m_URI);

        m_Status = Process.ProcessStatus.Running;

        yield return m_WebRequest.SendWebRequest();

        if (m_WebRequest.result == UnityWebRequest.Result.Success)
        {
            m_Status = Process.ProcessStatus.Succeeded;
            m_ResultData = m_WebRequest.downloadHandler.data;
            OnSuccess();
        }
        else
        {
            m_Status = Process.ProcessStatus.Failed;
            OnError(m_WebRequest.result);
        }
    }

    protected virtual void OnSuccess()
    {
        Debug.Log( "Loading of "+m_URI+ " completed" );
    }

    protected virtual void OnError(UnityWebRequest.Result errorResult)
    {
        Debug.LogError( errorResult.ToString() );
    }

    #endregion
}

}
