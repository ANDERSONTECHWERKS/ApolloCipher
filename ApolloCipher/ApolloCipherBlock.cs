using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ApolloCipher
{
    // A simple cipher, written by Apollo.
    internal class ApolloCipherBlock
    {
        // These are intermediate representations
        public string CipherTextString = "";
        public string PlainTextString = "";

        // This is the data
        private byte[] CipherTextByteArr;
        private byte[] PlainTextByteArr;
        private byte[] PasswordByteArr;

        private int CryptoIterator = 0;

        private byte SecretByte1;
        private byte SecretByte2;

        // Used only to generate terminating blocks
        private ApolloCipherBlock(int strLen, string password, byte SecretByte1, byte SecretByte2, bool ScriptEncrypted)
        {

            if (ScriptEncrypted)
            {
                CryptoIterator = 0;

                this.PasswordByteArr = Encoding.UTF8.GetBytes(password);

                this.CipherTextByteArr = new byte[32];
                this.CipherTextByteArr = Encoding.UTF8.GetBytes(strLen.ToString());
                this.CipherTextString = strLen.ToString();

                this.PlainTextByteArr = new byte[32];
                this.PlainTextString = "";

                // Get our secrets from the chain
                this.SecretByte1 = SecretByte1;
                this.SecretByte2 = SecretByte2;

            } else
            {
                CryptoIterator = 0;

                this.PasswordByteArr = Encoding.UTF8.GetBytes(password);

                this.CipherTextByteArr = new byte[32];
                this.CipherTextByteArr = Encoding.UTF8.GetBytes(strLen.ToString());
                this.CipherTextString = strLen.ToString();

                this.PlainTextByteArr = new byte[32];
                this.PlainTextByteArr = Encoding.UTF8.GetBytes(strLen.ToString());
                this.PlainTextString = strLen.ToString();

                // Get our secrets from the chain
                this.SecretByte1 = SecretByte1;
                this.SecretByte2 = SecretByte2;
            }

        }

        public ApolloCipherBlock(string plaintext, string password, byte SecretByte1, byte SecretByte2)
        {
            CryptoIterator = 0;


            this.PlainTextByteArr = Encoding.UTF8.GetBytes(plaintext);
            this.PasswordByteArr = Encoding.UTF8.GetBytes(password);

            // Get our secrets from the chain
            this.SecretByte1 = SecretByte1;
            this.SecretByte2 = SecretByte2;

            // Encrypt the plaintext we received
            this.EncryptPlainTextBlockPwd();
        }

        public ApolloCipherBlock(byte[] dataBytes, string password, byte SecretByte1, byte SecretByte2, bool ScriptEncrypted)
        {
            if (dataBytes.Length <= 32)
            {
                if (ScriptEncrypted)
                {
                    CryptoIterator = 0;

                    this.CipherTextByteArr = dataBytes;
                    this.CipherTextString = Encoding.UTF8.GetString(dataBytes);

                    this.PasswordByteArr = Encoding.UTF8.GetBytes(password);

                    this.PlainTextByteArr = CipherTextByteArr;
                    this.PlainTextString = CipherTextString;

                    // Get our secrets from the chain
                    this.SecretByte1 = SecretByte1;
                    this.SecretByte2 = SecretByte2;

                }
                else
                {
                    CryptoIterator = 0;

                    this.PlainTextByteArr = dataBytes;
                    this.PlainTextString = Encoding.UTF8.GetString(dataBytes);

                    this.PasswordByteArr = Encoding.UTF8.GetBytes(password);

                    this.CipherTextByteArr = PlainTextByteArr;
                    this.CipherTextString = PlainTextString;

                    // Get our secrets from the chain
                    this.SecretByte1 = SecretByte1;
                    this.SecretByte2 = SecretByte2;

                }

            }
            else
            {
                throw new ArgumentException("Cipher Block Constructor reports incorrect block length!");
            }
        }

        public static ApolloCipherBlock GenerateTerminatingBlock(int strLen, string password, byte Secret1, byte Secret2, bool ScriptEncrypted)
        {
            return new ApolloCipherBlock(strLen, password, Secret1, Secret2, ScriptEncrypted);
        }

        public void PrintPlainTextByteVals()
        {
            Console.WriteLine($"Current plaintext bytes are:");

            foreach (byte byteValue in PlainTextByteArr)
            {
                Console.WriteLine(byteValue + $"-> {(char)byteValue}");
            }
        }

        public void PrintCipherTextByteVals()
        {
            Console.WriteLine($"Current ciphertext bytes are:");
            foreach (byte byteValue in CipherTextByteArr)
            {
                Console.WriteLine(byteValue + $"-> {(char)byteValue}");
            }
        }

        public void SetPlainTextManual(string inputPlaintext)
        {
            PlainTextString = inputPlaintext;
            PlainTextByteArr = Encoding.UTF8.GetBytes(inputPlaintext);
        }

        public void SetCipherTextManual(string inputCipherText)
        {
            CipherTextString = inputCipherText;
            CipherTextByteArr = Encoding.UTF8.GetBytes(inputCipherText);

        }

        public string GetPlainTextString()
        {
            return Encoding.UTF8.GetString(PlainTextByteArr, 0, PlainTextByteArr.Length);
        }

        public string GetCipherTextString()
        {
            return Encoding.UTF8.GetString(CipherTextByteArr, 0, CipherTextByteArr.Length);
        }

        public byte[] GetCipherTextBytes()
        {
            return CipherTextByteArr;
        }

        public byte[] GetPlainTextBytes()
        {
            return PlainTextByteArr;
        }

        public void DecryptCipherTextBlockPwd()
        {
            byte[] tempArr = new byte[32];
            byte tempByte;
            byte tempPasswordByte;

            bool FoundTerminationMarker = false;
            int TerminatorIndicatedIndex = 0;

            CryptoIterator = 0;
            CipherTextByteArr.CopyTo(tempArr, 0);

            // Where we undo the XORing with our password
            for (int i = 0; i < tempArr.Length; i++)
            {
                tempByte = tempArr[i];

                if (CryptoIterator < PasswordByteArr.Length)
                {
                    tempPasswordByte = PasswordByteArr[CryptoIterator];
                    tempByte = (byte)((tempByte ^ tempPasswordByte));
                    CryptoIterator++;
                }
                else
                {
                    // We reset *first*, because we assume the above "if" has already shifted the byte behind us
                    CryptoIterator = 0;
                    tempPasswordByte = PasswordByteArr[CryptoIterator];
                    tempByte = (byte)((tempByte ^ tempPasswordByte));
                }

                tempByte = (byte)(tempByte ^ SecretByte2);
                tempByte = (byte)(tempByte ^ SecretByte1);

                /* We're gonna manage the termination byte at the chain level
                if (tempByte.Equals(TerminationByte))
                {
                    // Currently: We aren't really doing anything with the terminationbyte besides marking-and-forgetting.
                    // Anyways - here's some framework.
                    try
                    {
                        FoundTerminationMarker = true;
                        TerminatorIndicatedIndex = (int)tempByte;
                        PlainTextString = Encoding.UTF8.GetString(tempArr);

                        PlainTextByteArr = tempArr;
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to parse terminating byte!");
                        throw ex;
                    }

                }
                */

                tempArr[i] = tempByte;
            }

            // Blank the ciphertext once we're done with it.
            CipherTextString = "";
            Array.Clear(CipherTextByteArr);

            PlainTextString = Encoding.UTF8.GetString(tempArr);
            PlainTextByteArr = tempArr;
        }

        public void EncryptPlainTextBlockPwd()
        {
            // Lets make a cipher for fun and internet spaceship profit!
            // Make the temp buffer. We will make a 32-byte 
            byte[] tempArr = new byte[32];
            byte tempByte;
            byte tempPasswordByte;

            int TerminationIndex = 0;
            bool PlacedTerminationMarker = false;

            CryptoIterator = 0;

            PlainTextByteArr.CopyTo(tempArr, 0);

            for (int i = 0; i < tempArr.Length; i++)
            {
                tempByte = tempArr[i];

                // First, XOR with secrets 1 and 2
                tempByte = (byte)(tempByte ^ SecretByte1);
                tempByte = (byte)(tempByte ^ SecretByte2);

                // Next, XOR with password bytes
                if (CryptoIterator < PasswordByteArr.Length)
                {
                    tempPasswordByte = PasswordByteArr[CryptoIterator];
                    tempByte = (byte)((tempByte ^ tempPasswordByte));
                    CryptoIterator++;
                }
                else
                {
                    // We reset *first*, because we assume the above "if" has already shifted the byte behind us
                    CryptoIterator = 0;
                    tempPasswordByte = PasswordByteArr[CryptoIterator];
                    tempByte = (byte)((tempByte ^ tempPasswordByte));
                }

                tempArr[i] = tempByte;
            }

            PlainTextString = "";
            Array.Clear(PlainTextByteArr);

            CipherTextByteArr = tempArr;
            CipherTextString = Encoding.UTF8.GetString(tempArr);
            
        }

        public void SwapPlaintextCiphertextBlock()
        {
            string tmpstr = this.PlainTextString;
            byte[] tmpbyte = this.PlainTextByteArr;

            this.PlainTextString = this.CipherTextString;
            this.PlainTextByteArr = this.CipherTextByteArr;

            this.CipherTextString = tmpstr;
            this.CipherTextByteArr = tmpbyte;
        }
    }
}
