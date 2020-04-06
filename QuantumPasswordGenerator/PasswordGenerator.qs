namespace QuantumPasswordGenerator {
    open Microsoft.Quantum.Intrinsic;
    open Microsoft.Quantum.Canon;
    open Microsoft.Quantum.Arrays;
    open Microsoft.Quantum.Measurement;

    // Generates and returns a specified number of randomly set and measured qubits
    operation PasswordGenerator(nQubits : Int) : Result[] {
        using (qubits = Qubit[nQubits])  { // Allocate qubits
            ApplyToEach(H, qubits); // Put the qubits into superposition
            return ForEach(MResetZ, qubits); // Measure and return the answer
        }
    }
}