using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EMC.SourceOne.RBSProvider.ProviderConfig;

namespace RBSAppNET35
{
    public class Program
    {
        /// <summary>
        /// SharePoint 2010 is running under NET3.5 
        ///   the compression and encryption result sometimes are not the same between NET3.5 and NET4.0
        /// Wrote an App to do this
        /// </summary>
        static void Main(string[] args)
        {
            //
            // compress encrypt
            // -ce -c true -e AES256 -src C:\src.txt -dest D:\dest.bin
            //
            // decompress decrypt
            // -dd -c true -e AES256 -src C:\src.txt -dest D:\dest.bin
            //
            try
            {
                if (args.Length == 0)
                    throw new ArgumentException("need parameters");

                bool compressAndEncrypt = false;
                bool decryptAndDecompress = false;

                bool compress = false;
                EncryptionType encrypt = EncryptionType.None;

                string src = null, dest = null;

                for(int i = 0; i < args.Length; ++i)
                {
                    string arg = args[i];
                    switch (arg.ToLower())
                    {
                        case "-ce":
                            compressAndEncrypt = true;
                            break;
                        case "-dd":
                            decryptAndDecompress = true;
                            break;
                        case "-c":
                            ++i;
                            if (i < args.Length)
                            {
                                string value = args[i];
                                if (bool.TryParse(value, out compress) == false)
                                    throw new ArgumentException("the '-c' only accept 'true' or 'false'.");
                            }
                            else
                            {
                                throw new ArgumentException("the '-c' needs parameter.");
                            }
                            break;
                        case "-e":
                            ++i;
                            if (i < args.Length)
                            {
                                string value = args[i];
                                try
                                {
                                    encrypt = (EncryptionType)Enum.Parse(typeof(EncryptionType), value, true);
                                }
                                catch (Exception)
                                {
                                    throw new ArgumentException("the '-e' only accept None, AES128, AES256, DES64, RC2_128, TripleDES128, TripleDES192 ...");
                                }
                            }
                            else
                            {
                                throw new ArgumentException("the '-e' needs parameter.");
                            }
                            break;
                        case "-src":
                            ++i;
                            if (i < args.Length)
                            {
                                string value = args[i];
                                src = value;
                            }
                            else
                            {
                                throw new ArgumentException("the '-src' needs parameter.");
                            }
                            break;
                        case "-dest":
                            ++i;
                            if (i < args.Length)
                            {
                                string value = args[i];
                                dest = value;
                            }
                            else
                            {
                                throw new ArgumentException("the '-dest' needs parameter.");
                            }
                            break;
                        default:
                            throw new ArgumentException(string.Format("the '{0}' is not support.", arg));
                    }
                }

                if (compressAndEncrypt)
                {
                    CompressAndEncryptHelper.CompressAndEncryptFile(src, compress, encrypt, ref dest);
                }
                else if (decryptAndDecompress)
                {
                    CompressAndEncryptHelper.DecryptAndDecompressFile(src, compress, encrypt, ref dest);
                }
            }
            catch (ArgumentException aex)
            {
                PrintHelp(aex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void PrintHelp(string message)
        {
            Console.WriteLine(string.Format(@"
{0}

e.g.
  compress encrypt
    -ce -c true -e AES256 -src C:\src.txt -dest D:\dest.bin
  decompress decrypt
    -dd -c true -e AES256 -src C:\src.txt -dest D:\dest.bin
            ", message));
        }
    }
}
