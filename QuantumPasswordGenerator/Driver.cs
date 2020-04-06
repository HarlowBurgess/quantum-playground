using System;
using System.Linq;
using System.Text;
using Microsoft.Quantum.Simulation.Core;
using Microsoft.Quantum.Simulation.Simulators;

namespace QuantumPasswordGenerator
{
    /// <summary>
    /// Console app to generate a random password using quantum effects.
    /// </summary>
    /// <remarks>
    /// Note that by using a quantum simulator, the password will only be as random as a classically generated password.
    /// Classically generated random passwords ultimately use a classical algorithm which could, in theory, be cracked with
    /// a powerful enough supercomputer. However, nothing worth that level of effort would be protected by just a password
    /// anyway. The purpose of this program is simply to provide an introduction to hybrid quantum computing (C# + Q#).
    /// </remarks>
    class Driver
    {
        // Define valid password character space
        const string PasswordCharsGroupA = "abcdefghijklmnopqrstuvwxyz";
        const string PasswordCharsGroupB = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string PasswordCharsGroupC = "0123456789";
        const string PasswordCharsGroupD = "!$^*()"; // Less scary looking symbols from the larger group !#$%&()*+,-./:;<=>?@[\]^_{|}~
        const string ValidPasswordChars = PasswordCharsGroupA + PasswordCharsGroupB + PasswordCharsGroupC + PasswordCharsGroupD;

        // Define password length
        const int PasswordLength = 16;

        static void Main(string[] _)
        {
            // Create the quantum simulator
            //TODO: Use physical device instead of a simulator, if you are so fortunate as to have a quantum device :)
            using var qsim = new QuantumSimulator();

            // Determine how may bits are needed to cover the password character space
            int numBits = (int)Math.Log2(ValidPasswordChars.Length) + 1;

            string password;
            char[] passwordChars = new char[PasswordLength];
            // Generate random passwords until the generated password meets the password requirements
            do
            {
                // Generate the password characters one at a time.
                //   We generate one character at a time because each character index requires around 7 qubits, which would be 112 qubits
                //   for a 16 character password. As of this writing, the largest quantum gate processor has only 72 qubits.
                for (int i = 0; i < PasswordLength; i++)
                {
                    // Generate the bits using using the Q# operation
                    var passwordBits = PasswordGenerator.Run(qsim, numBits).Result;

                    // Concatenate the bits into a bit string and convert to an integer value
                    string bitString = string.Join("", passwordBits.Select(p => p.ToString() == "One" ? "1" : "0"));
                    int intValue = Convert.ToInt32(bitString, 2);

                    // Scale the result to the password character space and get the password character
                    int index = (int)(intValue * ValidPasswordChars.Length / Math.Pow(2, numBits));
                    passwordChars[i] = ValidPasswordChars[index];
                }
                // Create the password string from the quantum generated characters
                password = new string(passwordChars);
            }
            while (!DoesPasswordMeetRequirements(password));
            Console.WriteLine(password);
        }

        /// <summary>
        /// Returns true if the password meets requirements
        /// </summary>
        private static bool DoesPasswordMeetRequirements(string password)
        {
            // Check that password contains at least one character from at least three of the four character groups
            int representedGroupCount = 0;
            if (password.Any(c => PasswordCharsGroupA.Contains(c))) representedGroupCount++;
            if (password.Any(c => PasswordCharsGroupB.Contains(c))) representedGroupCount++;
            if (password.Any(c => PasswordCharsGroupC.Contains(c))) representedGroupCount++;
            if (password.Any(c => PasswordCharsGroupD.Contains(c))) representedGroupCount++;
            return representedGroupCount >= 3;
        }
    }
}