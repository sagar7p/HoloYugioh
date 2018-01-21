using UnityEngine;
using System.Threading;
using System.Net;
using System.Collections;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;


public delegate void ChangedEventHandler(object sender, GenericArgs e);

public class GenericArgs
{
    public JSONObject data;
}

public class FirebaseDatabase
{

    public event ChangedEventHandler Changed;

    public void AddObserver(CancellationToken token)
    {
        GetAndProcessFirebaseHttpResponse(token);
    }

    private void GetAndProcessFirebaseHttpResponse(CancellationToken cancellationToken)
    {
        var result = Task.Run(() =>
        {
            var httpResponse = ListenAsync().Result;
            using (httpResponse)
            {
                using (Stream contentStream = httpResponse.GetResponseStream())
                {
                    using (StreamReader contentStreamReader = new StreamReader(contentStream))
                    {

                        while (true)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            string read = contentStreamReader.ReadLineAsync().Result;
                            Debug.Log("FIREBASE: " + read);
                            if (read.Contains("{"))
                            {
                                read = read.Substring(6);
                                JSONObject obj = new JSONObject(read);
                                var string_path = obj["path"].str;
                                string_path = string_path.Substring(1);
                                var data = obj["data"];
                                GenericArgs args = new GenericArgs();
                                args.data = data;
                                Changed?.Invoke(string_path, args);
                            }
                        }
                    }
                }
            }
        },cancellationToken);

    }



    private async Task<HttpWebResponse> ListenAsync()
    {
        var _firebasePath = "https://holoyugioh.firebaseio.com/.json";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_firebasePath);
        request.Method = "GET";
        request.AllowAutoRedirect = true;
        request.Timeout = TimeSpan.FromSeconds(10).Milliseconds;
        request.Accept = "text/event-stream";

        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

        HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
        return response;
    }

    public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool isOk = true;
        // If there are errors in the certificate chain,
        // look at each error to determine the cause.
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    continue;
                }
                chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                bool chainIsValid = chain.Build((X509Certificate2)certificate);
                if (!chainIsValid)
                {
                    isOk = false;
                    break;
                }
            }
        }
        return isOk;
    }
}
