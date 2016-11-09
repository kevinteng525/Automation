using System;
using System.IO;
using System.Text;

namespace RBSLib
{
    public static class FileHelper
    {
        public const int ChunkSize = 32 * 1024;

        public static string GetTempBlob(int length)
        {
            string fileName = Path.GetTempFileName();

            using (FileStream fileStream = File.Create(fileName))
            {
                byte[] buffer = new byte[ChunkSize];
                Random ran = new Random(length);

                int left = length;
                int toWrite = left > ChunkSize ? ChunkSize : left;
                ran.NextBytes(buffer);

                while (left > 0)
                {
                    fileStream.Write(buffer, 0, toWrite);

                    left -= toWrite;
                    toWrite = left > ChunkSize ? ChunkSize : left;
                    ran.NextBytes(buffer); // new bytes
                }
            }

            return fileName;
        }

        public static string GetTempBlob(int length, bool compressable)
        {
            if (compressable == true)
            {
                string fileName = Path.GetTempFileName();

                using (FileStream fileStream = File.Create(fileName))
                {
                    string[] words = new string[] { " . ", "this ", "is ", "a ", "qi ", "test ", "application " };

                    Random ran = new Random(length);
                    int ranValue = ran.Next(0, words.Length);

                    int left = length;
                    while (left > 0)
                    {
                        byte[] bytes = ASCIIEncoding.ASCII.GetBytes(words[ranValue]);

                        int toWrite = bytes.Length > left ? left : bytes.Length;
                        fileStream.Write(bytes, 0, toWrite);

                        left -= toWrite;
                        ranValue = ran.Next(0, words.Length);
                    }
                }
                return fileName;
            }
            else
            {
                return GetTempBlob(length);
            }
        }

        public static bool BinaryCompare(string fileName1, string fileName2)
        {
            bool theSame = true;

            using (FileStream file1 = new FileStream(fileName1, FileMode.Open, FileAccess.Read))
            using (FileStream file2 = new FileStream(fileName2, FileMode.Open, FileAccess.Read))
            {
                if (file1.Length != file2.Length)
                {
                    theSame = false;
                }
                else
                {
                    byte[] buffer1 = new byte[ChunkSize];
                    byte[] buffer2 = new byte[ChunkSize];

                    int read1 = 0, read2 = 0;
                    while ((read1 = file1.Read(buffer1, 0, buffer1.Length)) > 0 &&
                        (read2 = file2.Read(buffer2, 0, buffer2.Length)) > 0)
                    {
                        for (int i = 0; i < read1; ++i)
                        {
                            if (buffer1[i] != buffer2[i])
                            {
                                theSame = false;
                                break;
                            }
                        }

                        if (theSame == false)
                        {
                            break;
                        }
                    }
                }
            }

            return theSame;
        }
    }
}
