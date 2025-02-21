// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("4OTY0kFrWrjeVZwigPgxyPH/uHrPNixgNuH76NU9BYYgB8mdUBcggp0eEB8vnR4VHZ0eHh+BBa9dcc0gWHlMtApiRmcdRJ0a/tZsHMEQPJVMV+jea4zkW+/p4MIKNiQsO+JXGw1eoZqmMy56tlc4Lmt12PmVhF9kTeZxL/sFDEoN4RNgmS3Ekcw9n8BWB3QaZdR9yhnqiD0hhbsVPY9SBC+dHj0vEhkWNZlXmegSHh4eGh8cYomUAaiDfG8GTsAnfRDkOy6ehb4cOF+Jvph2ecozs0OPsNGGnYezB+okuqwjhisOUSPMWDaw47mkvjanH17hG1IiaXLLlvWBxoQj51RBhSoTlASsSDSVkpsNJf9tFYneJnhvav9tXUl/zzJLEB0cHh8e");
        private static int[] order = new int[] { 8,10,10,11,10,12,7,10,8,9,10,13,12,13,14 };
        private static int key = 31;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
