using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesManagment
{
    internal abstract class AD_File
    {
        protected string fileName;
        protected DateTime lastUpdateTime = new DateTime(); //assigns default value 01/01/0001 00:00:00

        public AD_File(string fileName)
        {
            FileName = fileName;
            LastUpdateTime = lastUpdateTime; 
        }

        public string FileName
        {
            get { return fileName; }
            set
            {
                int i = 0;
                char c = value[i];

                while (i < value.Length)
                {
                    if (value[i] == '<' || value[i] == '?' || value[i] == '*' || value[i] == ':' || value[i] == '/' || value[i] == '\\' || value[i] == '|' || value[i] == '>')
                    {
                        throw new Exception("Error! A file name can't contain any of the following characters:\\ / : * ? \" < > | ");
                    }
                    i++;
                }

                if (value == null)
                {
                    throw new Exception("Error! cannot except null references, you must write file name");
                }

                fileName = value;
            }
        }
        public DateTime LastUpdateTime
        {
            get { return lastUpdateTime; }
            set { lastUpdateTime = DateTime.Now; }
        }

        public override string ToString()
        {
            return fileName + " " + lastUpdateTime;
        }

        public override bool Equals(object file2)
        {
            AD_File tmp = file2 as AD_File;
            if (tmp == null)
                throw new FormatException();
            if (tmp.FileName.Equals(this.fileName)) return true;
            else return false;
        }
        public abstract int GetSize(); // supposed to return the file size (in bytes)

    }
}
