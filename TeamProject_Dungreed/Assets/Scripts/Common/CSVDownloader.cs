using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Net;
using System.IO;

public static class CSVDownloader
{
    private const string k_googleSheetDocID = "14Vw8mO1oMelucsFIbJA7LqarFz15DEUnzE1WS1zCrK0";
    private const string sheetNumber = "0";
    private const string url = "https://docs.google.com/spreadsheets/d/" + k_googleSheetDocID + "/export?format=csv&id=" + k_googleSheetDocID + "&gid=" + sheetNumber;

    //https://docs.google.com/spreadsheets/d/14Vw8mO1oMelucsFIbJA7LqarFz15DEUnzE1WS1zCrK0/export?format=csv&id=14Vw8mO1oMelucsFIbJA7LqarFz15DEUnzE1WS1zCrK0&gid=1428965408
    internal static IEnumerator DownloadData(System.Action<string> onCompleted)
    {
        yield return new WaitForEndOfFrame();

        string downloadData = null;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if(webRequest.isNetworkError)
            {
                Debug.Log(" Download Error : " + webRequest.error);
            }
            else
            {
                Debug.Log(" Download secess");
                //Debug.Log(" Data : " + webRequest.downloadHandler.text);

                string versionSection = webRequest.downloadHandler.text.Substring(0, 5);
                int equalsIndex = versionSection.IndexOf('=');
                UnityEngine.Assertions.Assert.IsFalse(equalsIndex == -1, "Could not find a '=' at the start of the CSV");

                string versionText = webRequest.downloadHandler.text.Substring(0, equalsIndex);
                Debug.Log(" Download data version : " + versionText);

                downloadData = webRequest.downloadHandler.text.Substring(equalsIndex + 1);
            }
        }

        onCompleted(downloadData);
    }

    internal static IEnumerator DownloadData(string googleSheetDocID, string sheetId, System.Action<string> onCompleted)
    {
        string sheetUrl = "https://docs.google.com/spreadsheets/d/" + googleSheetDocID + "/export?format=csv&id=" + googleSheetDocID + "&gid=" + sheetId;

        yield return new WaitForEndOfFrame();

        string downloadData = null;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(sheetUrl))
        {

            yield return webRequest.SendWebRequest();

            Debug.Log(webRequest.isDone);

            if (webRequest.isNetworkError)
            {
                Debug.Log(" Download Error : " + webRequest.error);
            }
            else
            {
                Debug.Log(" Download secess");
                Debug.Log(" Data : " + webRequest.downloadHandler.text);

                string versionSection = webRequest.downloadHandler.text.Substring(0, 5);
                int equalsIndex = versionSection.IndexOf('=');
                UnityEngine.Assertions.Assert.IsFalse(equalsIndex == -1, "Could not find a '=' at the start of the CSV");

                string versionText = webRequest.downloadHandler.text.Substring(0, equalsIndex);
                Debug.Log(" Download data version : " + versionText);

                downloadData = webRequest.downloadHandler.text.Substring(equalsIndex + 1);
            }
        }

        onCompleted(downloadData);
    }

    internal static bool DownloadData(string googleSheetDocID, string sheetId, out string result)
    {
        string sheetUrl = "https://docs.google.com/spreadsheets/d/" + googleSheetDocID + "/export?format=csv&id=" + googleSheetDocID + "&gid=" + sheetId;

        System.Net.ServicePointManager.ServerCertificateValidationCallback += ForceCallBack;
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(sheetUrl);

        HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
        StreamReader sr = new StreamReader(resp.GetResponseStream());
        result = sr.ReadToEnd();
        sr.Close();
        ServicePointManager.ServerCertificateValidationCallback -= ForceCallBack;

        if(result == null || result == "")
        {
            Debug.Log("No Data");
            result = "";
            return false;
        }

        Debug.Log("Data : " + result);
        return true;
    }

    private static bool ForceCallBack(object sender, System.Security.Cryptography.X509Certificates.X509Certificate ceritificate,
        System.Security.Cryptography.X509Certificates.X509Chain chain,
        System.Net.Security.SslPolicyErrors sslPilicyErrors)
    {
        return true;
    }
    

}