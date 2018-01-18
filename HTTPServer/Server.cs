using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server  
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            //TODO: initialize this.serverSocket

            this.LoadRedirectionRules(redirectionMatrixPath);
            this.serverSocket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            IPEndPoint server_endpoint = new IPEndPoint(IPAddress.Any, portNumber);
            this.serverSocket.Bind(server_endpoint);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            this.serverSocket.Listen(1000); 
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientsocket = this.serverSocket.Accept();
                Console.WriteLine("New Client Accepted: {0}", clientsocket.RemoteEndPoint);
                Thread client_thread = new Thread(new ParameterizedThreadStart(HandleConnection));
                client_thread.Start(clientsocket);
            }
        }
     
        public void HandleConnection(object obj)
        {
            
            Socket clientsocket = (Socket)obj;
            // TODO: Create client socket 
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clientsocket.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                int recievedlength; 
                try
                {
                    // TODO: Receive request
                    byte [] data = new byte[1024];
                    recievedlength = clientsocket.Receive(data);
                    // TODO: break the while loop if receivedLen==0
                    if (recievedlength == 0)
                    {
                        Console.WriteLine("Client: {0} ended the connection", clientsocket.RemoteEndPoint);
                        break; 
                 
                    }
                    // TODO: Create a Request object using received request string
                    Request client_request = new Request(Encoding.ASCII.GetString(data,0,recievedlength));
                    Response response_object; 
                    // TODO: Call HandleRequest Method that returns the response
                    response_object = HandleRequest(client_request); 
                    // TODO: Send Response back to client

                    byte [] server_response = new byte [1024]; 
                    server_response = Encoding.ASCII.GetBytes (response_object.ResponseString);
                    clientsocket.Send(server_response);
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex); 
                }
            }

            clientsocket.Close();
            // TODO: close client socket
        }
        
        Response HandleRequest(Request request)
        {
            Response server_response;
            string physical_path;
            //throw new NotImplementedException();
            string content;
            try
            {
                bool req_check = request.ParseRequest();
                physical_path = Configuration.RootPath + "\\" + request.relativeURI;
                //TODO: check for bad request 
                if (!req_check)
                {
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    server_response= new Response (StatusCode.BadRequest, "text/html", content, null);
                    return server_response; 
                }
                //TODO: map the relativeURI in request to get the physical path of the resource.

                //TODO: check for redirect
                else if(Configuration.RedirectionRules.ContainsKey(request.relativeURI))
                {
                    string path;
                    path = GetRedirectionPagePathIFExist(request.relativeURI);
                    if (path == string.Empty )
                    {
                        content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                        server_response = new Response(StatusCode.NotFound, "text/html", content, null);

                    }
                    else
                    {
                        Console.WriteLine("Redirected To " + Configuration.RedirectionRules[request.relativeURI]);
                        content = LoadDefaultPage(Configuration.RedirectionDefaultPageName);
                        server_response = new Response(StatusCode.Redirect, "text/html", content, path);
                    }
                     return server_response; 
                }
                //TODO: check file exists
                else if (!File.Exists(physical_path))
                {

                    content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    server_response = new Response(StatusCode.NotFound, "text/html", content, null);
                    return server_response;
                }
                else
                {
                    content = LoadDefaultPage(request.relativeURI);
                    server_response = new Response(StatusCode.OK, "text/html", content, null);
                    return server_response;
                }

                //TODO: read the physical file

                // Create OK response
                
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                physical_path = Configuration.RootPath + "\\" + request.relativeURI;
                Logger.LogException(ex); 
                // TODO: in case of exception, return Internal Server Error. 
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                server_response = new Response(StatusCode.InternalServerError, "text/html", content, null);
                return server_response;
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
           string full_path ;
         full_path= Configuration.RootPath + "\\" + Configuration.RedirectionRules[relativePath];
        if (File.Exists(full_path))
            return Configuration.RedirectionRules[relativePath] ;
        else 
            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            // else read file and return its content
            if (!File.Exists(filePath))
            { 
              Logger.LogException(new Exception());
              return string.Empty;  
            }
            else 
            {
            StreamReader sr = new StreamReader(filePath);
                string line = sr.ReadLine();
                string content = line ; 
                    while (line != null)
                    {
                         
                    line = sr.ReadLine();
                        content += line;
                    }

                    return content; 
           }
           
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                StreamReader sr = new StreamReader(filePath);
                string line = sr.ReadLine();
                Configuration.RedirectionRules = new Dictionary<string, string>(); 
                while (line != null)
                {
                    // then fill Configuration.RedirectionRules dictionary 
                     string[] method = line.Split(',');
                    Configuration.RedirectionRules.Add(method[0], method[1]);
                    line = sr.ReadLine();
                }
               
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
