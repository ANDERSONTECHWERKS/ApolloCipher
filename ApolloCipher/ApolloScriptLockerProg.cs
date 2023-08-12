using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
// Blog used to develop this script:
//https://ianvink.wordpress.com/2022/12/03/a-straightforward-way-in-c-net-to-encrypt-and-decrypt-a-string-using-aes/
namespace ApolloCipher
{
    public class ApollosScriptLockerProg
    {
        // Static stuff for the program itself
        string Script;
        string Password;

        bool ScriptEncrypted = false;

        ApolloCipherBlockChain CipherChain;

        byte Secret1 = 69;
        byte Secret2 = 66;
        byte TerminationByte = 42;

        public ApollosScriptLockerProg(string script, bool ciphertextScript, string password)
        {
            this.CipherChain = new ApolloCipherBlockChain(script,password,Secret1,Secret2, ciphertextScript);
            this.Script = script;
            this.ScriptEncrypted = ciphertextScript;
            this.Password = password;
        }

        public ApollosScriptLockerProg(bool ciphertextScript, string password, string filename)
        {
            this.LoadScriptFromFile(filename, ciphertextScript);
            this.CipherChain = new ApolloCipherBlockChain(this.Script, password, this.Secret1, this.Secret2, ciphertextScript);
        }

        public static void Main()
        {
            Console.InputEncoding = Encoding.UTF8;

            string script = "";
            string password = "";
            string ciphertext = "";
            string plaintext = "";
            string? userInput = "";

            byte Secret1 = 66;
            byte Secret2 = 69;

            byte[] ciphertextBytes = null;

            bool debug = true;

            ApolloCipher.ApolloCipherBlockChain pChain;

            Console.WriteLine("Please enter what you want to encrypt and press return on an empty string when you have pasted everything:");

            userInput = Console.ReadLine();

            while (userInput != null && !userInput.Equals(""))
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


            plaintext = pChain.DecryptChain();

            if (debug)
            {
                Console.WriteLine($"Decryption reveals the following plaintext:");
                Console.WriteLine(plaintext);
            }


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

        public static string EncryptScript(string script, string password, byte Secret1, byte Secret2)
        {
            ApolloCipherBlockChain? pChain = null;
            string IterSubstring = "";

            for (int i = 0; i < script.Length; i += 16)
            {

                // Check for initialized pChain
                if (pChain == null)
                {
                    IterSubstring = script.Substring(i, 16);
                    ApolloCipherBlock pCipherHead = new ApolloCipherBlock(IterSubstring, password, Secret1, Secret2);
                    pChain = new ApolloCipherBlockChain(pCipherHead);
                    continue;
                }
                else
                {
                    // Try-catch here, in case we overrun the Length of the script.
                    try
                    {
                        IterSubstring = script.Substring(i, 16);
                    } catch(ArgumentOutOfRangeException aore)
                    {
                        IterSubstring = script.Substring(i, (script.Length-1) - i);
                    }



                    ApolloCipherBlock pCipherHead = new ApolloCipherBlock(IterSubstring, password, Secret1, Secret2);
                    pChain.AddBlockToTail(pChain, pCipherHead);
                }
            }
            return pChain.EncryptChain();

        }

        public static string DecryptScript(string encrypted, string password, byte Secret1, byte Secret2)
        {
            ApolloCipherBlockChain? pChain = null;

            for (int i = 0; i < encrypted.Length; i += 16)
            {

                // Check for initialized pChain
                if (pChain == null)
                {
                    ApolloCipherBlock pCipherHead = new ApolloCipherBlock(encrypted.Substring(i, 16), password, Secret1, Secret2);
                    pCipherHead.SetCipherTextManual(encrypted.Substring(i, 16));
                    pChain = new ApolloCipherBlockChain(pCipherHead);
                    continue;
                }
                else
                {
                    ApolloCipherBlock pCipherHead = new ApolloCipherBlock(encrypted.Substring(i, 16), password, Secret1, Secret2);
                    pCipherHead.SetCipherTextManual(encrypted.Substring(i, 16));
                    pChain.AddBlockToTail(pChain, pCipherHead);
                }
            }

            return pChain.DecryptChain();
        }

        public string EncryptLoadedScript()
        {
            if(this.ScriptEncrypted == false)
            {
                this.ScriptEncrypted = true;
                return CipherChain.EncryptChain();
            } else
            {
                // we always encrypt, regardless of whether or not already encrypted
                // TODO: Maybe this would be a good place to check if we are trying to encrypt something with our own key?
                this.ScriptEncrypted = true;
                return CipherChain.EncryptChain();
            }

        }

        public string DecryptLoadedScript()
        {
            if (this.ScriptEncrypted == true)
            {
                // If the script is encrypted - decrypt.
                this.ScriptEncrypted = false;
                return CipherChain.DecryptChain();
            } else
            {
                // If the script is already decrypted - just return plaintext.
                return CipherChain.DecryptChain();
            }
        }

        public void LoadScriptFromFile(string filename, bool ScriptEncrypted)
        {
            this.Script = "";

            StreamReader reader = new StreamReader(filename);

            string line;

            while ((line = reader.ReadLine()) != null)
            {
                this.Script += line;
            }
        }

        public void SaveScriptToFile(string filename)
        {
            StreamWriter writer = new StreamWriter(filename);
            writer.Write(this.Script);
        }

        public string GetCurrentCiphertext ()
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

