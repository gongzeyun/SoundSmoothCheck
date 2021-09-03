using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundCheck
{
    class Tools
    {
        public static void dumpRecordPCM(String filePath, byte[] pcm, int size)
        {
            BinaryWriter writer = new BinaryWriter(new FileStream(filePath, FileMode.Create | FileMode.Append));
            writer.Write(pcm, 0, size);
            writer.Flush();
            writer.Close();
        }
    }
}
