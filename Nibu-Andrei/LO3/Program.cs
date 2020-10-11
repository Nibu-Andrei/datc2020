using System;
using System.IO;
using System.Net;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json.Linq;

namespace DATC
{
    class Program
    {
        static string[] Scopes = { DriveService.Scope.Drive };
        static string ApplicationName = "DATC";
        static DriveService Service;
        static UserCredential credential;
        static void Main(string[] args)
        {
            Initialize();
            GetAllFiles();
        }
        static void Initialize(){
            

            using (var stream =
                new FileStream("client-datc.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    Environment.UserName,
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }
            // Create Drive API service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }
        static void GetAllFiles(){
            var request=(HttpWebRequest)WebRequest.Create("https://www.googleapis.com/drive/v3/files?q='root'%20in%20parents");
            request.Headers.Add(HttpRequestHeader.Authorization, "Bearer" + credential.Token.AccessToken);

            using (var response=request.GetResponse())
            {

                using (Stream data=response.GetResponseStream())
                using (var reader=new StreamReader(data))
                {
                    string text=reader.ReadToEnd();
                    var myData=JObject.Parse(text);
                    foreach(var file in myData["files"])
                    {
                        if (file["mimeType"].ToString()!="application/vnd.google-apps.folder")
                        {
                            Console.WriteLine("File name:"+ file["name"]);
                        }
                    }


                }
            }


        }
    }
}
