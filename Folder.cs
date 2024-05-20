using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

namespace FilesManagment
{
    internal class Folder : AD_File, IComplete
    {
        string path;

        AD_File[] arr;
        static Folder mainFolder = new Folder("","root");
        public Folder(string path, string fileName) : base(fileName)
        {
            Path = path;
            arr = new AD_File[0];
        }

        public AD_File[] Arr
        {
            get { return arr; }
        }

        public string Path
        { 
            get { return path; }
            set { path = value + "\\" + FileName; }
        }

        public static Folder MainFolder
        {
            get { return mainFolder; }
            set { mainFolder = value; }
        }

        public string GetFullPath()
        {
            return path;
        }

        public override int GetSize()
        {
            int counter = 0;

            foreach (AD_File file in arr)
            {
                if (file is Folder)
                {
                    file.GetSize();
                }

                if (file is DataFile)
                {
                    counter += ((DataFile)file).GetSize();
                }
            }
            return counter;
        }

        public bool IsFull(uint num)
        {
            int SizeOfOccupancy = this.GetSize();
            if (num - SizeOfOccupancy == 0) // cannot be a negative number ever
            {
                return true;
            }

            else return false;
        }

        public void AddFileToArray(AD_File file)
        {
            if (arr.Length == 0) // the array still have no objects
            {
                arr = new AD_File[1];
                arr[0] = file;
                return;
            }

            else // the array does have one object or more
            {
                if (file is Folder)
                {
                    foreach (AD_File f in arr)
                    {
                        if (f is Folder)
                        {
                            if (f.FileName == file.FileName)
                            {
                                throw new Exception("folder already existes");
                            }
                        }

                        if (f is DataFile)
                        {
                            continue;
                        }
                    }
                }


                if (file is DataFile)
                {
                    foreach (AD_File f in arr)
                    {
                        if (f is Folder)
                        {
                            continue;
                        }

                        if (f is DataFile)
                        {
                            if (f.FileName == file.FileName)
                            {
                                throw new Exception("file already existes");
                            }
                        }
                    }
                }

                Array.Resize(ref arr, arr.Length + 1);
                arr[arr.Length - 1] = file;
                return;

            }
        }

        public DataFile MkFile(string name, string data) // MAKING NEW DATA FILE, AND ADD IT TO THE ARRAY
        {
            DataFile newDataFile = new DataFile(name, data);
            AddFileToArray(newDataFile);
            return newDataFile;
        }

        public void MkDir(string name) // MAKING NEW FOLDER, AND ADD IT TO THE ARRAY
        {
            Folder newFolder = new Folder(path, name);
            AddFileToArray(newFolder);
        }
        public override string ToString() // PRINT FILES DETAILS
        {
            string str = "";

            foreach (AD_File f in arr)
            {
                if (f is Folder)
                {
                    str += f.ToString() + " <DIR>\n";
                }

                else
                {
                    str += ((DataFile)f).ToString() + "\n";
                }
            }
            return str;
        }

        public static Folder Cd(string path) // DIVE IN ALL THE HIERARCHY TILL FOUND THE REQUIRED FOLDER
        {
            string[] FoldersFromPath = path.Split('\\'); // holds list of the folders names

            AD_File[] tmpArr = mainFolder.arr; // start from root

            AD_File next = null;

            foreach (string folder in FoldersFromPath)
            {
                next = SearchFile(folder, tmpArr);

                if (next == null)
                {
                    throw new Exception("The folder doesnt exists in this path");
                }

                if ((((Folder)next).Arr).Length != 0)
                {
                    tmpArr = ((Folder)next).Arr;
                }
            }

           return (Folder)next;
        }

        public static AD_File SearchFile(string name, AD_File[] currArr) // helping method for search the file from path in the current files array
        {
            foreach (AD_File file in currArr)
            {
                Folder f = file as Folder;
                if (f != null)
                {
                    if ((file.FileName).Equals(name))
                    {
                        return file;
                    }
                }
                else { continue; }
            }
            return null;
        }

