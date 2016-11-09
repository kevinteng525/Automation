using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Saber.Common
{
    // to do : add common file operation function
    public class FileHelper
    {
        // to do : add common function: rename,copy,move,delete.

        public void ChangeFilesExtensionInFolder(string dir, string pattern, string newExt)
        {
            try
            {
                foreach (string d in Directory.GetFileSystemEntries(dir))
                {
                    if (File.Exists(d))
                    {
                        if (String.Equals(Path.GetExtension(d).ToString(), pattern))
                        {
                            FileInfo fi = new FileInfo(d);
                            Console.WriteLine("File names: " + fi.Name.ToString());
                            String newFilePath = Path.ChangeExtension(d, newExt);
                            Console.WriteLine("newFile path is: " + newFilePath);

                            fi.MoveTo(newFilePath);
                            Console.WriteLine("File extension changed: " + fi.Name.ToString());
                        }

                    }
                    else
                    {
                        DirectoryInfo d1 = new DirectoryInfo(d);
                        if (d1.GetFiles().Length != 0 || d1.GetDirectories().Length != 0)
                        {
                            ChangeFilesExtensionInFolder(d1.FullName, pattern, newExt);
                        }

                        //Directory.Delete(d, true);
                        Console.WriteLine("Directory used: " + d);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e);
            }
            finally
            {
            }

        }

        public void DeleteFolder(string dir)
        {
            try
            {
                foreach (string d in Directory.GetFileSystemEntries(dir))
                {
                    if (File.Exists(d))
                    {
                        FileInfo fi = new FileInfo(d);
                        if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                            fi.Attributes = FileAttributes.Normal;
                        File.Delete(d);
                        Console.WriteLine("File deleted: " + d);
                    }
                    else
                    {
                        DirectoryInfo d1 = new DirectoryInfo(d);
                        if (d1.GetFiles().Length != 0 || d1.GetDirectories().Length != 0)
                        {
                            DeleteFolder(d1.FullName);
                        }

                        Directory.Delete(d, true);
                        Console.WriteLine("Directory deleted: " + d);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e);
            }
        }
    }
}
