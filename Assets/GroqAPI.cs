using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public class GroqAPI : MonoBehaviour
{
    private static string apiKey = "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";
    [SerializeField] [TextArea(3, 10)] private string userPrompt = "Enter your prompt here";
    [SerializeField] [TextArea(3, 10)] private string CharacterInfo = "Enter your prompt here";
    
    private static string API_URL = "https://api.groq.com/openai/v1/chat/completions";


    public static IEnumerator SendRequest(string userPrompt,string CharacterInfo,Action<string> callback) // meta-llama/llama-4-maverick-17b-128e-instruct  ||  deepseek-r1-distill-llama-70b
    {
        string jsonBody = $@"{{
            ""model"": ""meta-llama/llama-4-maverick-17b-128e-instruct"",
            ""messages"": [
                {{
                    ""role"": ""user"",
                    ""content"": ""{CharacterInfo.Replace("\"", "\\\"").Replace("$$$",userPrompt.Replace("\"", "\\\""))}""
                }}
            ]
            
        }}";
        //,"reasoning_format": "hidden"
        using (UnityWebRequest request = new UnityWebRequest(API_URL, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {request.error} - {request.downloadHandler.text}");
            }
            else
            {
                string responseText = request.downloadHandler.text;
                try
                {
                    var jsonResponse = JsonUtility.FromJson<GroqResponse>(responseText);
                    string content = jsonResponse.choices[0].message.content;

                    string cleanedContent = Regex.Replace(content, @"<think>.*?</think>", "", RegexOptions.Singleline);
                    cleanedContent = cleanedContent.Trim();

                    Debug.Log($"{cleanedContent}");
                    callback?.Invoke(cleanedContent);
                    //Debug.Log($"Full Response (for debugging): {responseText}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to parse response: {e.Message}\nResponse: {responseText}");
                }
            }
        }
    }

    private static string GetHistoryText(List<Replique> history)
    {
        string historyText = string.Empty;
        //historyText += "Так же вот история нашего с тобой прошлого общения,обращайся к ней каждый раз как будешь обдумывать ответ И УЧИТЫВАЙ её при ответе на вопрос";
        foreach (var replique in history)
        {
            historyText += $" {replique.Sender} : {replique.Text}.";
        }
        return historyText;
    }
    public static IEnumerator SendRequest(string userPrompt,string CharacterInfo,List<Replique> history,Action<string> callback) // meta-llama/llama-4-maverick-17b-128e-instruct  ||  deepseek-r1-distill-llama-70b
    {
        string jsonBody = $@"{{
            ""model"": ""meta-llama/llama-4-maverick-17b-128e-instruct"",
            ""messages"": [
                {{
                    ""role"": ""user"",
                    ""content"": ""{CharacterInfo.Replace("\"", "\\\"").Replace("$$$",userPrompt.Replace("\"", "\\\"")) + (history.Count > 0 ? GetHistoryText(history) : string.Empty) }""
                }}
            ]
            
        }}";
        //,"reasoning_format": "hidden"
        using (UnityWebRequest request = new UnityWebRequest(API_URL, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {request.error} - {request.downloadHandler.text}");
            }
            else
            {
                string responseText = request.downloadHandler.text;
                try
                {
                    var jsonResponse = JsonUtility.FromJson<GroqResponse>(responseText);
                    string content = jsonResponse.choices[0].message.content;

                    string cleanedContent = Regex.Replace(content, @"<think>.*?</think>", "", RegexOptions.Singleline);
                    cleanedContent = cleanedContent.Trim();

                    Debug.Log($"{cleanedContent}");
                    callback?.Invoke(cleanedContent.Replace('"',' ').Replace('\n', ' '));
                    //Debug.Log($"Full Response (for debugging): {responseText}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to parse response: {e.Message}\nResponse: {responseText}");
                }
            }
        }
    }

    [System.Serializable]
    private class GroqResponse
    {
        public Choice[] choices;
    }

    [System.Serializable]
    private class Choice
    {
        public Message message;
    }

    [System.Serializable]
    private class Message
    {
        public string content;
    }
}