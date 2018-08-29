using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuperSocket.ClientEngine;
using WebSocket4Net;

using System.IO;

// Login:           1
// Logout:          2
// Authorize:       3
// QueryHeadsets:   4
// CreateSession:   5
// Subscribe:       6
// QuerySessions:   7
// From Cortex example: login, authenticate, query headset, create session, subscribe.

namespace CortexTest
{
    class CortexManager
    {
        WebSocket ws;

        string username;
        string password;
        string license;
        string client_id;
        string client_secret;

        string fileName;
        string fileLocation;
        string filePath;

        string token;

        public void Init ()
        {
            Console.WriteLine("CortexManager start.");
            Console.WriteLine();

            // Set up credentials.
            username = "asntchua";
            password = "Hesl1234";
            license = "a69442ad-6760-4d6d-9d36-0ad59bd45750";
            client_id = "sw37ALQ0sEXese8dA723pqracKIy49OSsYpCWezQ";
            client_secret = "Yt3BC6hXbJTwDneJOEjaXu2Q8rBEtjZsKG6Ria2E2rkTUUq795Uk0vroszf54J6OFoVHWfio8q8wJEUfyhaUoujCDDkrUHBjvW7xpEg5krg7wXofxu4Nxm5ouQGXdo6Y";

            fileName = "cortex.csv";
            fileLocation = @"C:\Users\nrobinson\Desktop\Chester\";
            filePath = fileLocation + fileName;

            // Create websocket.
            ws = new WebSocket("wss://emotivcortex.com:54321");

            // Websocket events.
            ws.Opened += new EventHandler(WebSocketOpened);
            ws.Closed += new EventHandler(WebSocketClosed);
            ws.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(WebSocketError);
            ws.MessageReceived += new EventHandler<MessageReceivedEventArgs>(WebSocketMessageReceived);

            Thread.Sleep(5000);
            Console.WriteLine("WebSocket event handling done.");
            Console.WriteLine();

            Console.WriteLine("Calling WebSocket.Open().");
            ws.Open();
            Console.WriteLine("WebSocket.Open() called.");
            Console.WriteLine();

            Thread.Sleep(5000);
            Logout();

            Thread.Sleep(5000);
            Login();

            Thread.Sleep(5000);
            Authorize();

            // Thread.Sleep(5000);
            // QueryHeadsets();

            Thread.Sleep(5000);
            CreateSession();
            // QuerySessions();

            Thread.Sleep(5000);
            Subscribe();

            Thread.Sleep(5000);
            Console.WriteLine("Calling WebSocket.Close().");
            ws.Close();
            Console.WriteLine("WebSocket.Close() called.");
            Thread.Sleep(10000);
        }

        public void WebSocketOpened (object sender, EventArgs e)
        {
            Console.WriteLine("WebSocket connection opened.");
        }

        public void WebSocketClosed (object sender, EventArgs e)
        {
            Console.WriteLine("WebSocket connection closed.");
        }

        public void WebSocketError(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            Console.WriteLine("Error Occurred.");
        }

        public void WebSocketMessageReceived (object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine("Message received.");
            JObject response = JObject.Parse(e.Message.ToString());
            Console.WriteLine(response.ToString());
            
            // Response from Authorize() request.
            // Getting authentication token.
            if (response.ToString().Contains("jsonrpc") && Int32.Parse(response["id"].ToString()) == 3)
            {
                this.token = response["result"]["_auth"].ToString();
            }

            // Response from QueryHeadsets() request.
            // Getting headset id.
            //if (Int32.Parse(response["id"].ToString()) == 4)
            //{
            //    this.headset_id = response["result"][0]["id"].ToString();
            //}

            // Handling streaming EEG data.
            if (!response.ToString().Contains("jsonrpc") && response.ToString().Contains("eeg"))
            {
                TextWriter writer = new StreamWriter(filePath, true);
                writer.Write(response["eeg"][0].ToString() + ",");
                writer.Write(response["eeg"][1].ToString() + ",");
                writer.Write(response["eeg"][2].ToString() + ",");
                writer.Write(response["eeg"][3].ToString() + ",");
                writer.Write(response["eeg"][4].ToString() + ",");
                writer.Write(response["eeg"][5].ToString() + ",");
                writer.Write(response["eeg"][7].ToString() + ",");
                writer.Write(response["eeg"][8].ToString() + ",");
                writer.Write(response["eeg"][9].ToString() + ",");
                writer.Write(response["eeg"][10].ToString() + ",");
                writer.Write(response["eeg"][11].ToString() + ",");
                writer.Write(response["eeg"][12].ToString() + ",");
                writer.Write(response["eeg"][13].ToString() + ",");
                writer.Write(response["eeg"][14].ToString() + ",");
                writer.Write(response["eeg"][15].ToString() + ",");
                writer.Write(response["eeg"][16].ToString() + ",");
                writer.Write(response["eeg"][17].ToString() + ",");
                writer.Write(response["eeg"][18].ToString() + ",");
                writer.Write(response["eeg"][19].ToString() + ",");
                writer.WriteLine();
                writer.Close();
            }

            // Response from QuerySessions() request.
            if (response.ToString().Contains("jsonrpc") && Int32.Parse(response["id"].ToString()) == 7)
            {
                string file = @"C:\Users\nrobinson\Desktop\Chester\QuerySessions.txt";

                TextWriter writer = new StreamWriter(file, false);
                writer.WriteLine(response.ToString());
                writer.Close();
            }

            Console.WriteLine();
        }

