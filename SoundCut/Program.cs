using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundCut
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = "song";
            byte[] head = new byte[44];
            using (var reader = new BinaryReader(new FileStream($@"..\..\..\..\{filename}.wav", FileMode.Open)))
            {
                var binaryWriter = new BinaryWriter(new FileStream($@"..\..\..\..\{filename}cut.wav", FileMode.Create));
                var length = reader.BaseStream.Length;
                int pos = 0;
                int l = 1 << 20;//*2, each sound takes 2 bytes
                int k = l * 2 + 44 - 8;
                while (pos < head.Length)
                {
                    var readbyte = reader.ReadByte();
                    head[pos] = readbyte;
                    pos++;
                    
                }
                head[4] = (byte)(k % 256);
                k = k / 256;
                head[5] = (byte)(k % 256);
                k = k / 256;
                head[6] = (byte)(k % 256);
                k = k / 256;
                head[7] = 0;
                head[36] = Convert.ToByte('d');
                head[37] = Convert.ToByte('a');
                head[38] = Convert.ToByte('t');
                head[39] = Convert.ToByte('a');

                k = l * 2;
                head[40] = (byte)(k % 256);
                k = k / 256;
                head[41] = (byte)(k % 256);
                k = k / 256;
                head[42] = (byte)(k % 256);
                head[43] = 0;
                head.ForEach((b) => binaryWriter.Write(b));
                k = l * 2;
                int shift = l * 2;
                for (int i = 0; i < shift; i++)
                {
                    reader.ReadByte();
                }
                    for (int i = 0; i < k; i++)
                {
                    binaryWriter.Write(reader.ReadByte());
                }
                binaryWriter.Close();
            }
            
        }
    }
}
