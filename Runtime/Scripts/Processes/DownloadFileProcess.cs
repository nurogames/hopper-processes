using System.Collections;
using System.Collections.Generic;
using Nuro.Processes;
using UnityEngine;
using UnityEngine.Networking;

namespace Nuro.Processes
{
    public class DownloadFileProcess : Process
    {
        private string m_Url;
        private string m_Filepath;

        public DownloadFileProcess(string url, string filepath) : base($"DownloadFileProcess({url})")
        {
            m_Url = url;
            m_Filepath = filepath;
        }

        protected override IEnumerator ProcessFunc()
        {
            using (UnityWebRequest webRequest = new UnityWebRequest(m_Url, UnityWebRequest.kHttpVerbGET))
            {
                webRequest.downloadHandler = new DownloadHandlerFile(m_Filepath);
                yield return webRequest.SendWebRequest();
                
                if (webRequest.result != UnityWebRequest.Result.Success )
                {
                    Debug.LogError($"Error: {webRequest.error}");
                    foreach (KeyValuePair<string,string> responseHeader in webRequest.GetResponseHeaders())
                    {
                        Debug.LogError($"Response Header: {responseHeader.Key}:{responseHeader.Value}"); // Debugging headers
                    }
                }
                else
                {
                    Debug.Log($"Successfully saved file to {m_Filepath}");
                }
            }
        }
    }
}