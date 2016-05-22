// ====================================================================================================
//
// Class/Library: CRYPTOGRAPHY CLASS
//       Created: Nov 07, 2008
//
// VERS 1.0.000 : Nov 07, 2008 : Original File Created.
//			1.0.001 : Dec 08, 2008 : Added better handling of null input strings into Encode/Decode.
//			1.0.003 : Jan 04, 2011 : Solved problem where Bad Data was being created due to an invalid length key.
//			1.0.004 : Feb 22, 2013 : Added AES/SHA256 Cryptography.
//															 Improved the security algorythm by using a rotating dynamic key.
//
// ====================================================================================================


using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


public class Crypto
{

	#region "PRIVATE VARIABLES"

		private static string strDefaultKey        = "12345678901234567890123456789012";

		//                                           |      ENCRYPTION KEY SIZE       |  (32 Characters)
		//                                           |================================|
    private static string strDefaultVpadding   = "12345678901234567890123456789012";
    private static string strDefaultPadding    = "12345678901234567890123456789012";

		//                                           |ENCRYPTION KEY SIZE |  (20 Characters)
    //                                           |====================|
    private static string strDefaultSalt       = "12345678901234567890";
    private static string strDefaultVector     = "12345678901234567890";

    private static int    intDefaultIterations = 1000;

	#endregion

	#region "CRYPTO INIT"

		private static void CryptoInit()
		{

			try 
			{ 
				if (strDefaultKey      == "") 
				{

					strDefaultKey = "1234567890123456";
				
				}
			} catch { strDefaultKey        = "1234567890123456"; }									// 16 CHARACTERS
			try 
			{
				if (strDefaultVpadding == "")
				{

					strDefaultVpadding = "12345678901234567890123456789012";

				}
			} catch { strDefaultVpadding   = "12345678901234567890123456789012"; }	// 32 CHARACTERS
			
			try 
			{
				if (strDefaultPadding  == "") 
				{

					strDefaultPadding = "12345678901234567890123456789012"; 

				}
			} catch { strDefaultPadding    = "12345678901234567890123456789012"; }	// 32 CHARACTERS
			
			try 
			{
				if (strDefaultSalt     == "") 
				{

					strDefaultSalt = "12345678901234567890";

				}
			} catch { strDefaultSalt       = "12345678901234567890"; }							// 20 CHARACTERS
			
			try 
			{
				if (strDefaultVector   == "") 
				{

					strDefaultVector = "12345678901234567890";

				}
			} catch { strDefaultVector     = "12345678901234567890"; }							// 20 CHARACTERS
			
			try 
			{
				if (intDefaultIterations < 1) 
				{

					intDefaultIterations = 572;

				}
			} catch { intDefaultIterations = 572; }
		}
		
	#endregion

