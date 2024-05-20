using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesManagment
{
    internal class DataFile : AD_File, IComparable<DataFile>
    {
        string content;

        public string Content
        {
            get { return content; }

            set
            {
                if (value == null)
                {
                    throw new Exception("Error! Cannot except empty data");
                }

                content = value;
            }
        }

        public DataFile(string content, string fileName) : base(fileName)
        {
            this.content = content;
        }

        public override int GetSize() 
        {
            int SizeInB = content.Length;
            return SizeInB;
        }

        public override string ToString()
        {
            return base.ToString() + " " + (float)GetSize() / 1024 + " KB"; 
        }

        public override bool Equals(object file2)
        {
            if (base.Equals(file2))
            {
                DataFile tmp = file2 as DataFile;
                if (tmp != null)
                {
                    return tmp.Content.Equals(this.content);
                }

            }
            return false;
        }

        public int CompareTo(DataFile file2)
        {
            return (this.GetSize().CompareTo(file2.GetSize()));
        }
    }
}