        public void Login ()
        {
            Console.WriteLine("Login() called.");

            JObject request = new JObject();
            JProperty jsonrpc = new JProperty("jsonrpc", "2.0");
            request.Add(jsonrpc);
            JProperty method = new JProperty("method", "login");
            request.Add(method);

            JObject param = new JObject();
            JProperty username = new JProperty("username", this.username);
            param.Add(username);
            JProperty password = new JProperty("password", this.password);
            param.Add(password);
            JProperty client_id = new JProperty("client_id", this.client_id);
            param.Add(client_id);
            JProperty client_secret = new JProperty("client_secret", this.client_secret);
            param.Add(client_secret);
            request.Add("params", param);

            JProperty id = new JProperty("id", 1);
            request.Add(id);

            Console.WriteLine(request.ToString());
            Console.WriteLine();
            ws.Send(request.ToString());
        }

        public void Logout ()
        {
            Console.WriteLine("Logout() called.");

            JObject request = new JObject();
            JProperty jsonrpc = new JProperty("jsonrpc", "2.0");
            request.Add(jsonrpc);
            JProperty method = new JProperty("method", "logout");
            request.Add(method);

            JObject param = new JObject();
            JProperty username = new JProperty("username", this.username);
            param.Add(username);
            request.Add("params", param);

            JProperty id = new JProperty("id", 2);
            request.Add(id);

            Console.WriteLine(request.ToString());
            Console.WriteLine();
            ws.Send(request.ToString());
        }

        // Get authentication token.
        public void Authorize ()
        {
            Console.WriteLine("Authorize() called.");

            JObject request = new JObject();
            JProperty jsonrpc = new JProperty("jsonrpc", "2.0");
            request.Add(jsonrpc);
            JProperty method = new JProperty("method", "authorize");
            request.Add(method);

            JObject param = new JObject();
            JProperty username = new JProperty("client_id", this.client_id);
            param.Add(username);
            JProperty password = new JProperty("client_secret", this.client_secret);
            param.Add(password);
            JProperty client_id = new JProperty("license", this.license);
            param.Add(client_id);
            request.Add("params", param);

            JProperty id = new JProperty("id", 3);
            request.Add(id);

            Console.WriteLine(request.ToString());
            Console.WriteLine();
            ws.Send(request.ToString());
        }

        // Query all headsets.
        public void QueryHeadsets ()
        {
            Console.WriteLine("QueryHeadsets() called.");

            JObject request = new JObject();
            JProperty jsonrpc = new JProperty("jsonrpc", "2.0");
            request.Add(jsonrpc);
            JProperty method = new JProperty("method", "queryHeadsets");
            request.Add(method);

            JObject param = new JObject();
            request.Add("params", param);

            JProperty id = new JProperty("id", 4);
            request.Add(id);

            Console.WriteLine(request.ToString());
            Console.WriteLine();
            ws.Send(request.ToString());
        }

        // Create active session on default headset.
        public void CreateSession ()
        {
            Console.WriteLine("CreateSession() called.");

            JObject request = new JObject();
            JProperty jsonrpc = new JProperty("jsonrpc", "2.0");
            request.Add(jsonrpc);
            JProperty method = new JProperty("method", "createSession");
            request.Add(method);

            JObject param = new JObject();
            JProperty _auth = new JProperty("_auth", this.token);
            param.Add(_auth);
            JProperty status = new JProperty("status", "active");
            param.Add(status);
            request.Add("params", param);

            JProperty id = new JProperty("id", 5);
            request.Add(id);

            Console.WriteLine(request.ToString());
            Console.WriteLine();
            ws.Send(request.ToString());
        }

        // Subscribe only EEG.
        public void Subscribe ()
        {
            Console.WriteLine("Subscribe() called.");

            JObject request = new JObject();
            JProperty jsonrpc = new JProperty("jsonrpc", "2.0");
            request.Add(jsonrpc);
            JProperty method = new JProperty("method", "subscribe");
            request.Add(method);

            JObject param = new JObject();
            JProperty _auth = new JProperty("_auth", this.token);
            param.Add(_auth);
            string[] array = new string[1];
            array[0] = "eeg";
            JProperty streams = new JProperty("streams", array);
            param.Add(streams);
            request.Add("params", param);

            JProperty id = new JProperty("id", 6);
            request.Add(id);

            Console.WriteLine(request.ToString());
            Console.WriteLine();
            ws.Send(request.ToString());
        }

        // Query all sessions.
        public void QuerySessions ()
        {
            Console.WriteLine("QuerySessions() called.");

            JObject request = new JObject();
            JProperty jsonrpc = new JProperty("jsonrpc", "2.0");
            request.Add(jsonrpc);
            JProperty method = new JProperty("method", "querySessions");
            request.Add(method);

            JObject param = new JObject();
            JProperty _auth = new JProperty("_auth", this.token);
            param.Add(_auth);
            request.Add("params", param);

            JProperty id = new JProperty("id", 7);
            request.Add(id);

            Console.WriteLine(request.ToString());
            Console.WriteLine();
            ws.Send(request.ToString());
        }
    }
}
