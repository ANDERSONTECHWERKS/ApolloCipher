using System.ComponentModel.Design;
using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ApolloCipher;

namespace ApolloCipher
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
            Debug.WriteLine($"Prog1 Encrypted plaintext, returning Ciphertext:\n{Ciphertext}");

            Plaintext = PicProg.DecryptLoadedData();
            Debug.WriteLine($"Prog1 Decrypted ciphertext, returning Plaintext:\n{Plaintext}");

            Assert.IsTrue(Plaintext.Equals(LitanyAgainstFear));
        }

        [TestMethod]
        public void TestEncryptDecrypt2()
        {
            string TestData = "return(Ő*ŝ)+(ő*Ŏ)+(Œ*ʫ);}public void š(){Ŏ=ŏ=0;}}";

            Console.WriteLine("TestEncryptDecrypt starting with the following data:\n" + TestData);
            PicProg = new ApolloProgram(TestData, false, Password);

            // First, check that it's not null.
            Assert.IsNotNull(PicProg);

            Debug.WriteLine($"Instantiating cipher program successful.");

            Debug.WriteLine($"Prog1 GetCurrentPlaintext at instantiation is:\n");
            Debug.WriteLine(PicProg.GetCurrentPlaintext());

            Debug.WriteLine($"Prog1 GetCurrentCiphertext at instantiation is:\n");
            Debug.WriteLine(PicProg.GetCurrentCiphertext());

            Ciphertext = PicProg.EncryptLoadedData();
            Debug.WriteLine($"Prog1 Encrypted plaintext, returning Ciphertext:\n{Ciphertext}");

            Plaintext = PicProg.DecryptLoadedData();
            Debug.WriteLine($"Prog1 Decrypted ciphertext, returning Plaintext:\n{Plaintext}");

            Assert.IsTrue(Plaintext.Equals(TestData));
        }

        [TestMethod]
        public void DeterminismTest()
        {
            string pt1;
            string pt2;
            string ct1;
            string ct2;

            string password = "TeStPaSsW0rD!!2345@*()";
            string data = "<--HUSKER-->";

            ApolloProgram prog1 = new ApolloProgram(data,false, password);
            ApolloProgram prog2 = new ApolloProgram(data,false, password);

            pt1 = prog1.GetCurrentPlaintext();
            pt2 = prog2.GetCurrentPlaintext();

            prog1.EncryptLoadedData();
            prog2.EncryptLoadedData();

            ct1 = prog1.GetCurrentCiphertext();
            ct2 = prog2.GetCurrentCiphertext();

            Debug.WriteLine($"<--pt1-->\n{pt1}\n<--/pt1-->");
            Debug.WriteLine($"<--pt2-->\n{pt2}\n<--/pt2-->");
            Debug.WriteLine($"<--ct1-->\n{ct1}\n<--/ct1-->");
            Debug.WriteLine($"<--ct2-->\n{ct2}\n<--/ct2-->");
            Debug.WriteLine($"<--pt1 == pt2-->\n{pt1 == pt2}\n<--/pt1 == pt2-->");
            Debug.WriteLine($"<--ct1 == ct2-->\n{ct1 == ct2}\n<--/ct1 == ct2-->");

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
            PicProg2 = new ApolloProgram(CipherText, true, Password2);

            Debug.WriteLine($"Prog2 ciphertext is:\n");
            Debug.WriteLine(PicProg2.GetCurrentCiphertext());

            Debug.WriteLine($"Prog2 decrypting data...\n");
            Plaintext = PicProg2.DecryptLoadedData();

            Debug.WriteLine($"prog2 plaintext is:\n");
            Debug.WriteLine(Plaintext);

            Assert.IsTrue(Plaintext.Equals(LitanyAgainstFear));
        }

        [TestMethod]
        public void TestSmall()
        {
            string pt1;
            string pt2;
            string ct1;
            string ct2;

            string password = "TeSt";
            string data = "HUSK";

            ApolloProgram prog1 = new ApolloProgram(data, false, password);
            ApolloProgram prog2 = new ApolloProgram(prog1.EncryptLoadedData(), true, password);

            pt2 = prog2.DecryptLoadedData();

            Debug.WriteLine(pt2);

            Assert.IsTrue(pt2.Equals(data));
            
        }
        [TestMethod]
        public void encryptSingleChar()
        {
            string Ciphertext = "";
            string Plaintext = "C";

            PicProg = new ApolloProgram(Plaintext,false,"cookie");
            Ciphertext = PicProg.EncryptLoadedData();
            Assert.IsTrue(PicProg.DecryptLoadedData().Equals(Plaintext));

            Plaintext = ".";
            PicProg = new ApolloProgram(Plaintext, false, "cookie");
            Ciphertext = PicProg.EncryptLoadedData();
            Assert.IsTrue(PicProg.DecryptLoadedData().Equals(Plaintext));
        }

        [TestMethod]
        public void TestInternalShiftingOps()
        {
            PicProg = new ApolloProgram(false, "testPassword", "DefendTheArticles.txt");

            // First, check that it's not null.
            Assert.IsNotNull(PicProg);

            Debug.WriteLine($"TestFileOperations Instantiating program successful.\n");

            Debug.WriteLine($"Prog1 GetCurrentPlaintext at instantiation is:\n");
            Plaintext = PicProg.GetCurrentPlaintext();
            Debug.WriteLine(Plaintext + "\n");

            Debug.WriteLine($"Prog1 encrypting:\n");
            Ciphertext = PicProg.EncryptLoadedData();
            Debug.WriteLine(Ciphertext + "\n");

            Plaintext = PicProg.DecryptLoadedData();
            Debug.WriteLine($"Prog1 Decrypted ciphertext, Displaying Plaintext:\n {Plaintext}\n");

            Assert.IsTrue(Plaintext.Equals(DefendTheArticles));
        }

        [TestMethod]

        public void TestDataEncryptDecryptToDisk()
        {
            string Ciphertext = "";
            string Plaintext = "";
            string InitPlaintext = "";

            PicProg = new ApolloProgram(false, "NIMRODS", "script.cs");

            // First, check that it's not null.
            Assert.IsNotNull(PicProg);
            Debug.WriteLine($"Instantiating cipher program successful.\n");

            Debug.WriteLine($"Prog1 Plaintext is:\n");
            Plaintext = PicProg.GetCurrentPlaintext();
            InitPlaintext = Plaintext;
            Debug.WriteLine(Plaintext);

            Debug.WriteLine($"Encrypting...\n");
            Ciphertext = PicProg.EncryptLoadedData();

            Debug.WriteLine($"Prog1 Ciphertext is:\n");
            Debug.WriteLine(Ciphertext);

            Debug.WriteLine($"Saving...\n");
            PicProg.SaveCipherTextToFile("scriptEncrypted.cs");

            PicProg2 = new ApolloProgram(true, "NIMRODS", "scriptEncrypted.cs");

            Debug.WriteLine($"Prog2 GetCurrentCiphertext at instantiation:\n");
            Ciphertext = PicProg2.GetCurrentCiphertext();
            Debug.WriteLine(Ciphertext);

            Plaintext = PicProg2.DecryptLoadedData();

            Debug.WriteLine($"Prog2 has decrypted its data:\n");
            Debug.WriteLine(Plaintext);

            PicProg2.SaveCipherTextToFile("picProg2CT.txt");
            PicProg2.SavePlainTextToFile("picProg2PT.txt");

            Assert.IsTrue(Plaintext.Equals(InitPlaintext));
        }

    }
}