	/// <summary>
	/// Encrypts a string
	/// </summary>
	/// <param name="PlainText">Text to be encrypted</param>
	/// <param name="Password">Password to encrypt with</param>
	/// <param name="Salt">Salt to encrypt with</param>
	/// <param name="CryptoType">Can be either AES, 3DES, RJINDAEL</param>
	/// <param name="PasswordIterations">Number of iterations to do</param>
	/// <param name="InitialVector">Needs to be 16 ASCII characters long for SHA1, 32 ASCII characters long for SHA256</param>
	/// <param name="KeySize">Can be 128, 192, or 256</param>
	/// <returns>An encrypted string</returns>
	public static string Encrypt(string PlainText, string Password = "", string Salt = "", string CryptoType = "3des", int PasswordIterations = 0, string InitialVector = "", int KeySize = 256)
	{
		// EMPTY STRINGS RETURN EMPTY ENCRYPTION
		if (string.IsNullOrEmpty(PlainText))
			return "";

		// INIT CRYPTOGRAPHY
		CryptoInit();

		// POPULATE EMPTY FIELDS WITH DEFAULT VALUES
		if (Password == "")
				Password = strDefaultKey;
		if (Salt == "")
				Salt = strDefaultSalt;
		if (InitialVector == "")
				InitialVector = strDefaultVector;
		if (PasswordIterations == 0)
				PasswordIterations = intDefaultIterations;

		try
		{
			// DETERMINE HASH
			int BlockSize = 128;
			Boolean blnSetInit = false;
			SymmetricAlgorithm SymmetricKey;

			// SET THE CRYPTOGRAPHY METHOD
			if (CryptoType.ToLower() == "aes")
			{
				// AES
				BlockSize    = 128;		// MUST BE 128
				if (KeySize  < 128)
					KeySize    = 128;
				if (KeySize  > 256)
					KeySize    = 256;
//			SymmetricKey = new AesCryptoServiceProvider();
				SymmetricKey = new RijndaelManaged();
				blnSetInit = true;
				if (InitialVector.Length < (BlockSize / 8))
					InitialVector += strDefaultVpadding.Substring(0, (BlockSize / 8) - InitialVector.Length);
				else if (InitialVector.Length > (BlockSize / 8))
					InitialVector = InitialVector.Substring(0, (BlockSize / 8));
				if (Password.Length < (BlockSize / 8))
					Password += strDefaultPadding.Substring(0, (BlockSize / 8) - Password.Length);
				else if (Password.Length > (BlockSize / 8))
					Password = Password.Substring(0, (BlockSize / 8));
			} else if (CryptoType.ToLower() == "3des") {
				// TRIPLE-DES
				BlockSize    = 64;		// MUST BE 64
				if (KeySize  < 128)
					KeySize    = 128;
				if (KeySize  > 192)
					KeySize    = 192;
				SymmetricKey = new TripleDESCryptoServiceProvider();
			} else {
				// RIJNDAEL - NOT FIPS COMPLIANT
				if (BlockSize < 128)
					BlockSize   = 128;
				if (BlockSize > 256)
					BlockSize   = 256;
				if (KeySize   < 128)
					KeySize     = 128;
				if (KeySize   > 256)
					KeySize     = 256;
				SymmetricKey  = new RijndaelManaged();
			}

			// SET THE INITIAL VECTOR AND CRYPTOGRAPHY PASSWORD
			if (!blnSetInit)
			{
				if (InitialVector.Length < (BlockSize / 8))
					InitialVector += strDefaultVpadding.Substring(0, (BlockSize / 8) - InitialVector.Length);
				else if (InitialVector.Length > (BlockSize / 8))
					InitialVector = InitialVector.Substring(0, (BlockSize / 8));
				if (Password.Length < (KeySize / 8))
					Password += strDefaultPadding.Substring(0, (KeySize / 8) - Password.Length);
				else if (Password.Length > (KeySize / 8))
					Password = Password.Substring(0, (KeySize / 8));
			}

			// BUILD THE ENCRYPTION PARAMETERS
			byte[]              InitialVectorBytes     = Encoding.ASCII.GetBytes(InitialVector);
			byte[]              PlainTextBytes         = Encoding.UTF8.GetBytes(PlainText);
			byte[]              SaltValueBytes         = Encoding.ASCII.GetBytes(Salt);
			Rfc2898DeriveBytes  pwdGen                 = new Rfc2898DeriveBytes(Password, SaltValueBytes, PasswordIterations);
		  byte[]							KeyBytes               = pwdGen.GetBytes(KeySize   / 8);	// This will generate a 128/256 bits key
	    byte[]							IVBytes                = pwdGen.GetBytes(BlockSize / 8);  // This will generate a 128/256 bits IV
													SymmetricKey.Padding   = PaddingMode.PKCS7;
													SymmetricKey.Mode      = CipherMode.CBC;
													SymmetricKey.BlockSize = BlockSize;
													SymmetricKey.Key       = KeyBytes;
													SymmetricKey.IV        = IVBytes;
			byte[]              CipherTextBytes        = null;

			// PERFORM THE ENCRYPTION
			using (ICryptoTransform Encryptor = SymmetricKey.CreateEncryptor(KeyBytes, InitialVectorBytes))
			{
				using (MemoryStream MemStream = new MemoryStream()) 
				{
					using (CryptoStream CryptoStream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write))
					{
						CryptoStream.Write(PlainTextBytes, 0, PlainTextBytes.Length);
						CryptoStream.FlushFinalBlock();
						CipherTextBytes = MemStream.ToArray();
						MemStream.Close();
						CryptoStream.Close();
					}
				}
			}
			SymmetricKey.Clear();
			return Convert.ToBase64String(CipherTextBytes);


		} catch {
			return PlainText;
		}

	}
	
	/// <summary>
	/// Decrypts a string
	/// </summary>
	/// <param name="CipherText">Text to be decrypted</param>
	/// <param name="Password">Password to decrypt with</param>
	/// <param name="Salt">Salt to decrypt with</param>
	/// <param name="CryptoType">Can be either AES, 3DES, RJINDAEL</param>
	/// <param name="PasswordIterations">Number of iterations to do</param>
	/// <param name="InitialVector">Needs to be 16 ASCII characters long for SHA1, 32 ASCII characters long for SHA256</param>
	/// <param name="KeySize">Can be 128, 192, or 256</param>
	/// <returns>A decrypted string</returns>
	public static string Decrypt(string CipherText, string Password = "", string Salt = "", string CryptoType = "3des", int PasswordIterations = 0, string InitialVector = "", int KeySize = 256)
	{
		if (string.IsNullOrEmpty(CipherText))
			return "";

		// INIT CRYPTOGRAPHY
		CryptoInit();

		// POPULATE EMPTY FIELDS WITH DEFAULT VALUES
		if (Password == "")
				Password = strDefaultKey;
		if (Salt == "")
				Salt = strDefaultSalt;
		if (InitialVector == "")
				InitialVector = strDefaultVector;
		if (PasswordIterations == 0)
				PasswordIterations = intDefaultIterations;

		try
		{
			// DETERMINE HASH
			int BlockSize = 128;
			Boolean blnSetInit = false;
			SymmetricAlgorithm SymmetricKey;

			// SET THE CRYPTOGRAPHY METHOD
			if (CryptoType.ToLower() == "aes")
			{
				// AES
				BlockSize    = 128;		// MUST BE 128
				if (KeySize  < 128)
					KeySize    = 128;
				if (KeySize  > 256)
					KeySize    = 256;
//			SymmetricKey = new AesCryptoServiceProvider();
				SymmetricKey = new RijndaelManaged(); 
				blnSetInit   = true;
				if (InitialVector.Length < (BlockSize / 8))
					InitialVector += strDefaultVpadding.Substring(0, (BlockSize / 8) - InitialVector.Length);
				else if (InitialVector.Length > (BlockSize / 8))
					InitialVector = InitialVector.Substring(0, (BlockSize / 8));
				if (Password.Length < (BlockSize / 8))
					Password += strDefaultPadding.Substring(0, (BlockSize / 8) - Password.Length);
				else if (Password.Length > (BlockSize / 8))
					Password = Password.Substring(0, (BlockSize / 8));
			} else if (CryptoType.ToLower() == "3des") {
				// TRIPLE-DES
				BlockSize    = 64;		// MUST BE 64
				if (KeySize  < 128)
					KeySize    = 128;
				if (KeySize  > 192)
					KeySize    = 192;
				SymmetricKey = new TripleDESCryptoServiceProvider();
			} else {
				// RIJNDAEL - NOT FIPS COMPLIANT
				if (BlockSize < 128)
					BlockSize   = 128;
				if (BlockSize > 256)
					BlockSize   = 256;
				if (KeySize   < 128)
					KeySize     = 128;
				if (KeySize   > 256)
					KeySize     = 256;
				SymmetricKey  = new RijndaelManaged();
			}

			// SET THE INITIAL VECTOR AND CRYPTOGRAPHY PASSWORD
			if (!blnSetInit)
			{
				if (InitialVector.Length < (BlockSize / 8))
					InitialVector += strDefaultVpadding.Substring(0, (BlockSize / 8) - InitialVector.Length);
				else if (InitialVector.Length > (BlockSize / 8))
					InitialVector = InitialVector.Substring(0, (BlockSize / 8));
				if (Password.Length < (KeySize / 8))
					Password += strDefaultPadding.Substring(0, (KeySize / 8) - Password.Length);
				else if (Password.Length > (KeySize / 8))
					Password = Password.Substring(0, (KeySize / 8));
			}

			// BUILD THE DECRYPTION PARAMETERS
			byte[]              InitialVectorBytes     = Encoding.ASCII.GetBytes(InitialVector);
			byte[]              CipherTextBytes        = Convert.FromBase64String(CipherText);
			byte[]              SaltValueBytes         = Encoding.ASCII.GetBytes(Salt);
			Rfc2898DeriveBytes  pwdGen                 = new Rfc2898DeriveBytes(Password, SaltValueBytes, PasswordIterations);
		  byte[]							KeyBytes               = pwdGen.GetBytes(KeySize   / 8);	// This will generate a 128/256 bits key
	    byte[]							IVBytes                = pwdGen.GetBytes(BlockSize / 8);  // This will generate a 128/256 bits IV
													SymmetricKey.Padding   = PaddingMode.PKCS7;
													SymmetricKey.Mode      = CipherMode.CBC;
													SymmetricKey.BlockSize = BlockSize;
													SymmetricKey.Key       = KeyBytes;
													SymmetricKey.IV        = IVBytes;
			byte[]              PlainTextBytes         = new byte[CipherTextBytes.Length];
			int                 ByteCount              = 0;

			// PERFORM THE DECRYPTION
			using (ICryptoTransform Decryptor = SymmetricKey.CreateDecryptor(KeyBytes, InitialVectorBytes))
			{
				using (MemoryStream MemStream = new MemoryStream(CipherTextBytes))
				{
					using (CryptoStream CryptoStream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read))
					{
						ByteCount = CryptoStream.Read(PlainTextBytes, 0, PlainTextBytes.Length);
						MemStream.Close();
						CryptoStream.Close();
					}
				}
			}
			SymmetricKey.Clear();
			return Encoding.UTF8.GetString(PlainTextBytes, 0, ByteCount);

		} catch {
			return "";
		}
	}

}
