using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            
            //Start server
            // 1) Make server object on port 1000
            // 2) Start Server
            CreateRedirectionRulesFile(); 
            Server system = new Server(1000, "Redirectionrules.txt");
            system.StartServer();
        }

        static void CreateRedirectionRulesFile()
        {
            // TODO: Create file named redirectionRules.txt
            // each line in the file specify a redirection rule
            // example: "aboutus.html,aboutus2.html"
           // means that when making request to aboustus.html,, it redirects me to aboutus2
            
            //  TextWriter redirection = new StreamWriter("Redirectionrules.txt",true);
             // redirection.WriteLine("aboutus.html,aboutus2.html"); 
            string redirection_rules = "aboutus.html,aboutus2.html";
            System.IO.File.WriteAllText("Redirectionrules.txt", redirection_rules);
        }
         
    }
}
