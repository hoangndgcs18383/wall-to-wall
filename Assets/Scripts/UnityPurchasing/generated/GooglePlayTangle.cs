// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("bOETguRvO7b/mX043v7LLjBP09fGgYLHrQBYViu/C6o3BA0kXZYtZc1OQE9/zU5FTc1OTk/qEId4WiU9SMr5y4vC22IHXYov8GMsM8snZAPopiNcoQGgkB5AXgkdUZUXQnJDIWvTlDlfMk/D5nr5+lrSnYnqDrxQHbmsN51QruwSyZ3c/pmt8pCsSwA9Kw4f/z3BUCXs5GrqFhzhr1VR+J9rbe1D4RyAg5KskInhIEMizZ25cu6S0FCz5FaP9Ic9tkigS7dJEoEJ1fn6QT+efj1X4Ad+cSfYju7LK3/NTm1/QklGZckHybhCTk5OSk9McyxspYC9OjuoG73TQkhNDCcIJ2bBs+4RwCdZJHpCAWycstvGiHJNcmeYnKu9zK7aEE1MTk9O");
        private static int[] order = new int[] { 13,6,6,10,8,12,10,8,12,10,10,13,12,13,14 };
        private static int key = 79;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
