namespace nfs2iso2nfs.Helpers
{
    public class NfsHelper
    {
        public static void SplitNFSFile(string hif, int size)
        {
            using var nfs = new BinaryReader(File.OpenRead(hif));
            long baseStreamSize = nfs.BaseStream.Length;
            var i = 0;
            do
            {
                var nfsTemp = new BinaryWriter(File.OpenWrite(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "hif_" + string.Format("{0:D6}", i) + ".nfs"));
                nfsTemp.Write(nfs.ReadBytes(baseStreamSize > size ? size : size));
                size -= size;
                i++;
            } while (size > 0);
        }
        public static void EncryptNFS(string inFile, string outFile, byte[] key, int size, byte[] header)
        {
            using var er = new BinaryReader(File.OpenRead(inFile));
            using var ew = new BinaryWriter(File.OpenWrite(outFile));
            ew.Write(header);
            CryptNFS(er, ew, key, size, true);
        }
        public static void DecryptNFS(string inFile, string outFile, byte[] key, int size)
        {
            using var er = new BinaryReader(File.OpenRead(inFile));
            using var ew = new BinaryWriter(File.OpenWrite(outFile));
            CryptNFS(er, ew, key, size);
        }
        private static void CryptNFS(BinaryReader er, BinaryWriter ew, byte[] key, int size, bool encrypt = false)
        {
            var iv = ByteHelper.BuildZero(key.Length);

            byte[] block_iv = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1F, 0x00 };
            var sector = new byte[size];
            var timer = 0;
            var i = 0;
            //init size
            long leftSize = er.BaseStream.Length;
            do
            {
                if (timer == 8000)
                {
                    timer = 0;
                    i++;
                }
                timer++;
                sector = er.ReadBytes(leftSize > size ? size: (int)leftSize);

                if (ew.BaseStream.Position >= 0x18000)                               //use the different IVs if writing game partition data
                {
                    iv = block_iv;
                }

                if (ew.BaseStream.Position < 0x18000)                        // if encrypting and not game partition
                    sector = KeyHelper.CryptAes128Cbc(KeyHelper.CreateAes128Cbc(key, iv),sector, encrypt);                    // use zero IV

                if (ew.BaseStream.Position >= 0x18000)                       // if encrypting game partition
                {
                    sector = KeyHelper.CryptAes128Cbc(KeyHelper.CreateAes128Cbc(key, iv), sector, encrypt);                       // use different IV for each block
                    block_iv[15]++;                                                 // increment the value after writing
                    if (block_iv[15] == 0)                                          // and go further if necessary
                    {
                        block_iv[14]++;
                        if (block_iv[14] == 0)
                        {
                            block_iv[13]++;
                            if (block_iv[13] == 0)
                            {
                                block_iv[12]++;                                     // I suppose it's a 4 byte value...?
                            }                                                       // it won't ever happen anyway
                        }
                    }
                }

                //write it to outfile
                ew.Write(sector);

                //decrease remaining size
                leftSize -= size;

                //loop till end of file
            } while (leftSize > 0);
        }
        public static long[] EncryptManipulateIso(string inFile, string outFile, int sectorSize, byte[] commonKey)
        {
            using var er = new BinaryReader(File.OpenRead(inFile));
            using var ew = new BinaryWriter(File.OpenWrite(outFile));
            return ManipulateIso(er, ew, sectorSize, commonKey);
        }
        public static void DecryptManipulateIso(string inFile, string outFile, int sectorSize, byte[] commonKey)
        {
            using var er = new BinaryReader(File.OpenRead(inFile));
            using var ew = new BinaryWriter(File.OpenWrite(outFile));
            ManipulateIso(er, ew, sectorSize, commonKey, true);
        }
        private static long[] ManipulateIso(BinaryReader er, BinaryWriter ew, int sectorSize, byte[] commonKey, bool enc = false)
        {
            long[] sizeInfo = new long[2];
            ew.Write(er.ReadBytes(0x40000));

            var partitionTable = er.ReadBytes(0x20);
            ew.Write(partitionTable);
            int[,] partitionInfo = new int[2, 4];            //first coorfinate number of partitions, second offset of partition table

            for (byte i = 0; i < 4; i++)
            {
                partitionInfo[0, i] = partitionTable[0x0 + 0x8 * i] * 0x1000000 + partitionTable[0x1 + 0x8 * i] * 0x10000 + partitionTable[0x2 + 0x8 * i] * 0x100 + partitionTable[0x3 + 0x8 * i];
                if (partitionInfo[0, i] == 0)
                    partitionInfo[1, i] = 0;
                else partitionInfo[1, i] = (partitionTable[0x4 + 0x8 * i] * 0x1000000 + partitionTable[0x5 + 0x8 * i] * 0x10000 + partitionTable[0x6 + 0x8 * i] * 0x100 + partitionTable[0x7 + 0x8 * i]) * 0x4;
            }
            partitionInfo = ByteHelper.Sort(partitionInfo, 4);

            byte[][] partitionInfoTable = new byte[4][];
            var partitionOffsetList = new List<int>();
            long curPos = 0x40020;
            var k = 0;

            for (var i = 0; i < 4; i++)
            {
                if (partitionInfo[0, i] != 0)
                {
                    ew.Write(er.ReadBytes((int)(partitionInfo[1, i] - curPos)));
                    curPos += (partitionInfo[1, i] - curPos);
                    partitionInfoTable[i] = er.ReadBytes(0x8 * partitionInfo[0, i]);
                    curPos += (0x8 * partitionInfo[0, i]);
                    for (var j = 0; j < partitionInfo[0, i]; j++)
                        if (partitionInfoTable[i][0x7 + 0x8 * j] == 0) //check if game partition
                        {
                            partitionOffsetList.Add((partitionInfoTable[i][0x0 + 0x8 * j] * 0x1000000 + partitionInfoTable[i][0x1 + 0x8 * j] * 0x10000 + partitionInfoTable[i][0x2 + 0x8 * j] * 0x100 + partitionInfoTable[i][0x3 + 0x8 * j]) * 0x4);
                            k++;
                        }
                    ew.Write(partitionInfoTable[i]);
                }
            }
            var partitionOffsets = partitionOffsetList.ToArray();
            partitionOffsets = ByteHelper.Sort(partitionOffsets, partitionOffsets.Length);
            sizeInfo[0] = partitionOffsets[0];

            var iv = new byte[0x10];
            var decHashTable = new byte[0x400];
            var encHashTable = new byte[0x400];

            for (var i = 0; i < partitionOffsets.Length; i++)
            {
                ew.Write(er.ReadBytes((int)(partitionOffsets[i] - curPos)));
                curPos += (partitionOffsets[i] - curPos);
                ew.Write(er.ReadBytes(0x1BF));                              //Write start of partiton

                var enc_titlekey = er.ReadBytes(0x10);                   //read encrypted titlekey
                ew.Write(enc_titlekey);                                     //Write encrypted titlekey
                ew.Write(er.ReadBytes(0xD));                                //Write bytes till titleID

                var titleID = er.ReadBytes(0x8);                         //read titleID
                ew.Write(titleID);

                for (int j = 0; j < 0x10; j++)
                    if (j < 8)
                        iv[j] = titleID[j];
                    else iv[j] = 0x0;

                ew.Write(er.ReadBytes(0xC0));                               //Write bytes till end of ticket
                var partitionHeader = er.ReadBytes(0x1FD5C);
                var partitionSize = (long)0x4 * (partitionHeader[0x18] * 0x1000000 + partitionHeader[0x19] * 0x10000 + partitionHeader[0x1A] * 0x100 + partitionHeader[0x1B]);

                ew.Write(partitionHeader);                                  //Write bytes till start of partition data
                curPos += 0x20000;
                curPos += partitionSize;

                var titleKey = KeyHelper.CryptAes128Cbc(KeyHelper.CreateAes128Cbc(commonKey, iv), enc_titlekey);
                var Sector = new byte[sectorSize];

                //NFS to ISO
                //ISO to NFS
                while (partitionSize >= sectorSize)
                {
                    Array.Clear(iv, 0, 0x10);                                                // clear IV for encrypting hash table
                    if (enc)
                    {
                        decHashTable = er.ReadBytes(0x400);                                      // read raw hash table from nfs
                        encHashTable = KeyHelper.CryptAes128Cbc(KeyHelper.CreateAes128Cbc(titleKey, iv), decHashTable, enc);            // encrypt table
                        ew.Write(encHashTable);                                                  // write encrypted hash table to iso
                    }
                    else
                    {
                        encHashTable = er.ReadBytes(0x400);                                      // read encrypted hash table from iso
                        decHashTable = KeyHelper.CryptAes128Cbc(KeyHelper.CreateAes128Cbc(titleKey, iv), encHashTable, enc);           // decrypt table
                        ew.Write(decHashTable);                                                  // write decrypted hash table to nfs
                    }

                    //quit the loop if already at the end of input file or beyond (avoid the crash)
                    if (er.BaseStream.Position >= er.BaseStream.Length)
                        break;

                    Array.Copy(encHashTable, 0x3D0, iv, 0, 0x10);                            // get IV for encrypting the rest
                    Sector = er.ReadBytes(sectorSize - 0x400);
                    Sector = KeyHelper.CryptAes128Cbc(KeyHelper.CreateAes128Cbc(titleKey, iv), Sector, enc);                         // encrypt the remaining bytes

                    ew.Write(Sector);
                    partitionSize -= sectorSize;
                }
                sizeInfo[1] = curPos - sizeInfo[0];
            }
            if (enc)
            {
                var num = 0x118240000;
                long rest = (curPos > num ? 0x1FB4E0000 : num) - curPos;

                while (rest > 0)
                {
                    ew.Write(ByteHelper.BuildZero(rest > sectorSize ? sectorSize : (int)rest));
                    rest -= sectorSize;
                }
            }
            return sizeInfo;
        }

    }
}
