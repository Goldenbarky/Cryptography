using System.Numerics;

class Program {
    public static void Main(string[] args) {
        Console.Write("What would you like to do?\n1) Decrypt a message\n2) Calculate a GCD\n3) Calculate a modulo\n4) Compute a LFSR cycle\n5) Compute the Extended Euclidian Algorithm\n6) Determine the classification of a LFSR polynomial\n7) Determine the primitive roots for an integer\n8) Compute polynomial multiplication\n");
        string response = Console.ReadLine();
        
        string[] nums;
        switch (response) {
            case "1":
                Decryption();
                break;
            case "2":
                Console.Write("Please enter the numbers (ex. 3, 4): ");
                nums = Console.ReadLine().Split(", ");
                Console.WriteLine(GCD(int.Parse(nums[0]), int.Parse(nums[1])));
                break;
            case "3":
                Console.Write("Please enter the numbers (ex. 7 % 2): ");
                nums = Console.ReadLine().Split(" % ");
                Console.WriteLine(Modulo(int.Parse(nums[0]), int.Parse(nums[1])));
                break;
            case "4":
                Console.Write("Enter initial bits (110): ");
                string initial_keystream = Console.ReadLine();
                Console.Write("Enter the polynomial (0,1,3): ");
                string polynomialString = Console.ReadLine();
                string coefficients = PolynomialBitString(polynomialString);
                if(initial_keystream.Length != coefficients.Length)
                    Console.WriteLine("The two inputs must be the same length");
                else {
                    (string keystream, HashSet<int> values) = Linear_Feedback_Shift_Register(initial_keystream, coefficients, true);
                    //Console.WriteLine($"The keystream is {keystream}");
                }
                break;
            case "5":
                Console.Write("Enter the numbers (ex. 31, 6): ");
                nums = Console.ReadLine().Split(", ");
                Console.WriteLine(ExtendedEuclidianAlgorithm(int.Parse(nums[0]), int.Parse(nums[1])));
                break;
            case "6":
                Console.Write("Enter the polynomial (eg. (0,1,2)): ");
                string polynomial = Console.ReadLine();
                LFSR_Classification(polynomial);
                break;
            case "7":
                Console.Write("Enter the number to generate primitive roots for (ex. 7): ");
                int num = int.Parse(Console.ReadLine());
                Console.Write("Print orders? (y/n): ");
                bool print = Console.ReadLine() == "y";
                PrimitiveRoots(num, print).ForEach(x=>Console.Write($"{x}, "));
                Console.WriteLine();
                break;
            case "8":
                string input = "";
                while (true) {
                    Console.Write("Enter the polynomials in hex (ex. 72 * 2F ): ");
                    input = Console.ReadLine();
                    if(input == "q") break;
                    nums = input.Split(" * ");
                    Console.WriteLine(PolynomialMultiplication(nums[0], nums[1]));
                }
                break;
            default:
                Console.WriteLine("That is not an option");
                break;
        }
    }

    public static void Decryption() {
        Console.Write("What type of cipher would you like to decrypt? ");
        string response = Console.ReadLine();
        Console.Write("Please enter the text to be decrypted: ");
        string ciphertext = Console.ReadLine();

        if(response == "shift") Shift_Decryption(ciphertext);
        else if (response == "affine") Affine_Decrpytion(ciphertext);
    }

    public static void Shift_Decryption(string ciphertext) {
        for(int i = 1; i < 26; i++) {
            string plaintext = "";

            ciphertext.ToList().ForEach(x=>{
                if((int)x == 32) plaintext += x;
                else plaintext += (char)(x+i-122+96);
            });

            Console.WriteLine($"Attempt {i}: {plaintext}");
        }
    }

    //Computes the lfsr and returns a tuple of the keystream cycle and the visited values in the cycle
    public static (string, HashSet<int>) Linear_Feedback_Shift_Register(string initial_keystream, string coefficients, bool printToConsole = false) {
        int[] paths = Bit_String(coefficients);

        Queue<int> registers = new Queue<int>();
        foreach(int bit in Bit_String(initial_keystream)) {
            registers.Enqueue(bit);
        }

        List<int> keystream = new List<int>();
        HashSet<int> prev_registers = new HashSet<int>();

        int i = 0;
        while(true) {
            int prev = 0;
            int max_cycle = (int) Math.Pow(2, paths.Length) - 1;

            List<int> register_list = registers.ToList();

            //Reverse the inputs for human readability
            register_list.Reverse();
            
            string register_string = string.Join("", register_list);
            if(printToConsole) Console.WriteLine(register_string);

            if(!prev_registers.Add(Convert.ToInt32(register_string, 2))) {
                if(printToConsole) Console.WriteLine($"Sequence repeats after {i} cycles. Max cycle length for this lfsr is {max_cycle}");
                break;
            }

            for(int j = 0; j < paths.Length; j++) {
                prev = (paths[j] & registers.ToList()[j]) ^ prev;
            }
            keystream.Add(registers.Dequeue());
            registers.Enqueue(prev);

            i++;
        }

        return (string.Join("", keystream), prev_registers);
    }

