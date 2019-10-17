using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Btd6Launcher.Memory
{
    class MemoryUtils
    {

        public static unsafe Int64 AOBScan(byte* memory, Int64 memoryLength, byte[] scanFor)
        {
            for(Int64 scan = 0; scan < memoryLength; scan++)
            {
                for(Int64 i = 0; i < scanFor.Length; i++)
                {
                    if(memory[scan] != scanFor[i])
                    {
                        break;
                    }
                    else
                    {
                        if(i == scanFor.Length - 1)
                        {
                            return scan - scanFor.Length;
                        }
                        scan++;
                    }
                }
            }

            return 0;
        }

        // Gets bytes between 2 0x0's.
        public static unsafe byte[] BytesBetweenNull(byte* memory, Int64 memoryLength, Int64 it)
        {
            Int64 start = 0;
            Int64 end = 0;
            // Scan backwards for null
            for(Int64 i = it; i > 0; i--)
            {
                if(memory[i] == 0x0)
                {
                    start = i + 1;
                    break;
                }
            }

            for(Int64 i = it; i < memoryLength; i++)
            {
                if(memory[i] == 0x0)
                {
                    end = i;
                    break;
                }
            }

            byte[] insideBytes = new byte[end - start];

            for(Int64 i = start, k = 0; i < end; i++, k++)
            {
                insideBytes[k] = memory[i];
            }

            return insideBytes;

        }



    }
}