        public override bool Equals(object file) // EQUALIZATION OF TWO FILES (ONE IS THE STATER METHOD, THE OTHER IS THE ARGUMENT)
        {
            AD_File[] Files1 = null;
            AD_File[] Files2 = null;

            Folder folder = file as Folder;

            if (folder != null) // האובייקט שבארגומנט הוא תיקייה
            {
                if (this.Arr.Length == ((Folder)file).Arr.Length) // בדיקת אורך מערך תיקיות - בדיקת מבנה
                {
                    Files1 = this.Arr; // מערך של התיקייה המפעילה את המתודה
                    Files2 = ((Folder)file).Arr; //  מערך של התיקייה שבארגומנט במתודה

                    for (int i = 0; i < Files1.Length; i++)
                    {
                        if (Files1[i] is DataFile) // במקרה של קובץ תוכן
                        {
                            if (Files2[i] is DataFile)
                            {
                                return ((DataFile)Files1[i]).Equals(Files2[i]); // השוואת שם ותוכן עי מתודת השוואה של קבצי תוכן- בן
                            }
                        }

                        else // במקרה של תיקייה
                        {
                            if ((Files1[i]).Equals(Files2[i])) // השוואת שמות התיקיות עי מתודת השוואה של האבא
                            {
                                Folder folderFromArr = (Folder)Files1[i];
                                bool res = folderFromArr.Equals(Files2[i]); // רקורסיה
                                if (res == false)
                                { return false; }
                                if (res == true)
                                { continue; }
                            }

                            else { return false; }
                        }
                        
                    }
                return true;

                    
                }

                else // מבנה היררכי לא שווה
                {
                    return false;
                }
            }


            else // האובייקט שבארגומנט הוא קובץ תוכן
            {
                return false; 
            }
            
        }

        public bool Fc(string path1, string path2) // LOCALIZE FILE, AS THE PATH OF IT INDICATE. ONCE FOR RESOURCE FILE AND ONCE FOR DESTINATION FILE
        {
            string SourcePath = path1; // the full path
            string DestPath = path2;

            int SourceLastIndexOf = SourcePath.LastIndexOf("\\");
            int DestLastIndexOf = DestPath.LastIndexOf("\\");

            string lastFileInSourcePath = SourcePath.Substring(SourceLastIndexOf + 1); // only last file from path1
            string lastFileInDestPath = DestPath.Substring(DestLastIndexOf + 1);     // only last file from path2

            string cuttedSourcePath = SourcePath.Substring(0, SourceLastIndexOf); // the index indicate the length for getting the path without the last file.
            string cuttedDestPath = DestPath.Substring(0,DestLastIndexOf);

            AD_File SourceFolder = null;
            AD_File DestFolder = null;

            AD_File[] ArrOfSource = (Cd(cuttedSourcePath)).Arr;

            foreach (AD_File adfile in ArrOfSource)
            {
                if (adfile.FileName == lastFileInSourcePath)
                {
                    if (adfile is DataFile)
                    { SourceFolder= (DataFile)adfile; }
                    else { SourceFolder = (Folder)adfile; }
                    continue;
                }
            }

            if (SourceFolder == null)
            {
                throw new Exception("The file doesnt exists");
            }

            AD_File[] ArrOfDest = (Cd(cuttedDestPath)).Arr;

            foreach (AD_File adfile in ArrOfDest)
            {
                if (adfile.FileName == lastFileInDestPath)
                {
                    if (adfile is DataFile)
                    { DestFolder = (DataFile)adfile; }
                    else { DestFolder = (Folder)adfile; }
                    continue;
                }
            }

            if (DestFolder == null)
            {
                throw new Exception("The file doesnt exists");
            }

            return SourceFolder.Equals(DestFolder); // or both folders, or both data files, and going to the equals method according to the files type
        }
    }
}
