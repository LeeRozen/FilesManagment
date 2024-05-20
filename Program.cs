using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FilesManagment
{
    internal class Program
    {
        const int capcityRoot = 2000;
        static void Main(string[] args)
        {
            Folder root = Folder.MainFolder;
            Folder curr = root;
            bool quit = false;
            string command, p_command;
            DataFile[] arr = new DataFile[1];

            do
            {
                try
                {
                    Console.Write(curr.GetFullPath().Substring(5) + ">");
                    command = Console.ReadLine();
                    if (command.Substring(0, 2) == "cd")
                    {
                        p_command = command.Substring(3);
                        if (p_command == root.FileName)
                            curr = root;
                        else
                            curr = Folder.Cd(p_command);
                        continue;
                    }
                    if (command.Substring(0, 2) == "FC")
                    {
                        p_command = command.Substring(3,
                        command.Substring(3).IndexOf(' '));
                        command = command.Substring(command.Substring(3).IndexOf(' ') + 4);
                        if (curr.Fc(p_command, command) == true)
                            Console.WriteLine(" equals");
                        else
                            Console.WriteLine("not equals");
                        continue;
                    }
                    if (command == "dir")
                    {
                        Console.WriteLine(curr);
                        continue;
                    }
                    if (command.Substring(0, 5) == "mkdir")
                    {
                        if (root.IsFull(capcityRoot))
                        {
                            Console.WriteLine("The root folder is full, files cannot be added");
                        continue;
                        }
                        p_command = command.Substring(6);
                        curr.MkDir(p_command);
                        continue;
                    }
                    if (command.Substring(0, 4) == "echo")
                    {
                        if (root.IsFull(capcityRoot))
                        {
                            Console.WriteLine("The root folder is full, files cannot be added");
                        continue;
                        }
                        command = command.Substring(5);
                        arr[arr.Length - 1] = curr.MkFile(command.Substring(0,
                        command.IndexOf('>') - 1), command.Substring(command.IndexOf('>') + 2));
                        Array.Resize(ref arr, arr.Length + 1);
                        continue;
                    }
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.Message);
                    quit = true;
                }
                catch (NullReferenceException ex)
                {
                    Console.WriteLine(ex.Message);
                    quit = true;
                }
                catch (Exception exs)
                {
                    Console.WriteLine(exs.Message);
                    quit = true;
                }
            } while (!quit);

            Console.WriteLine("The end commend line!\n");
            Array.Sort(arr);
            if (arr[0] != null)
            { Console.WriteLine("View all content files sorted by file size:"); }

            foreach (DataFile item in arr)
            {
                if (item != null)
                    Console.WriteLine(item);
            }
            Console.ReadKey();
        }

    }
}

