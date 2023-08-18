using System.ComponentModel.Design;
using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ApolloCipher;

namespace DataLockerMSTests
{

    [TestClass]
    public class EncryptDecryptTests
    {
        string LitanyAgainstFear = "" +
            "I must not fear.\r\n" +
            "Fear is the mind-killer.\r\n" +
            "Fear is the little-death that brings total obliteration.\r\n" +
            "I will face my fear.\r\n" +
            "I will permit it to pass over me and through me.\r\n" +
            "And when it has gone past, I will turn the inner eye to see its path.\r\n" +
        "Where the fear has gone there will be nothing. Only I will remain.";

        string DefendTheArticles = "Apollo: I swore an oath. To defend the articles. The articles say there is an election in seven months. Now, if you are telling me we are throwing out the law, then I am not a captain, you are not a commander, and you are not the president. And I don't owe either of you a damned explanation for anything.";

        string InitPlaintext;
        string Plaintext;
        string Ciphertext;
        string Password = "Gentlemen";
        string Password2 = "Behold";
        ApolloProgram PicProg;
        ApolloProgram PicProg2;

        [TestMethod]
        public void TestEncryptDecrypt()
        {
            Console.WriteLine("TestEncryptDecrypt starting with the following data:\n" + LitanyAgainstFear);
            PicProg = new ApolloProgram(LitanyAgainstFear, false,Password);
            
            // First, check that it's not null.
            Assert.IsNotNull(PicProg);

            Debug.WriteLine($"Instantiating cipher program successful.");

            Debug.WriteLine($"Prog1 GetCurrentPlaintext at instantiation is:\n");
            Debug.WriteLine(PicProg.GetCurrentPlaintext());

            Debug.WriteLine($"Prog1 GetCurrentCiphertext at instantiation is:\n");
            Debug.WriteLine(PicProg.GetCurrentCiphertext());

            Ciphertext = PicProg.EncryptLoadedData();
            Debug.WriteLine($"Prog1 Encrypted plaintext, returning Ciphertext:\n {Ciphertext}");

            Plaintext = PicProg.DecryptLoadedData();
            Debug.WriteLine($"Prog1 Decrypted ciphertext, returning Plaintext:\n {Plaintext}");

            Assert.IsTrue(Plaintext.Equals(LitanyAgainstFear));
        }

        [TestMethod]
        public void TestEncryptDecryptx2nodes()
        {
            string CipherText = "";
            string PlainText = "";

            Console.WriteLine("TestEncryptDecrypt starting with the following script:\n" + LitanyAgainstFear);
            PicProg = new ApolloProgram(LitanyAgainstFear, false, Password);

            // Make the two encryptor/decryptors feed each other ciphertext and return it to normal.

            // First, check that it's not null.
            Assert.IsNotNull(PicProg);
            Debug.WriteLine($"Instantiating cipher program successful.");

            // Show us the status of PicProg1
            Debug.WriteLine($"GetCurrentPlaintext1 at instantiation is:\n");
            Debug.WriteLine(PicProg.GetCurrentPlaintext());

            CipherText = PicProg.EncryptLoadedData();

            Debug.WriteLine($"GetCurrentCiphertext1 is:\n");
            Debug.WriteLine(CipherText);

            Debug.WriteLine($"Creating Prog2 with ciphertext=true input...\n");
            PicProg2 = new ApolloProgram(Ciphertext, true, Password2);

            Debug.WriteLine($"Prog2 ciphertext is:\n");
            Debug.WriteLine(PicProg2.GetCurrentCiphertext());

            Debug.WriteLine($"Prog2 decrypting data...\n");
            Plaintext = PicProg2.DecryptLoadedData();

            Debug.WriteLine($"prog2 plaintext is:\n");
            Debug.WriteLine(Plaintext);

            Assert.IsTrue(Plaintext.Equals(LitanyAgainstFear));
        }

        [TestMethod]
        public void TestFileOperations()
        {
            PicProg = new ApolloProgram(false, "testPassword", "DefendTheArticles.txt");

            // First, check that it's not null.
            Assert.IsNotNull(PicProg);

            Debug.WriteLine($"TestFileOperations Instantiating program successful.\n");

            Debug.WriteLine($"Prog1 GetCurrentPlaintext at instantiation is:\n");
            Plaintext = PicProg.GetCurrentPlaintext();
            Debug.WriteLine(Plaintext + "\n");

            Debug.WriteLine($"Prog1 GetCurrentCiphertext at instantiation is:\n");
            Ciphertext = PicProg.GetCurrentCiphertext();
            Debug.WriteLine(Ciphertext + "\n");

            Ciphertext = PicProg.EncryptLoadedData();
            Debug.WriteLine($"Prog1 Encrypted plaintext. Displaying Ciphertext:\n {Ciphertext}\n");

            Plaintext = PicProg.DecryptLoadedData();
            Debug.WriteLine($"Prog1 Decrypted ciphertext, Displaying Plaintext:\n {Plaintext}\n");

            Assert.IsTrue(Plaintext.Equals(DefendTheArticles));
        }

        [TestMethod]

        public void TestDataEncryptDecryptToDisk()
        {
            PicProg = new ApolloProgram(false, "NIMRODS", "script.cs");

            // First, check that it's not null.
            Assert.IsNotNull(PicProg);
            Debug.WriteLine($"Instantiating cipher program successful.\n");

            Debug.WriteLine($"Prog1 Plaintext is:\n");
            Plaintext = PicProg.GetCurrentPlaintext();
            InitPlaintext = Plaintext;
            Debug.WriteLine(Plaintext);

            Debug.WriteLine($"Encrypting...\n");
            PicProg.EncryptLoadedData();

            Debug.WriteLine($"Prog1 Ciphertext is:\n");
            Ciphertext = PicProg.GetCurrentCiphertext();
            Debug.WriteLine(Ciphertext);

            Debug.WriteLine($"Saving...\n");
            PicProg.SaveCipherTextToFile("scriptEncrypted.cs");

            PicProg2 = new ApolloProgram(true, "NIMRODS", "scriptEncrypted.cs");

            Debug.WriteLine($"Prog2 GetCurrentCiphertext at instantiation:\n");
            Ciphertext = PicProg2.GetCurrentCiphertext();
            Debug.WriteLine(Ciphertext);

            PicProg2.DecryptLoadedData();

            Debug.WriteLine($"Prog2 has decrypted its data:\n");
            Plaintext = PicProg2.GetCurrentPlaintext();
            Debug.WriteLine(Plaintext);

            Assert.IsTrue(Plaintext.Equals(InitPlaintext));
        }

    }
}