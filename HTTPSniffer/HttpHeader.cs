﻿using System;
using System.Text;

namespace HTTPSniffer
{
    public class HttpHeader
    {
        ASCIIEncoding encoding = new ASCIIEncoding();
        
        private string[] m_StrHTTPField = new string[52];

        private byte[] m_byteData = new byte[4096];

        public HttpHeader(string HTTPRequest)
        {
            try
            {
                int IndexHeaderEnd;
                string Header;

                // Si la taille de requête est supérieur ou égale à 1460, alors toutes la chaine est l'entête http
                if (HTTPRequest.Length >= 1460)
                {
                    Header = HTTPRequest;
                }
                else {
                    IndexHeaderEnd = HTTPRequest.IndexOf("\r\n\r\n");
                    Header = HTTPRequest.Substring(0, IndexHeaderEnd);
                    Data = encoding.GetBytes(HTTPRequest.Substring(IndexHeaderEnd + 4));
                }
               
                HTTPHeaderParse(Header);
            }
            catch (Exception)
            { }
        }

        public HttpHeader(byte[] ByteHTTPRequest)
        {
            string HTTPRequest = encoding.GetString(ByteHTTPRequest);
            try
            {
                int IndexHeaderEnd;
                string Header;

                // Si la taille de requête est supérieur ou égale à 1460, alors toutes la chaine est l'entête http
                if (HTTPRequest.Length >= 1460)
                    Header = HTTPRequest;
                else
                {
                    IndexHeaderEnd = HTTPRequest.IndexOf("\r\n\r\n");
                    Header = HTTPRequest.Substring(0, IndexHeaderEnd);
                    Data = encoding.GetBytes(HTTPRequest.Substring(IndexHeaderEnd + 4));
                }

                HTTPHeaderParse(Header);
            }
            catch (Exception)
            { }
        }

        private void HTTPHeaderParse(string Header)
        {
            #region HTTP HEADER REQUEST & RESPONSE

            HTTPHeaderField HHField;
            string HTTPfield, buffer;
            int Index;
            foreach (int IndexHTTPfield in Enum.GetValues(typeof(HTTPHeaderField)))
            {
                HHField = (HTTPHeaderField)IndexHTTPfield;
                HTTPfield = "\n" + HHField.ToString().Replace('_', '-') + ": "; //Ajout de \n devant pour éviter les doublons entre cookie et set_cookie
                // Si le champ n'est pas présent dans la requête, on passe au champ suivant
                Index = Header.IndexOf(HTTPfield);
                if (Index == -1)
                    continue;

                buffer = Header.Substring(Index + HTTPfield.Length);
                Index = buffer.IndexOf("\r\n");
                if (Index == -1)
                    m_StrHTTPField[IndexHTTPfield] = buffer.Trim();
                else
                    m_StrHTTPField[IndexHTTPfield] = buffer.Substring(0, Index).Trim();

                Console.WriteLine("Index = " + IndexHTTPfield + " | champ = " + HTTPfield.Substring(1) + " " + m_StrHTTPField[IndexHTTPfield]);
            }
           
            // Affichage de tout les champs
            /*for (int j = 0; j < m_StrHTTPField.Length; j++)
            {
                HHField = (HTTPHeaderField)j;
                Console.WriteLine("m_StrHTTPField[" + j + "]; " + HHField + " = " + m_StrHTTPField[j]);
            }
            */
            #endregion

        }

        #region [ PROPERTIES ]

        public string[] HTTPField
        {
            get { return m_StrHTTPField; }
            set { m_StrHTTPField = value; }
        }

        public byte[] Data
        {
            get { return m_byteData; }
            set { m_byteData = value; }
        }

        #endregion
    }

    public enum HTTPHeaderField
    {
        Accept = 0,
        Accept_Charset = 1,
        Accept_Encoding = 2,
        Accept_Language = 3,
        Accept_Ranges = 4,
        Authorization = 5,
        Cache_Control = 6,
        Connection = 7,
        Cookie = 8,
        Content_Length = 9,
        Content_Type = 10,
        Date = 11,
        Expect = 12,
        From = 13,
        Host = 14,
        If_Match = 15,
        If_Modified_Since = 16,
        If_None_Match = 17,
        If_Range = 18,
        If_Unmodified_Since = 19,
        Max_Forwards = 20,
        Pragma = 21,
        Proxy_Authorization = 22,
        Range = 23,
        Referer = 24,
        TE = 25,
        Upgrade = 26,
        User_Agent = 27,
        Via = 28,
        Warn = 29,
        Age = 30,
        Allow = 31,
        Content_Encoding = 32,
        Content_Language = 33,
        Content_Location = 34,
        Content_Disposition = 35,
        Content_MD5 = 36,
        Content_Range = 37,
        ETag = 38,
        Expires = 39,
        Last_Modified = 40,
        Location = 41,
        Proxy_Authenticate = 42,
        Refresh = 43,
        Retry_After = 44,
        Server = 45,
        Set_Cookie = 46,
        Trailer = 47,
        Transfer_Encoding = 48,
        Vary = 49,
        Warning = 50,
        WWW_Authenticate = 51
    };
}