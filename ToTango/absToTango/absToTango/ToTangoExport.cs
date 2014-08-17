﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace absToTango
{
    /// <summary>
    /// Provides an easy way to export ToTango filters 
    /// </summary>
    public class ToTangoExport
    {
        private string _token;
        private string _headers = "";
        private string _aliasHead = "";

        /// <summary>
        /// Initializes a new instance of the ToTangoExport class.
        /// </summary>
        /// <param name="token">Your ToTango API authentication key.</param>        
        /// <param name="headerFile">Your Mapping file.</param>
        public ToTangoExport(string token, string headerFile)
        {            
            this._token = token;
            foreach (string line in File.ReadAllLines(headerFile))
            {
                if ((line.Trim().Length > 0) && (!line.StartsWith("#")))
                {
                    if (this._headers == "")
                    {
                        this._headers = line.Replace("\t", "");
                        this._aliasHead = this._headers;
                    }
                    else
                    {
                        this._aliasHead = line.Replace("\t", "");
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Start the export on the given url
        /// </summary>
        /// <param name="url">The Url to the API, something like: https://app.totango.com/api/v1/accounts/active_list/10010/current.json</param>        
        /// <param name="outname">The name of the output file</param> 
        public string Start(string url, string outname)
        {
            List<string> lines = new List<string>();
            lines.Add(this._aliasHead);
            if (this._headers.Length > 0)
            {
                using (ToTangoReader tangoReader = new ToTangoReader(this._token, url))
                {
                    dynamic account = null;
                    do
                    {
                        account = tangoReader.ReadAccount();
                        if (account != null)
                            lines.Add(concatAttribs(account));
                    }
                    while (account != null);
                }
            }
            outname = newName(outname);
            File.WriteAllLines(outname, lines);
            return outname;
        }

#region "  Private Methods  "

        private string newName(string outname)
        {
            string dName = outname;
            int i = 0;
            while (File.Exists(outname))
            {
                string ext = Path.GetExtension(outname);
                outname = dName.Replace(ext, "") + i.ToString() + ext;
                i++;
            }
            return outname;
        }
        
        private string concatAttribs(dynamic account)
        {
            string line = "";
            foreach (string attrib in this._headers.Split(','))
            {
                line += getAttrib(account, attrib) + ",";
            }
            return line.TrimEnd(',');
        }

        private string getAttrib(dynamic account, string attrib)
        {
            string ret = "";
            attrib = attrib.Trim();
            if (account.attributes[attrib] != null)
            {
                ret = account.attributes[attrib].value;
            }
            return ret;
        }

#endregion
    }
}