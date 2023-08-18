using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace ApolloCipher
{
    public class ApolloProgram
    {
        // Static stuff
        string Data = "";
        string Password;

        bool DataEncrypted = false;

        ApolloCipherBlockChain CipherChain;

        byte[] LoadedFileBytes;
        byte Secret1 = 69;
        byte Secret2 = 66;
        byte TerminationByte = 42;

        //TODO: XORGen experiments.
        public static void XORGen()
        {
            byte x1;
            byte x2;
            byte x3;

            //now XOR them. with sauce.
        }

        public ApolloProgram(string data, bool ciphertextData, string password)
        {
            this.Password = password;
            this.DataEncrypted = ciphertextData;
            this.Data = data;
            this.CipherChain = new ApolloCipherBlockChain(data, password, Secret1, Secret2, ciphertextData);
        }

        public ApolloProgram(bool ciphertextData, string password, string filename)
        {
            this.Password = password;
            this.DataEncrypted = ciphertextData;
            this.LoadDataFromFile(filename, ciphertextData);
        }

        public static void Main()
        {

            ConsoleColor fgColor = Console.ForegroundColor;
            ConsoleColor bgColor = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.InputEncoding = Encoding.UTF8;

            string script = "";
            string password = "";
            string ciphertext = "";
            string plaintext = "";
            string userInput = "";

            byte Secret1 = 66;
            byte Secret2 = 69;

            byte[] ciphertextBytes = null;

            bool debug = true;

            ApolloCipher.ApolloCipherBlockChain pChain;


            Console.WriteLine("Please enter the text you want to encrypt and type 'ENDINPUT' on a new line once you have pasted everything:");

            userInput = Console.ReadLine();

            while (userInput != null && !userInput.Equals("ENDINPUT"))
            {
                if (userInput.Equals(""))
                {
                    script += "\n";
                    userInput = Console.ReadLine();
                }
                else
                {
                    script += userInput;
                    userInput = Console.ReadLine();
                }
            }


            Console.WriteLine("Please enter the password you want to use to encrypt this text with:");
            password = Console.ReadLine();

            Console.WriteLine("<-- ENCRYPTION -->");

            pChain = new ApolloCipherBlockChain(script, password, Secret1, Secret2, false);

            ciphertext = pChain.EncryptChain();

            if (debug)
            {
                Console.WriteLine($"Ciphertext is:\n{ciphertext}");
                Console.WriteLine($"Reminder - You previously had set the password to:\n{password}");
                Console.WriteLine("<-- DECRYPTION -->");
            }
            else
            {
                Console.WriteLine(ciphertext);
            }

            plaintext = pChain.DecryptChain();

            if (debug)
            {
                Console.WriteLine($"Decryption reveals the following plaintext:");
                Console.WriteLine(plaintext);
            }

            Console.WriteLine("Hope you enjoyed the demo! Press ENTER to quit.");
            Console.ReadLine();

        }

        byte[] CreateKeyBuffer(string key)
        {
            byte[] buffer = new byte[32];
            byte[] keyArray = Encoding.UTF8.GetBytes(key);

            for (int i = 0; i < buffer.Length - 1; i++)
            {
                // This check makes sure that we write all the bytes of our key into the new buffer we are creating,
                // fills the remaining length with zeroes
                if (i <= keyArray.Length - 1)
                {
                    buffer[i] = keyArray[i];
                }
                else
                {
                    buffer[i] = 0;
                }
            }

            return buffer;
        }
#region WIP
        public static string EncryptData(string script, string password, byte Secret1, byte Secret2)
        {
            ApolloCipherBlockChain pChain = null;
            string IterSubstring = "";

            for (int i = 0; i < script.Length; i += 16)
            {

                // Check for initialized pChain
                if (pChain == null)
                {
                    IterSubstring = script.Substring(i, 16);
                    ApolloCipherBlock pCipherHead = new ApolloCipherBlock(IterSubstring, password, Secret1, Secret2, false);
                    pChain = new ApolloCipherBlockChain(pCipherHead, password, false, Secret1, Secret2);
                    continue;
                }
                else
                {
                    // Try-catch here, in case we overrun the Length of the script.
                    try
                    {
                        IterSubstring = script.Substring(i, 16);
                    }
                    catch (ArgumentOutOfRangeException aore)
                    {
                        IterSubstring = script.Substring(i, (script.Length - 1) - i);
                    }



                    ApolloCipherBlock pCipherHead = new ApolloCipherBlock(IterSubstring, password, Secret1, Secret2, false);
                    pChain.AddBlockToTail(pChain, pCipherHead);
                }
            }
            return pChain.EncryptChain();

        }

        public static string DecryptData(string encrypted, string password, byte Secret1, byte Secret2)
        {
            ApolloCipherBlockChain pChain = null;
            
            for (int i = 0; i < encrypted.Length; i += 16)
            {

                // Check for initialized pChain
                if (pChain == null)
                {
                    ApolloCipherBlock pCipherHead = new ApolloCipherBlock(encrypted.Substring(i, 16), password, Secret1, Secret2, false);
                    pCipherHead.SetCipherTextManual(encrypted.Substring(i, 16));
                    pChain = new ApolloCipherBlockChain(pCipherHead, password, true, Secret1, Secret2);
                    continue;
                }
                else
                {
                    ApolloCipherBlock pCipherHead = new ApolloCipherBlock(encrypted.Substring(i, 16), password, Secret1, Secret2, false);
                    pCipherHead.SetCipherTextManual(encrypted.Substring(i, 16));
                    pChain.AddBlockToTail(pChain, pCipherHead);
                }
            }

            return pChain.DecryptChain();
        }
#endregion
        public string EncryptLoadedData()
        {
            if (this.DataEncrypted == false)
            {
                this.DataEncrypted = true;
                this.Data = CipherChain.EncryptChain();
                return this.Data;
            }
            else
            {
                // we always encrypt, regardless of whether or not already encrypted
                // TODO: Maybe this would be a good place to check if we are trying to encrypt something with our own key?
                this.DataEncrypted = true;
                this.Data = CipherChain.EncryptChain();
                return CipherChain.EncryptChain();
            }

        }

        public string DecryptLoadedData()
        {
            if (this.DataEncrypted == true)
            {
                // If the script is encrypted - decrypt.
                this.Data = CipherChain.DecryptChain();
                this.DataEncrypted = false;
                return this.Data;
            }
            else
            {
                // If the script is already decrypted - just return plaintext.
                return this.CipherChain.GetChainPlainText();
            }
        }

        public void LoadDataFromFile(string filename, bool DataEncrypted)
        {
            byte[] FileByteArr;
            byte[] TempByteArr = new byte[32];
            byte TMPByte;
            int FileByteLen = 0;
            this.Data = "";

            StreamReader reader;

            ApolloCipherBlock NewBlock;

            this.CipherChain = new ApolloCipherBlockChain(DataEncrypted, Password, Secret1, Secret2);

            try
            {
                if (File.Exists(filename))
                {
                    reader = new StreamReader(filename, Encoding.UTF8);
                    FileByteArr = new byte[File.ReadAllBytes(filename).Length];
                    FileByteArr = File.ReadAllBytes(filename);
                    this.LoadedFileBytes = FileByteArr;
                }
                else
                {
                    throw new FileNotFoundException(filename);
                }
            }
            catch (Exception e)
            {
                // Silently fail and return until we decide on output
                throw e;
            }

            FileByteLen = FileByteArr.Length;

            // Let's load the file byte by byte
            for (int i = 0; i < FileByteLen; i += 32)
            {
                // This if-else determines the behavior for whether or not we are at the end of the string.
                // Oddly enough - behavior for the end of the string is the 'if' part. I don't typically do it this way.
                if (i + 32 >= FileByteLen)
                {
                    TempByteArr = new byte[32];

                    Buffer.BlockCopy(FileByteArr, i, TempByteArr, 0, FileByteLen - i);

                    NewBlock = new ApolloCipherBlock(TempByteArr, this.Password, this.Secret1, this.Secret2, this.DataEncrypted);

                    this.CipherChain.AddBlockToTail(this.CipherChain, NewBlock);

                    if (DataEncrypted)
                    {
                        this.Data += NewBlock.GetCipherTextString();
                    }
                    else
                    {
                        this.Data += NewBlock.GetPlainTextString();
                    }

                    // If we are loading unencrypted data - add a terminating block.
                    // We only want to add terminating blocks when we think we are going to encrypt - we don't add terminating blocks 
                    // and then decrypt. That's just confusion.
                    if (!DataEncrypted)
                    {
                        // Attach terminating block to chain (haha it's a linkedlist but y'know: "cHaIn").
                        ApolloCipherBlock TermBlock = ApolloCipherBlock.GenerateTerminatingBlock(this.Data.Length, this.Password, Secret1, Secret2, this.DataEncrypted);
                        this.CipherChain.AddBlockToTail(this.CipherChain, TermBlock);
                    }
                }
                else
                {
                    TempByteArr = new byte[32];

                    Buffer.BlockCopy(FileByteArr, i, TempByteArr, 0, 32);

                    NewBlock = new ApolloCipherBlock(TempByteArr, this.Password, this.Secret1, this.Secret2, this.DataEncrypted);

                    this.CipherChain.AddBlockToTail(this.CipherChain, NewBlock);


                    if (DataEncrypted)
                    {
                        this.Data += NewBlock.GetCipherTextString();
                    }
                    else
                    {
                        this.Data += NewBlock.GetPlainTextString();
                    }
                }
            }

        }

        public void SaveCipherTextToFile(string filename)
        {
            StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8);
            writer.Write(this.CipherChain.GetChainCipherText());
            writer.Flush();
            writer.Close();
        }

        public void SavePlainTextToFile(string filename)
        {
            StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8);
            writer.Write(this.CipherChain.GetChainPlainText());
            writer.Flush();
            writer.Close();
        }

        public string GetCurrentCiphertext()
        {
            return this.CipherChain.GetChainCipherText();
        }

        public string GetCurrentPlaintext()
        {
            return this.CipherChain.GetChainPlainText();
        }

        public void SetCipherText(string cipherText)
        {

        }

        public void SetPlainText(string plaintext)
        {

        }
    }
}

