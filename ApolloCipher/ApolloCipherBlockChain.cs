using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApolloCipher
{
    // It's a linkedList that we chain cipherblocks with
    class ApolloCipherBlockChain
    {
        private ApolloCipherBlock TermBlock;

        private ApolloCipherBlock Value;
        private ApolloCipherBlockChain Next;
        private ApolloCipherBlockChain Prev;

        // Secret Bytes used for XORing to add confusion
        private byte SecretByte1 = 69;
        private byte SecretByte2 = 66;

        // Terminating byte will be SecretByte1 ^ SecretByte2, eventually. If this is ever zero: Something has gone wrong.
        private bool DataEncrypted = false;

        private string Password = "";

        // Generic stuff for counting and debug
        private int BlockCount = 0;

        internal ApolloCipherBlockChain(bool dataEncrypted, string password, byte SecretByte1, byte SecretByte2)
        {
            DataEncrypted = dataEncrypted;
            Value = null;
            Next = null;
            Prev = null;

            this.SecretByte1 = SecretByte1;
            this.SecretByte2 = SecretByte2;

            this.Password = password;
        }

        internal ApolloCipherBlockChain(ApolloCipherBlock head, string password, bool dataEncrypted, byte SecretByte1, byte SecretByte2)
        {
            DataEncrypted = dataEncrypted;
            Value = head;
            Next = null;
            Prev = null;

            // Blockcount is one. The head. The head we attached right above.
            BlockCount = 1;

            this.SecretByte1 = SecretByte1;
            this.SecretByte2 = SecretByte2;

            this.Password = password;

        }



        public ApolloCipherBlockChain(string data, string password, byte SecretByte1, byte SecretByte2, bool DataEncrypted)
        {
            this.SecretByte1 = SecretByte1;
            this.SecretByte2 = SecretByte2;

            this.DataEncrypted = DataEncrypted;

            this.Password = password;

            ApolloCipherBlock IterBlock;

            byte[] plaintextBytes = Encoding.UTF8.GetBytes(data);
            byte[] tmpByteArr;
            
            int ByteIterator = 0;
            Int32 PlaintextLength = plaintextBytes.Length;

            while(ByteIterator < PlaintextLength) {

                tmpByteArr = new byte[32];

                // Check if we are about to run over the array (end of plaintext string)

                // To be clear: this first 'if' condition hit when we are *at the end of the string*, AKA: "We have gone over plaintext len"
                if (ByteIterator + 32 > PlaintextLength)
                {
                    // If true: We only copy to the end of the array
                    for (int i = 0; ByteIterator + i <= PlaintextLength - 1; i++)
                    {
                        tmpByteArr[i] = plaintextBytes[ByteIterator + i];
                    }

                    IterBlock = new ApolloCipherBlock(tmpByteArr,password,SecretByte1,SecretByte2, DataEncrypted);

                    AddBlockToTail(this,IterBlock);

                    // Lets create a termination block to add at the end of each chain. This block will contain the stringlength, used to truncate
                    // the decryped message.
                    
                    TermBlock = ApolloCipherBlock.GenerateTerminatingBlock(PlaintextLength,password,SecretByte1,SecretByte2,DataEncrypted);
                    
                    AddBlockToTail(this, TermBlock);

                    break;
                } else
                {
                    // Otherwise: Below is the normal block processing up until we reach the end.
                    // If false: We copy the entire block
                    for (int i = 0;  i <= 31; i++)
                    {
                        tmpByteArr[i] = plaintextBytes[ByteIterator + i];
                    }

                    IterBlock = new ApolloCipherBlock(tmpByteArr, password, SecretByte1, SecretByte2, DataEncrypted);
                    AddBlockToTail(this, IterBlock);

                    ByteIterator += 32;
                    continue;
                }
            }
        }

        public void SetSecret1(byte newSecret1)
        {
            this.SecretByte1 = newSecret1;
        }

        public void SetSecret2(byte newSecret2)
        {
            this.SecretByte2 = newSecret2;
        }

        public byte GetSecret1()
        {
            return this.SecretByte1;
        }

        public byte GetSecret2() {
            return this.SecretByte2;
        }

        public void AddBlockToTail(ApolloCipherBlockChain blockchain, ApolloCipherBlock block)
        {
            ApolloCipherBlockChain prev = blockchain;

            // Check head
            if (blockchain.Value == null)
            {
                blockchain.Value = block;
                blockchain.Next = null;
                blockchain.Prev = null;
                return;
            }

            // Check next
            if (blockchain.Next == null)
            {
                blockchain.Next = new ApolloCipherBlockChain(block,Password, DataEncrypted, SecretByte1, SecretByte2);
            }
            else
            {
                // Iterate...
                while (blockchain.Next != null)
                {
                    prev = blockchain;
                    blockchain = blockchain.Next;
                }


                // assign new node to tail, assign 'previous' block to prior.
                blockchain.Next = new ApolloCipherBlockChain(block, Password, DataEncrypted, SecretByte1, SecretByte2);
                blockchain.Next.Prev = blockchain;
            }

            // increment blockcount. We assume we were successful.
            BlockCount++;

        }

        public string GetChainPlainText()
        {
            ApolloCipherBlockChain tmpChain = this;
            string result = "";

            while (tmpChain.Next != null)
            {
                result += tmpChain.Value.GetPlainTextString();

                tmpChain = tmpChain.Next;
            }

            result += tmpChain.Value.GetPlainTextString() + "\n";

            return result;
        }

        public string GetChainCipherText()
        {
            ApolloCipherBlockChain tmpChain = this;
            string result = "";

            while (tmpChain.Next != null)
            {
                result += tmpChain.Value.GetCipherTextString();

                tmpChain = tmpChain.Next;

            }

            result += tmpChain.Value.GetCipherTextString();

            return result;
        }

        public string EncryptChain()
        {
            ApolloCipherBlockChain tmpChain = this;
            string result = "";

            while (tmpChain.Next != null)
            {
                tmpChain.Value.EncryptPlainTextBlockPwd();

                result += tmpChain.Value.GetCipherTextString();

                tmpChain = tmpChain.Next;

            }

            tmpChain.Value.EncryptPlainTextBlockPwd();
            result += tmpChain.Value.GetCipherTextString();

            return result;
        }

        public string DecryptChain()
        {
            ApolloCipherBlockChain tmpChain = this;
            string TermIndexStr = "";
            int TerminationIndex = -1;
            byte[] indexBytes = new byte[4];
            byte[] TermIndexPlaintextByteArr;
            string Result = "";
            
            while (tmpChain.Next != null)
            {
                
                tmpChain.Value.DecryptCipherTextBlockPwd();

                Result += tmpChain.Value.GetPlainTextString();

                tmpChain = tmpChain.Next;

            }

            // At this point - we expect and check for the terminator block.

            // TODO: Figure out why the data gets mangled in this call to DecryptCipher...PWD()
            tmpChain.Value.DecryptCipherTextBlockPwd();

            // Once we detect where the string is *supposed* to end - copy that substring to the result.
            TermIndexPlaintextByteArr = tmpChain.Value.GetPlainTextBytes();

            indexBytes[0] = TermIndexPlaintextByteArr[0];
            indexBytes[1] = TermIndexPlaintextByteArr[1];
            indexBytes[2] = TermIndexPlaintextByteArr[2];
            indexBytes[3] = TermIndexPlaintextByteArr[3];

            // This is the exception: We are only intersted in the first four bytes
            TermIndexStr = Encoding.UTF8.GetString(indexBytes, 0, 4);

            try
            {
                TerminationIndex = Int32.Parse(TermIndexStr);

            }
            catch (Exception ex) {
                // We assume termination index is still null
                //TerminationIndex = null;
                Console.WriteLine("DEBUG: Decryption unable to find TerminationIndex! Decrypting entire message!");

                // This is where we drop the block off the end.
                //if(tmpChain.prev != null && tmpChain.prev.next != null)
                //{
                //    tmpChain.prev.next = null;
                //}

                // TODO: Need to figure out how to automatically drop the last block.
                // Probably need to implement next/prev behavior.
            }

            // Check if we were able to parse a termination index. If not - we'll just decrypt the whole thing.
            if (TerminationIndex >= 0)
            {
                Result = Result.Substring(0, TerminationIndex);
                this.DataEncrypted = false;
                return Result.TrimEnd('\0');

            } else
            {
                Result += tmpChain.Value.GetPlainTextString();
                this.DataEncrypted = false;
                return Result.TrimEnd('\0');
            }

        }

        public void SwapCiphertextPlaintextChain()
        {
            ApolloCipherBlockChain tmpChain = this;

            while (tmpChain.Next != null)
            {
                tmpChain.Value.SwapPlaintextCiphertextBlock();
            }
        }
    }
}
