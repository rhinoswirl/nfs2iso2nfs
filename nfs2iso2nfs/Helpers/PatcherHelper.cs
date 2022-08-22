using System.Text;

namespace nfs2iso2nfs.Helpers
{
    public class PatcherHelper
    {
        public static bool CheckIfRev509(MemoryStream inputIos)
        {
            var bufferRev = new byte[4];
            byte[] revPattern = { 0x73, 0x76, 0x6E, 0x2D };                                  // search for "svn-"
            var revision = "";

            for (var offset = 0; offset < inputIos.Length - 4; offset++)
            {
                inputIos.Position = offset;                                                  // set position to advance byte by byte
                inputIos.Read(bufferRev, 0, 4);                                             // because we read 4 bytes at once

                if (ByteHelper.ByteArrayCompare(bufferRev, revPattern))                                // see if it matches
                {
                    inputIos.Read(bufferRev, 0, 4);
                    revision = Encoding.UTF8.GetString(bufferRev, 0, bufferRev.Length);
                    break;
                }
            }
            return revision == "r590";
        }
        public static void DontKeepLegit(MemoryStream inputIos)
        {
            byte[] buffer_4 = new byte[4];
            int patchCount = 0;
            byte[] oldHashCheck = { 0x20, 0x07, 0x23, 0xA2 };
            byte[] newHashCheck = { 0x20, 0x07, 0x4B, 0x0B };

            for (int offset = 0; offset < inputIos.Length - 4; offset++)
            {
                inputIos.Position = offset;                                                               // set position to advance byte by byte
                inputIos.Read(buffer_4, 0, 4);                                                            // because we read 4 bytes at once

                if (ByteHelper.ByteArrayCompare(buffer_4, oldHashCheck) || ByteHelper.ByteArrayCompare(buffer_4, newHashCheck))  // see if it matches one of the patterns
                {
                    inputIos.Seek(offset + 1, SeekOrigin.Begin);                                          // if it does, advance on byte further in
                    inputIos.WriteByte(0x00);                                                             // the output and write a zero

                    patchCount++;
                }
            }
        }
        public static void MapShoulderToTrigger(MemoryStream inputIos)
        {
            byte[] buffer_4 = new byte[4];
            int patchCount = 0;

            byte[] pattern1 = { 0x40, 0x05, 0x46, 0xA9 };
            byte[] patch1 = { 0x26, 0x80, 0x40, 0x06 };

            byte[] pattern2 = { 0x1C, 0x05, 0x40, 0x35 };
            byte[] patch2 = { 0x25, 0x40, 0x40, 0x05 };

            byte[] pattern3 = { 0x23, 0x7F, 0x1C, 0x02 };
            byte[] patch3 = { 0x46, 0xB1, 0x23, 0x20, 0x40, 0x03 };

            byte[] pattern4 = { 0x46, 0x53, 0x42, 0x18 };
            byte[] patch4 = { 0x23, 0x10, 0x40, 0x03 };

            byte[] pattern5 = { 0x1C, 0x05, 0x80, 0x22 };
            byte[] patch5 = { 0x25, 0x40, 0x80, 0x22, 0x40, 0x05 };

            for (var offset = 0; offset < inputIos.Length - 4; offset++)
            {
                inputIos.Position = offset;                                             // set position to advance byte by byte
                inputIos.Read(buffer_4, 0, 4);                                          // because we read 4 bytes at once

                if (ByteHelper.ByteArrayCompare(buffer_4, pattern1))                                // see if it matches
                {
                    inputIos.Seek(offset, SeekOrigin.Begin);                            // seek
                    inputIos.Write(patch1, 0, 4);                                       // and then patch

                    patchCount++;
                }

                if (ByteHelper.ByteArrayCompare(buffer_4, pattern2))                                // see if it matches
                {
                    inputIos.Seek(offset, SeekOrigin.Begin);                            // seek
                    inputIos.Write(patch2, 0, 4);                                       // and then patch

                    patchCount++;
                }

                if (ByteHelper.ByteArrayCompare(buffer_4, pattern3))                                // see if it matches
                {
                    inputIos.Seek(offset, SeekOrigin.Begin);                            // seek
                    inputIos.Write(patch3, 0, 6);                                       // and then patch

                    patchCount++;
                }

                if (ByteHelper.ByteArrayCompare(buffer_4, pattern4))                                // see if it matches
                {
                    inputIos.Seek(offset, SeekOrigin.Begin);                            // seek
                    inputIos.Write(patch4, 0, 4);                                       // and then patch

                    patchCount++;
                }

                if (ByteHelper.ByteArrayCompare(buffer_4, pattern5))                                // see if it matches
                {
                    inputIos.Seek(offset, SeekOrigin.Begin);                            // seek
                    inputIos.Write(patch5, 0, 6);                                       // and then patch

                    patchCount++;
                }
            }
        }
        public static void EnableWiiRemoteEmulation(MemoryStream inputIos)
        {
            byte[] buffer_8 = new byte[8];
            int patchCount = 0;
            byte[] pattern = { 0x16, 0x13, 0x1C, 0x02, 0x40, 0x9A, 0x1C, 0x13 };
            byte[] patch = { 0x23, 0x00 };

            for (var offset = 0; offset < inputIos.Length - 6; offset++)
            {
                inputIos.Position = offset;                                              // set position to advance byte by byte
                inputIos.Read(buffer_8, 0, 8);                                           // because we read 8 bytes at once

                if (ByteHelper.ByteArrayCompare(buffer_8, pattern))                                  // see if it matches
                {
                    inputIos.Seek(offset, SeekOrigin.Begin);                             // seek
                    inputIos.Write(patch, 0, 2);                                         // and then patch

                    patchCount++;
                }
            }
        }
        public static void EnableHorizontalWiiRemoteEmulation(MemoryStream inputIos)
        {
            byte[] buffer_8 = new byte[8];
            int patchCount = 0;
            byte[] pattern = { 0x4A, 0x71, 0x42, 0x13, 0xD0, 0xD2, 0x9B, 0x00 };

            for (int offset = 0; offset<inputIos.Length - 8; offset++)
            {
                inputIos.Position = offset;                                              // set position to advance byte by byte
                inputIos.Read(buffer_8, 0, 8);                                           // because we read 8 bytes at once

                if (ByteHelper.ByteArrayCompare(buffer_8, pattern))                                  // see if it matches
                {
                    inputIos.Seek(offset + 0x07, SeekOrigin.Begin);
                    inputIos.WriteByte(0x02);                                            // dpad left -> down
                    patchCount++;

                    inputIos.Seek(offset + 0x0F, SeekOrigin.Begin);
                    inputIos.WriteByte(0x03);                                            // dpad right -> up
                    patchCount++;

                    inputIos.Seek(offset + 0x1D, SeekOrigin.Begin);
                    inputIos.WriteByte(0x01);                                            // dpad down -> right
                    patchCount++;

                    inputIos.Seek(offset + 0x2B, SeekOrigin.Begin);
                    inputIos.WriteByte(0x00);                                            // dpad up -> left

                    patchCount++;

                    inputIos.Seek(offset + 0x65, SeekOrigin.Begin);
                    inputIos.WriteByte(0x07);                                            // B -> 2
                    patchCount++;

                    inputIos.Seek(offset + 0x75, SeekOrigin.Begin);
                    inputIos.WriteByte(0x06);                                            // A -> 1
                    patchCount++;

                    inputIos.Seek(offset + 0x85, SeekOrigin.Begin);
                    inputIos.WriteByte(0x04);                                            // 1 -> B
                    patchCount++;

                    inputIos.Seek(offset + 0x95, SeekOrigin.Begin);
                    inputIos.WriteByte(0x05);                                            // 2 -> A

                    patchCount++;
                }
            }
        }
        public static void EnableProperInputInHomebrew(MemoryStream inputIos)
        {
            byte[] buffer_4 = new byte[4];                                                    // buffer for 4-byte arrays
            byte[] buffer_8 = new byte[8];
            int patchCount = 0;

            // disable AHBPROT
            byte[] pattern_ahbprot = { 0xD0, 0x0B, 0x23, 0x08, 0x43, 0x13, 0x60, 0x0B };
            byte[] patch_ahbprot = { 0x46, 0xC0 };

            for (int offset = 0; offset < inputIos.Length - 8; offset++)
            {
                inputIos.Position = offset;                                              // set position to advance byte by byte
                inputIos.Read(buffer_8, 0, 8);                                           // because we read 8 bytes at once

                if (ByteHelper.ByteArrayCompare(buffer_8, pattern_ahbprot))                          // see if it matches
                {
                    inputIos.Seek(offset, SeekOrigin.Begin);                             // seek to offset
                    inputIos.Write(patch_ahbprot, 0, 2);                                 // and then patch

                    patchCount++;
                }
            }

            //disable MEMPROT
            byte[] pattern_memprot = { 0x01, 0x94, 0xB5, 0x00, 0x4B, 0x08, 0x22, 0x01 };
            byte[] patch_memprot = { 0x22, 0x00 };

            for (int offset = 0; offset < inputIos.Length - 8; offset++)
            {
                inputIos.Position = offset;                                              // set position to advance byte by byte
                inputIos.Read(buffer_8, 0, 8);                                           // because we read 8 bytes at once

                if (ByteHelper.ByteArrayCompare(buffer_8, pattern_memprot))                          // see if it matches
                {
                    inputIos.Seek(offset + 6, SeekOrigin.Begin);                         // seek to offset
                    inputIos.Write(patch_memprot, 0, 2);                                 // and then patch

                    patchCount++;
                }
            }

            // nintendont 1
            byte[] pattern_nintendont_1 = { 0xB0, 0xBA, 0x1C, 0x0F };
            byte[] patch_nintendont_1 = { 0xE5, 0x9F, 0x10, 0x04, 0xE5, 0x91, 0x00, 0x00, 0xE1, 0x2F, 0xFF, 0x10, 0x12, 0xFF, 0xFF, 0xE0 };
            for (int offset = 0; offset < inputIos.Length - 4; offset++)
            {
                inputIos.Position = offset;                                              // set position to advance byte by byte
                inputIos.Read(buffer_4, 0, 4);                                           // because we read 4 bytes at once

                if (ByteHelper.ByteArrayCompare(buffer_4, pattern_nintendont_1))                     // if it matches
                {
                    inputIos.Seek(offset - 12, SeekOrigin.Begin);                        // seek to offset
                    inputIos.Write(patch_nintendont_1, 0, 16);                           // and then patch

                    patchCount++;
                }
            }

            //nintendont 2
            byte[] pattern_nintendont_2 = { 0x68, 0x4B, 0x2B, 0x06 };
            byte[] patch_nintendont_2 = { 0x49, 0x01, 0x47, 0x88, 0x46, 0xC0, 0xE0, 0x01, 0x12, 0xFF, 0xFE, 0x00, 0x22, 0x00, 0x23, 0x01, 0x46, 0xC0, 0x46, 0xC0 };
            for (int offset = 0; offset < inputIos.Length - 4; offset++)
            {
                inputIos.Position = offset;                                              // set position to advance byte by byte
                inputIos.Read(buffer_4, 0, 4);                                           // because we read 4 bytes at once

                if (ByteHelper.ByteArrayCompare(buffer_4, pattern_nintendont_2))                     // if it matches
                {
                    inputIos.Seek(offset, SeekOrigin.Begin);                             // seek to offset
                    inputIos.Write(patch_nintendont_2, 0, 20);                           // and then patch

                    patchCount++;
                }
            }

            //nintendont 3
            byte[] pattern1_nintendont_3 = { 0x0D, 0x80, 0x00, 0x00, 0x0D, 0x80, 0x00, 0x00 };
            byte[] pattern2_nintendont_3 = { 0x00, 0x00, 0x00, 0x02 };
            byte[] patch_nintendont_3 = { 0x00, 0x00, 0x00, 0x03 };
            for (int offset = 0; offset < inputIos.Length - 8; offset++)
            {
                inputIos.Position = offset;                                              // set position to advance byte by byte
                inputIos.Read(buffer_8, 0, 8);                                           // because we read 8 bytes at once

                if (ByteHelper.ByteArrayCompare(buffer_8, pattern1_nintendont_3))                    // if it matches
                {
                    inputIos.Seek(offset + 0x10, SeekOrigin.Begin);
                    inputIos.Read(buffer_4, 0, 4);
                    if (ByteHelper.ByteArrayCompare(buffer_4, pattern2_nintendont_3))                // if it matches
                    {
                        inputIos.Seek(offset + 0x10, SeekOrigin.Begin);                    // seek to offset
                        inputIos.Write(patch_nintendont_3, 0, 4);                        // and then patch

                        patchCount++;
                    }
                }
            }
        }
        public static void WiiMotePassthrough(MemoryStream inputIos)
        {
            byte[] buffer_4 = new byte[4];                                                    // buffer for 4-byte arrays
            byte[] buffer_8 = new byte[8];
            int patchCount = 0;

            //wiimote passthrough
            byte[] pattern_passthrough = { 0x20, 0x4B, 0x01, 0x68, 0x18, 0x47, 0x70, 0x00 };
            byte[] patch_passthrough = { 0x20, 0x00 };

            for (int offset = 0; offset < inputIos.Length - 8; offset++)
            {
                inputIos.Position = offset;                                              // set position to advance byte by byte
                inputIos.Read(buffer_8, 0, 8);                                           // because we read 8 bytes at once

                if (ByteHelper.ByteArrayCompare(buffer_8, pattern_passthrough))                      // if it matches
                {
                    inputIos.Seek(offset + 3, SeekOrigin.Begin);                         // seek to offset
                    inputIos.Write(patch_passthrough, 0, 2);                             // and then patch

                    patchCount++;
                }
            }

            // the custom function
            byte[] pattern_custom_func = { 0x28, 0x00, 0xD0, 0x03, 0x49, 0x02, 0x22, 0x09 };
            byte[] patch_custom_func = { 0xF0, 0x04, 0xFF, 0x21, 0x48, 0x02, 0x21, 0x09, 0xF0, 0x04, 0xFE, 0xF9 };

            for (int offset = 0; offset < inputIos.Length - 8; offset++)
            {
                inputIos.Position = offset;                                              // set position to advance byte by byte
                inputIos.Read(buffer_8, 0, 8);                                           // because we read 8 bytes at once

                if (ByteHelper.ByteArrayCompare(buffer_8, pattern_custom_func))                      // if it matches
                {
                    inputIos.Seek(offset, SeekOrigin.Begin);                             // seek to offset
                    inputIos.Write(patch_custom_func, 0, 12);                            // and then patch

                    patchCount++;
                }
            }

            // call custom function
            byte[] pattern_custom_call = { 0xF0, 0x01, 0xFA, 0xB9 };
            byte[] patch_custom_call = { 0xF7, 0xFC, 0xFB, 0x95 };

            for (int offset = 0; offset < inputIos.Length - 4; offset++)
            {
                inputIos.Position = offset;                                              // set position to advance byte by byte
                inputIos.Read(buffer_4, 0, 4);                                           // because we read 4 bytes at once

                if (ByteHelper.ByteArrayCompare(buffer_4, pattern_custom_call))                      // if it matches
                {
                    inputIos.Seek(offset, SeekOrigin.Begin);                             // seek to offset
                    inputIos.Write(patch_custom_call, 0, 4);                             // and then patch

                    patchCount++;
                }
            }
        }
        public static void InstantCC(MemoryStream inputIos)
        {
            byte[] buffer_8 = new byte[8];
            int patchCount = 0;
            byte[] pattern = { 0x78, 0x93, 0x21, 0x10, 0x2B, 0x02, 0xD1, 0xB7 };
            byte[] patch = { 0x78, 0x93, 0x21, 0x10, 0x2B, 0x02, 0x46, 0xC0 };

            for (int offset = 0; offset < inputIos.Length - 8; offset++)
            {
                inputIos.Position = offset;                                              // set position to advance byte by byte
                inputIos.Read(buffer_8, 0, 8);                                           // because we read 8 bytes at once

                if (ByteHelper.ByteArrayCompare(buffer_8, pattern))                                  // see if it matches
                {
                    inputIos.Seek(offset, SeekOrigin.Begin);                             // seek
                    inputIos.Write(patch, 0, 8);

                    patchCount++;
                }
            }
        }
        public static void NoCC(MemoryStream inputIos)
        {
            byte[] buffer_8 = new byte[8];
            int patchCount = 0;
            byte[] pattern = { 0x78, 0x93, 0x21, 0x10, 0x2B, 0x02, 0xD1, 0xB7 };
            byte[] patch = { 0x78, 0x93, 0x21, 0x10, 0x2B, 0x02, 0xE0, 0xB7 };

            for (int offset = 0; offset < inputIos.Length - 8; offset++)
            {
                inputIos.Position = offset;                                              // set position to advance byte by byte
                inputIos.Read(buffer_8, 0, 8);                                           // because we read 8 bytes at once

                if (ByteHelper.ByteArrayCompare(buffer_8, pattern))                                  // see if it matches
                {
                    inputIos.Seek(offset, SeekOrigin.Begin);                             // seek
                    inputIos.Write(patch, 0, 8);

                    patchCount++;
                }
            }

        }
    }
}
