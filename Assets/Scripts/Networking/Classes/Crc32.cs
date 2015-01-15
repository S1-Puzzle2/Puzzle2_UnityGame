using System;
using System.IO;

namespace Vbaccelerator.Components.Algorithms
{
   /// <summary>
   /// Calculates a 32bit Cyclic Redundancy Checksum (CRC) using the
   /// same polynomial used by Zip.
   /// </summary>
   public class CRC32
   {
     
        public static uint calcCrc32(byte[] bytes) {

       /**************************************************************************
        *  Using direct calculation
        **************************************************************************/
            uint crc  = 0xFFFFFFFF;       // initial contents of LFBSR
            uint poly = 0xEDB88320;   // reverse polynomial

            foreach (byte b in bytes) {
                uint temp = (crc ^ b) & 0xff;

                // read 8 bits one at a time
                for (int i = 0; i < 8; i++) {
                    if ((temp & 1) == 1) temp = (temp >> 1) ^ poly;
                    else                 temp = (temp >> 1);
                }
                crc = (crc >> 8) ^ temp;
            }

            // flip bits
            crc = crc ^ 0xffffffff;

            return crc;
       }



   }
}