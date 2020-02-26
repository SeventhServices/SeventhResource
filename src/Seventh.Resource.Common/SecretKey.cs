using Seventh.Resource.Common.Crypts;

namespace Seventh.Resource.Common
{
    public class SecretKey
    {
        static SecretKey()
        {
            Implement = new SecretKey();
        }

        public static SecretKey Implement { get; }

        public string SaveDataServices { get; }
            = SecretKeyCrypt.Decrypt("14f8d9ac10e72a75b3631de9dd91daadfb9205247b8c8e1184ea4934a5cb045e");
        public string DefaultId { get; }
            = SecretKeyCrypt.Decrypt("1b621d10d7fbf999c5f598d8bbde55aecdeb88aac7b61cbf924051f6592bde5f1387acfa09f6d4973ab5027876fac94693d8ff250e8ecacdccb6c39667508a69544905fdb86768dcc62d7fc9a157c47c");
        public string DefaultEncPid { get; }
            = SecretKeyCrypt.Decrypt("d542e828d4cfcc3406a4f0a38df24289");
        public string DecryptUuidKey { get; }
            = SecretKeyCrypt.Decrypt("e4c8808b4e82bd4c5af8bfa0c3c81b8fe1d32fd23bc09eec5ad14342a256eff0");
        public string DecryptAssetPrivateKey { get; }
            = SecretKeyCrypt.Decrypt("a0b8ef881e8ccb18dffc19fd830520207fcdd3b762cc2de2817ac771c85ea917");
        public string DecryptAssetPrivateKey2 { get; }
            = SecretKeyCrypt.Decrypt("0c96da71300ab54ddc38c63db8d9854194c2cc215b53d0a145b4d0933ad0a7bc");
        public string DecryptAssetNameHashKey { get; }
            = SecretKeyCrypt.Decrypt("ef8dde4587a5d2320012d6e67cd68ceb8ab14ee8131c99626d470193db364621");

    }
}