    //Calculates the type of polynomial the lfsr represents
    public static void LFSR_Classification(string polynomial) {
        string polynomialCoefficients = PolynomialBitString(polynomial);
        HashSet<int> tested_values = new HashSet<int>();
        int longestLength = -1;
        int maxCycleLength = (int)Math.Pow(2, polynomialCoefficients.Length) - 1;

        for(int i = 1; i <= maxCycleLength; i++) {
            if(tested_values.Contains(i)) continue;

            string initial_keystream = Convert.ToString(i, 2);

            //Prepend 0s until keystream is same length as the polynomial
            while(initial_keystream.Length < polynomialCoefficients.Count()) {
                initial_keystream = initial_keystream.Insert(0, "0");
            }

            (string keystream, HashSet<int> visited_values) = Linear_Feedback_Shift_Register(initial_keystream, polynomialCoefficients);

            visited_values.ToList().ForEach(x=>tested_values.Add(x));

            if(longestLength == -1) longestLength = keystream.Length;
            
            if(longestLength == maxCycleLength) {
                Console.WriteLine($"The polynomial {polynomial} is primitive");
                return;
            } else if (longestLength != keystream.Length) {
                Console.WriteLine($"The polynomial {polynomial} is reducible");
                return;
            }
        }

        Console.WriteLine($"The polynomial {polynomial} is irreducible");
    }

    //Returns a bitwise representation of the polynomial
    public static string PolynomialBitString(string polynomial) {
        string[] polynomialArray = polynomial.Trim(new[] {'(', ')'}).Split(",");
        int polynomialDegree = int.Parse(polynomialArray[polynomialArray.Count()-1]);
        string polynomialCoefficients = "";

        for(int i = 0; i < polynomialDegree; i++) {
            polynomialCoefficients += (polynomialArray.Contains(i.ToString())) ? "1" : "0";
        }

        return polynomialCoefficients;
    }

    //Turns a string of bits into an array of integers for bitwise operations
    public static int[] Bit_String(string str) {
        int[] bits = new int[str.Length];

        for(int i = 0; i < str.Length; i++) {
            if(str[i] != '0' && str[i] != '1') return null;
            bits[i] = (str[i] == '1') ? 1 : 0;
        }

        return bits;
    }

    public static void Affine_Decrpytion(string ciphertext) {
        Console.Write("What is b? ");
        int b = int.Parse(Console.ReadLine());
        Console.Write("What is a inverse? ");
        int a = int.Parse(Console.ReadLine());

        string plaintext = "";

        ciphertext.ToList().ForEach(x=>{
            plaintext += (char) (Modulo((a * ((x-97)-b)), 26) + 97);
        });

        Console.WriteLine(plaintext);
    }

    public static int Modulo(int a, int b) {
        return ((a % b) + b) % b;
    }

    public static int GCD(int a, int b) {
        if(b == 0)
            return a;
        else
            return GCD(b, Modulo(a, b));
    }

    //Returns the multiplicative inverse for b mod a
    public static int ExtendedEuclidianAlgorithm(int a, int b) {
        List<int> s = new List<int> {1, 0};
        List<int> t = new List<int> {0, 1};
        List<int> r = new List<int> {a, b};
        List<int> q = new List<int> {0};
        int i = 1;

        do {
            i++;
            r.Add(Modulo(r[i-2], r[i-1])); 
            q.Add((r[i-2] - r[i]) / r[i-1]);
            s.Add(s[i-2] - (q[i-1] * s[i-1]));
            t.Add(t[i-2] - (q[i-1] * t[i-1]));
        } while (r[i] != 0);

        return Modulo(t[i-1], a);
    }

    //Returns the primitive roots for an integer
    //Takes a boolean as input as whether to print out the order of each root
    public static List<int> PrimitiveRoots(int n, bool printOrders) {
        List<int> generators = new List<int>();
        for(int i = 1; i < n; i++) {
            int order;
            for(order = 1; order < n; order++) {
                if(BigInteger.ModPow(i, order, n) == 1)
                    break;
            }
            if(printOrders) {
                if(order == n)
                    Console.WriteLine($"{i}: (GCD) {GCD(i, n)}");
                else
                    Console.WriteLine($"{i}: order {order}");
            }
                
            if(order == n - 1)
                generators.Add(i);
        }

        return generators;
    }

    public static string PolynomialMultiplication(string hex1, string hex2) {
        int num1 = Convert.ToInt32(hex1, 16);
        int num2 = Convert.ToInt32(hex2, 16);

        int finalNum = 0;

        //Compute multipliation mod 2
        for(int i = 128; i >= 1; i /= 2) {
            finalNum = finalNum ^ ((i & num1) * num2);
        }

        //Mod by AES irriducible
        while(finalNum > 256) {
            int bitStringLength = Convert.ToString(finalNum, 2).Length-1;
            int AESIrreducible = Convert.ToInt32("100011011", 2) << bitStringLength-8;
            finalNum = finalNum ^ AESIrreducible;
        }

        //Convert to hex
        return finalNum.ToString("X");
    }
}