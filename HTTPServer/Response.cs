using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            //throw new NotImplementedException();
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            DateTime currentdate = DateTime.Now; 
            headerLines.Add(contentType);
            headerLines.Add(content.Length.ToString());
            headerLines.Add(currentdate.ToString());
            string headerlines_out;
            if (redirectoinPath != null)
            {
                headerLines.Add(redirectoinPath);
                headerlines_out = "Content-Type: " + headerLines[0] + "\r\n" + "Content-Length: " + headerLines[1] + "\r\n" + "Date: " + headerLines[2] + "\r\n" + "location: " + headerLines[3] + "\r\n" +"\r\n" + content + "\r\n";
            }
            else
                headerlines_out = "Content-Type: " + headerLines[0] + "\r\n" + "Content-Length: " + headerLines[1] + "\r\n" + "Date: " + headerLines[2] + "\r\n"+ "\r\n" + content + "\r\n";
            
                // TODO: Create the response string
            if (code == StatusCode.OK)
                responseString = GetStatusLine(code) + " OK" + "\r\n" + headerlines_out;
            else if (code == StatusCode.NotFound)
                responseString = GetStatusLine(code) + " Not Found" + "\r\n" + headerlines_out;
            else if (code == StatusCode.BadRequest)
                responseString = GetStatusLine(code) + " Bad Request" + "\r\n" + headerlines_out;
            else if (code == StatusCode.InternalServerError)
                responseString = GetStatusLine(code) + " Internal Server Error" + "\r\n" + headerlines_out;
            else if (code == StatusCode.Redirect)
                responseString = GetStatusLine(code) + " Redirect" + "\r\n" + headerlines_out;
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = Configuration.ServerHTTPVersion + " " + ((int)code) + " ";
            return statusLine; 
                //string.Empty;
        }
    }
}
