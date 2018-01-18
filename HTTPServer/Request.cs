using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;
        Dictionary<string, string> Content_lines;
        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] request_lines;
        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        
        public bool ParseRequest()
        {
            //throw new NotImplementedException();

            //TODO: parse the receivedRequest using the \r\n delimeter   

           request_lines = this.requestString.Split(new[] { "\r\n" }, StringSplitOptions.None);
            if (request_lines.Length < 3)
                return false;
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)

            // Parse Request line
          
            // Validate blank line exists
            
            // Load header lines into HeaderLines dictionary
            
            if (ParseRequestLine() && ValidateBlankLine() && LoadHeaderLines())
            {
                if(this.method==RequestMethod.POST)
                return LoadContectLines();
                return true;
            }
            return false ; 
        }

        private bool ParseRequestLine()
        {
            
            //throw new NotImplementedException();
            string[] method = request_lines[0].Split(' ');

            if (method[2] == "HTTP/1.1")
            {
                this.httpVersion = HTTPVersion.HTTP11;
            }
            else if (method[2] == "HTTP/1.0")
            {
                this.httpVersion = HTTPVersion.HTTP10;
            }
            else if (method[2] == "HTTP/0.9")
                this.httpVersion = HTTPVersion.HTTP09; 

            if (method[0] == "GET")
                this.method = RequestMethod.GET;
            else if (method[0] == "POST")
                this.method = RequestMethod.POST;
            else if (method[0] == "HEAD")
                this.method = RequestMethod.HEAD;
            else
                return false;


            this.relativeURI = method[1].Remove(0,1);

            return ValidateIsURI(this.relativeURI); 
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
             string [] header;
            string value;
            headerLines = new Dictionary<string, string>();
            bool is_hosted = false;
             for (int i = 1; i < request_lines.Length; i++)
             {
                 if (request_lines[i] == "")
                     break;
                 header = request_lines[i].Split(':');
                if (header[0] == "Host")
                    is_hosted = true;
                value = "";
                for(int j=1;j<header.Length;j++)
                {
                    if (j != 1)
                        value += ":";
                    value += header[j];
                }
                 headerLines.Add(header[0], value);
             }
            return (is_hosted || this.httpVersion==HTTPVersion.HTTP10); 
        }
        public static char HexStringTochar(string hexString)
        {
            byte bytes;
            bytes = Convert.ToByte(hexString, 16);
            char c = Convert.ToChar(bytes);
            return c;
        }
        private bool LoadContectLines()
        {
            bool is_blacnk_line = false;
            for (int i = 1; i < request_lines.Length; i++)
            {
                if(is_blacnk_line)
                {
                    string[] all = request_lines[i].Split('&');
                    if(all.Length>=2)
                    {
                        string[] user_side = all[0].Split('=');
                        string[] pass_side = all[1].Split('=');
                        string user_id = "";
                        if (user_side.Length >=2)
                        user_id = user_side[1];
                        string pass = "";
                        if (pass_side.Length >= 2)
                            pass = pass_side[1];
                        for(int l=0;l<user_id.Length;l++)
                        {
                            if(user_id[l] == '%')
                            {
                                if(l+2 <user_id.Length)
                                {
                                    string numb = "";
                                    numb += user_id[l + 1];
                                    numb += user_id[l + 2];
                                    char c = HexStringTochar(numb);
                                    user_id = user_id.Remove(l, 3);
                                    user_id = user_id.Insert(l, c.ToString());
                                }
                            }
                        }
                        for (int l = 0; l < pass.Length; l++)
                        {
                            if (pass[l] == '%')
                            {
                                if (l + 2 < pass.Length)
                                {
                                    string numb = "";
                                    numb += pass[l + 1];
                                    numb += pass[l + 2];
                                    char c = HexStringTochar(numb);
                                    pass = pass.Remove(l, 3);
                                    pass = pass.Insert(l, c.ToString());
                                }
                                else
                                    return false;
                            }
                        }
                        Console.WriteLine("UserID = "+user_id);
                        Console.WriteLine("Pass = "+pass);
                        Content_lines = new Dictionary<string, string>();
                        Content_lines.Add("UserID", user_id);
                        Content_lines.Add("Pass", pass);
                    }
                    else
                    {
                        return false;
                    }
                }
                if (request_lines[i] == "")
                    is_blacnk_line = true;
            }
            return true;
        }
        private bool ValidateBlankLine()
        {
           // throw new NotImplementedException();
            foreach (string s in request_lines)
            {
                if (s == "")
                    return true;
            }
            return false; 
        
        }

    }
}
