using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Nuro.Processes
{
    [Serializable]
    public class ErrorData
    {
        public string error;
        public string error_description;
    }

    public class WebRequestProcess : Process
    {
        public enum MethodTypes { GET, POST, PATCH, DELETE }
        // Private variables
        protected MethodTypes m_Method = MethodTypes.GET;
        protected string m_URI = null;
        protected string m_Data;
        protected List<IMultipartFormSection> m_Multipart;
        protected WWWForm m_FormData;
        protected Dictionary<string, string> m_Headers = new Dictionary<string, string>();

        protected string m_Result;

        protected byte[] m_BinaryResult;

        // Getter
        public string URI => m_URI;
        public string Result => m_Result;
        public byte[] BinaryResult => m_BinaryResult;

        // Constructor
        public WebRequestProcess(string name, string uri, Sprite icon) : base(name, icon)
        {
            m_URI = uri;
        }

        public WebRequestProcess(MethodTypes method, string name, string uri, string data, Dictionary<string, string> headers, Sprite icon) : base(name, icon)
        {
            m_Method = method;
            m_URI = uri;
            m_Data = data;
            m_Headers = headers;
        }

        public WebRequestProcess(MethodTypes method, string name, string uri, WWWForm formData, Dictionary<string, string> headers, Sprite icon) : base(name, icon)
        {
            m_Method = method;
            m_URI = uri;
            m_FormData = formData;
            m_Headers = headers;
        }

        protected override IEnumerator ProcessFunc()
        {
            m_Status = ProcessStatus.Running;

            if (m_Method == MethodTypes.GET)
            {
                using (UnityWebRequest webRequest = UnityWebRequest.Get(m_URI))
                {
                    if (m_Headers != null)
                    {
                        foreach (KeyValuePair<string, string> header in m_Headers)
                        {
                            webRequest.SetRequestHeader(header.Key, header.Value);
                        }
                    }

                    UnityWebRequestAsyncOperation operation = webRequest.SendWebRequest();
                    while (!operation.isDone)
                    {
                        ChangeProgress(operation.progress);
                        yield return null;
                    }
                    ChangeProgress(1f);

                    if (webRequest.result == UnityWebRequest.Result.Success)
                    {
                        m_Status = ProcessStatus.Succeeded;
                        m_BinaryResult = webRequest.downloadHandler.data;
                        OnSuccess(webRequest.downloadHandler.text);
                    }
                    else
                    {
                        m_Status = ProcessStatus.Failed;
                        try
                        {
                            ErrorData errorData = JsonUtility.FromJson<ErrorData>(webRequest.downloadHandler.text);
                            OnError(webRequest.result, webRequest.error + ": \n" + errorData.error + "\n" + errorData.error_description);

                        }
                        catch
                        {
                            OnError(webRequest.result, webRequest.downloadHandler.text);
                        }
                    }
                }
            }
            else if (m_Method == MethodTypes.POST || m_Method == MethodTypes.PATCH)
            {
                if (m_Multipart != null)
                {
                    using (UnityWebRequest webRequest = UnityWebRequest.Post(m_URI, m_Multipart))
                    {
                        if (m_Method == MethodTypes.PATCH)
                            webRequest.method = "PATCH";

                        if (m_Headers != null)
                        {
                            foreach (KeyValuePair<string, string> header in m_Headers)
                            {
                                webRequest.SetRequestHeader(header.Key, header.Value);
                            }
                        }

                        UnityWebRequestAsyncOperation operation = webRequest.SendWebRequest();
                        while (!operation.isDone)
                        {
                            ChangeProgress(operation.progress);
                            yield return null;
                        }
                        ChangeProgress(1f);

                        if (webRequest.result == UnityWebRequest.Result.Success)
                        {
                            m_Status = ProcessStatus.Succeeded;
                            m_BinaryResult = webRequest.downloadHandler.data;
                            OnSuccess(webRequest.downloadHandler.text);
                        }
                        else
                        {
                            m_Status = ProcessStatus.Failed;
                            try
                            {
                                ErrorData errorData = JsonUtility.FromJson<ErrorData>(webRequest.downloadHandler.text);
                                OnError(webRequest.result, webRequest.error + ": \n" + errorData.error + "\n" + errorData.error_description);

                            }
                            catch
                            {
                                OnError(webRequest.result, webRequest.downloadHandler.text);
                            }
                        }
                    }
                }
                else if (m_FormData == null)
                {
                    using (UnityWebRequest webRequest = UnityWebRequest.Post(m_URI, m_Data, "application/json"))
                    {
                        if (m_Method == MethodTypes.PATCH)
                            webRequest.method = "PATCH";

                        if (m_Headers != null)
                        {
                            foreach (KeyValuePair<string, string> header in m_Headers)
                            {
                                webRequest.SetRequestHeader(header.Key, header.Value);
                            }
                        }

                        UnityWebRequestAsyncOperation operation = webRequest.SendWebRequest();
                        while (!operation.isDone)
                        {
                            ChangeProgress(operation.progress);
                            yield return null;
                        }
                        ChangeProgress(1f);

                        if (webRequest.result == UnityWebRequest.Result.Success)
                        {
                            m_Status = ProcessStatus.Succeeded;
                            m_BinaryResult = webRequest.downloadHandler.data;
                            OnSuccess(webRequest.downloadHandler.text);
                        }
                        else
                        {
                            m_Status = ProcessStatus.Failed;
                            try
                            {
                                ErrorData errorData = JsonUtility.FromJson<ErrorData>(webRequest.downloadHandler.text);
                                OnError(webRequest.result, webRequest.error + ": \n" + errorData.error + "\n" + errorData.error_description);

                            }
                            catch
                            {
                                OnError(webRequest.result, webRequest.downloadHandler.text);
                            }
                        }
                    }
                }
                else
                {
                    using (UnityWebRequest webRequest = UnityWebRequest.Post(m_URI, m_FormData))
                    {
                        if (m_Headers != null)
                        {
                            foreach (KeyValuePair<string, string> header in m_Headers)
                            {
                                webRequest.SetRequestHeader(header.Key, header.Value);
                            }
                        }

                        UnityWebRequestAsyncOperation operation = webRequest.SendWebRequest();
                        while (!operation.isDone)
                        {
                            ChangeProgress(operation.progress);
                            yield return null;
                        }
                        ChangeProgress(1f);

                        if (webRequest.result == UnityWebRequest.Result.Success)
                        {
                            m_Status = ProcessStatus.Succeeded;
                            m_BinaryResult = webRequest.downloadHandler.data;
                            OnSuccess(webRequest.downloadHandler.text);
                        }
                        else
                        {
                            m_Status = ProcessStatus.Failed;
                            try
                            {
                                ErrorData errorData = JsonUtility.FromJson<ErrorData>(webRequest.downloadHandler.text);
                                OnError(webRequest.result, webRequest.error + ": \n" + errorData.error + "\n" + errorData.error_description);

                            }
                            catch
                            {
                                OnError(webRequest.result, webRequest.downloadHandler.text);
                            }
                        }
                    }
                }
            }
            else if (m_Method == MethodTypes.DELETE)
            {
                using (UnityWebRequest webRequest = UnityWebRequest.Delete(m_URI))
                {
                    if (m_Headers != null)
                    {
                        foreach (KeyValuePair<string, string> header in m_Headers)
                        {
                            webRequest.SetRequestHeader(header.Key, header.Value);
                        }
                    }

                    UnityWebRequestAsyncOperation operation = webRequest.SendWebRequest();
                    while (!operation.isDone)
                    {
                        ChangeProgress(operation.progress);
                        yield return null;
                    }
                    ChangeProgress(1f);

                    if (webRequest.result == UnityWebRequest.Result.Success)
                    {
                        m_Status = ProcessStatus.Succeeded;
                        m_BinaryResult = webRequest.downloadHandler.data;
                        OnSuccess("");
                    }
                    else
                    {
                        m_Status = ProcessStatus.Failed;
                        try
                        {
                            ErrorData errorData = JsonUtility.FromJson<ErrorData>(webRequest.downloadHandler.text);
                            OnError(webRequest.result, webRequest.error + ": \n" + errorData.error + "\n" + errorData.error_description);

                        }
                        catch
                        {
                            OnError(webRequest.result, webRequest.downloadHandler.text);
                        }
                    }
                }
            }
        }

        protected virtual void OnSuccess(string requestResult)
        {
            m_Status = ProcessStatus.Succeeded;
            m_Result = requestResult;
        }

        protected virtual void OnError(UnityWebRequest.Result errorResult, string errorMessage)
        {
            m_Status = ProcessStatus.Failed;
            m_Result = errorMessage;
        }
    }
}