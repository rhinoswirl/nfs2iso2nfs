namespace nfs2iso2nfs.Helpers
{
    public class ByteHelper
    {
        public static byte[] GetHeader(string inFile)
        {
            using var file = new BinaryReader(File.OpenRead(inFile));
            return file.ReadBytes(0x200);
        }
        public static byte[] BuildZero(int size)
        {
            var iv = new byte[size];
            for (int i = 0; i < size; i++)
                iv[i] = 0;
            return iv;
        }
        public static int[,] Sort(int[,] list, int size)
        {
            int max = 0;
            int maxIndex = 0;
            int temp;
            for (int j = 0; j < size; j++)
            {
                for (int i = 0; i < size - j; i++)
                    if (list[1, i] > max)
                    {
                        max = list[1, i];
                        maxIndex = i;
                    }
                temp = list[0, size - j - 1];
                list[0, size - j - 1] = list[0, maxIndex];
                list[0, maxIndex] = temp;
                temp = list[1, size - j - 1];
                list[1, size - j - 1] = list[1, maxIndex];
                list[1, maxIndex] = temp;
            }
            return list;
        }
        public static int[] Sort(int[] list, int size)
        {
            int max = 0;
            int maxIndex = 0;
            int temp;
            for (int j = 0; j < size; j++)
            {
                for (int i = 0; i < size - j; i++)
                    if (list[i] > max)
                    {
                        max = list[i];
                        maxIndex = i;
                    }
                temp = list[size - j - 1];
                list[size - j - 1] = list[maxIndex];
                list[maxIndex] = temp;
            }
            return list;
        }

        public static bool ByteArrayCompare(byte[] b1, byte[] b2)
        {
            return b1.SequenceEqual(b2);
        }
    }
}
