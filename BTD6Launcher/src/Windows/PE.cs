using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

//
// I have set up regions and plenty of other shit on this file. I recommend you use them
// 


namespace Btd6Launcher.Windows.PE
{   
    //Looks cleaner, unessesary.
    using UInt8 = Byte;



    //
    // Pe class is here for all Microsoft Portable Executable format needs.
    // Current uses:
    //      Getting the exports of an image, used for yoinking Unity's IL2cpp Methods.
    //      
    //

    public static class PE
    {
        #region constants

        public const UInt16 IMAGE_DOS_SIGNATURE = 0x5A4d; //MZ

        public const UInt32 IMAGE_NT_PEHEADER_SIGNATURE = 0x4550; //PE

        public const UInt32 IMAGE_NT_OPTIONAL_HDR32_MAGIC = 0x10B;
        public const UInt32 IMAGE_NT_OPTIONAL_HDR64_MAGIC = 0x10B;


        #endregion



        #region PEStructs

        //
        // So something that's actually pretty fucking retarded about C# is that structs have access modifiers.
        // Which means you can have a private property on a struct, which makes the variable entirely unaccessable.
        // So no you have to deal with a fucking ugly public acces modifier on every property in every struct.
        //

        // Region that contains all them PE file structs and enums.
        // 
        // Most information taken from https://docs.microsoft.com/en-us/windows/desktop/debug/pe-format
        // Some information also taken from the <winnt.h> C++ header, starting at _IMAGE_NT_HEADERS64
        //

        #region PeFile
        public enum MachineTypes : UInt16
        {
            IMAGE_FILE_MACHINE_UNKNOWN = 0x0000, //Asumed to run on any machine type
            IMAGE_FILE_MACHINE_AM33 = 0x01D3, //Matsushita AM33
            IMAGE_FILE_MACHINE_AMD64 = 0x8664, //X64
            IMAGE_FILE_MACHINE_ARM = 0x01C0, //ARM Little endian
            IMAGE_FILE_MACHINE_ARM64 = 0xAA64, //ARM64 Little endian
            IMAGE_FILE_MACHINE_ARMNT = 0x01C4, //ARM Thumb-2 little endian
            IMAGE_FILE_MACHINE_EBC = 0x0EBC, //EFI byte code
            IMAGE_FILE_MACHINE_I386 = 0x014C, //Intel 386 or later proccessors and compatible proccessors
            IMAGE_FILE_MACHINE_IA64 = 0x0200, //Intel Itanium processor family
            IMAGE_FILE_MACHINE_M32R = 0x9041, //Mitsubishi M32R little endian
            IMAGE_FILE_MACHINE_MIPS16 = 0x0266, //MIPS16
            IMAGE_FILE_MACHINE_MIPSFPU = 0x0366, //MIPS with FPU
            IMAGE_FILE_MACHINE_MIPSFPU16 = 0x0466, //MIPS 16 with FPU
            IMAGE_FILE_MACHINE_POWERPC = 0x01F0, //Power PC little endian
            IMAGE_FILE_MACHINE_POWERPCFP = 0x01F1, //Power PC with floating point support
            IMAGE_FILE_MACHINE_R4000 = 0x0166, //MIPS little endian
            IMAGE_FILE_MACHINE_RISCV32 = 0x5032, //RISC-V 32 bit
            IMAGE_FILE_MACHINE_RISCV64 = 0x5064, //RISC-V 64 bit
            IMAGE_FILE_MACHINE_RISCV128 = 0x5128, //RISC-V 128 bit
            IMAGE_FILE_MACHINE_SH3 = 0x01A2, //Hitachi SH3
            IMAGE_FILE_MACHINE_SH3DSP = 0x01A3, //Hitachi SH3 DSP
            IMAGE_FILE_MACHINE_SH4 = 0x01A6, //Hitachi SH4
            IMAGE_FILE_MACHINE_SH5 = 0x01A8, //Hitachi SH5
            IMAGE_FILE_MACHINE_THUMB = 0x01C2, //Thumb
            IMAGE_FILE_MACHINE_WCEMIPSV2 = 0x0169, //MIPS little-endian WCE v2
        };

        [Flags]
        public enum Characteristics : UInt16
        { //BitField
            IMAGE_FILE_RELOCS_STRIPPED = 0x0001, //Image only, Indicates that it does not contain relocations and must be loaded at it's base address	
            IMAGE_FILE_EXECUTABLE_IMAGE = 0x0002, //Image only. Indicated that it is valid and can be run, causes link error if set to 0				
            IMAGE_FILE_LINE_NUMS_STRIPPED = 0x0004, //Deprecated																						
            IMAGE_FILE_LOCAL_SYMS_STRIPPED = 0x0008, //Deprecated																						
            IMAGE_FILE_AGRESSIVE_WS_TRIM = 0x0010, //Deprecated																							
            IMAGE_FILE_LARGE_ADDRESS_AWARE = 0x0020, //Application can handle >2GB addresses.															
                                                     //Reserved = 0x0040,																														
            IMAGE_FILE_BYTES_RESERVED_LO = 0x0080, //Deprecated.																						
            IMAGE_FILE_32BIT_MACHINE = 0x0100, //If is 32 bit word																						
            IMAGE_FILE_DEBUG_STRIPPED = 0x0200, //If debugging information is removed																	
            IMAGE_FILE_REMOVABLE_RUN_FROM_SWAP = 0x0400, //If the image is on removable media, fully load it and copy it to swap						
            IMAGE_FILE_NET_RUN_FROM_SWAP = 0x0800, //if the image is on network media, fully load it and copy it to the swap							
            IMAGE_FILE_SYSTEM = 0x1000, //The image file is a system file, not a user program															
            IMAGE_FILE_DLL = 0x2000, //The image file is a DLL, Such files are considered EXE's for most purposes, but they cannot be run				
            IMAGE_FILE_UP_SYSTEM_ONLY = 0x4000, //The file should only run on a uniprocessor machine													
            IMAGE_FILE_BYTES_REVERSED_HI = 0x8000 //Deprecated																							
        };

        //The first few hundred bytes, used to tell you in CMD "Cannot run this program in MS-DOS mode"
        public struct MS_DOS_Stub
        {
            public UInt16 e_magic;                     // Magic number
            public UInt16 e_cblp;                      // Bytes on last page of file
            public UInt16 e_cp;                        // Pages in file
            public UInt16 e_crlc;                      // Relocations
            public UInt16 e_cparhdr;                   // Size of header in paragraphs
            public UInt16 e_minalloc;                  // Minimum extra paragraphs needed
            public UInt16 e_maxalloc;                  // Maximum extra paragraphs needed
            public UInt16 e_ss;                        // Initial (relative) SS value
            public UInt16 e_sp;                        // Initial SP value
            public UInt16 e_csum;                      // Checksum
            public UInt16 e_ip;                        // Initial IP value
            public UInt16 e_cs;                        // Initial (relative) CS value
            public UInt16 e_lfarlc;                    // File address of relocation table
            public UInt16 e_ovno;                      // Overlay number
            private unsafe fixed UInt16 e_res1[4];    // Reserved words                 
            public UInt16 e_oemid;                     // OEM identifier (for e_oeminfo)
            public UInt16 e_oeminfo;                   // OEM information; e_oemid specific
            private unsafe fixed UInt16 e_res2[10];     // Reserved words 
            public UInt32 e_lfanew;                    // File address of new exe header 
        };

        [StructLayout(LayoutKind.Explicit)] //Not nessesary
        public struct file_header
        {
            //Magic Number: PE\0\0 (0x50, 0x45, 0x00, 0x00)
            //[FieldOffset(0)] UInt32 signature;                 //17744 (4550) Magic Number

            //enum Architecture
            [FieldOffset(0)] public MachineTypes machine;                   //34404 (8664) x86 64 bit

            //This indicated the size of the section table, which immediatley follows the header.
            [FieldOffset(2)] public UInt16 numberOfSections;          //5

            //C time (Number of seconds since 00:00 Jan 1, 1970), indicated when file was created
            [FieldOffset(4)] public UInt32 timeDateStamp;             //Varies

            //Should be 0, deprecated	
            [FieldOffset(8)] public UInt32 pointerToSymbolTable;      //0

            //Should be 0, deprecated
            [FieldOffset(12)] public UInt32 numberOfSymbols;           //0

            //THe size of the optional header
            [FieldOffset(16)] public UInt16 sizeOfOptionalHeader;      //240

            //Bitfield enum characteristics
            [FieldOffset(18)] public Characteristics characteristics;           //8226
        };

        public enum WindowsSubsystem : UInt16
        {
            IMAGE_SUBSYSTEM_UNKNOWN = 0, //An uknown subsystem
            IMAGE_SUBSYSTEM_NATIVE = 1, //Device drivers and native Windows processes
            IMAGE_SUBSYSTEM_WINDOWS_GUI = 2, //The Windows GUI subsystem
            IMAGE_SUBSYSTEM_WINDOWS_CUI = 3, //The windows character subsystem
            IMAGE_SUBSYSTEM_OS2_CUI = 5, //The OS/2 character subsystem
            IMAGE_SUBSYSTEM_POSIX_CUI = 7, //The Posix character system
            IMAGE_SUBSYSTEM_NATIVE_WINDOWS = 8, //Native Win9x driver
            IMAGE_SUBSYSTEM_WINDOWS_CE_GUI = 9, //Windows CE
            IMAGE_SUBSYSTEM_EFI_APPLICATION = 10, //An Extensible Firmware Interface (EFI) application
            IMAGE_SUBSYSTEM_EFI_BOOT_SERVER_DRIVER = 11, //An EFI driver wiht boot services
            IMAGE_SUBSYSTEM_EFI_RUNTIME_DRIVER = 12, //An EFI driver with run time services
            IMAGE_SUBSYSTEM_EFI_ROM = 13, //An EFI ROM image
            IMAGE_SUBSYSTEM_XBOX = 14, //Xbox
            IMAGE_SUBSYSTEM_WINDOWS_BOOT_APPLICATION = 16, //Windows boot application
            IMAGE_SUBSYSTEM_XBOX_CODE_CATALOG = 17,
        };

        [Flags]
        public enum DLLCharacteristics : UInt16
        {
            //RESERVED = 0x0001
            //RESERVED = 0x0002
            //RESERVED = 0x0004
            //RESERVED = 0x0008
            //UNUSED = 0x0010
            IMAGE_DLLCHARACTERISTICS_HIGH_ENTROPY_VA = 0x0020,  //Image can handle a high entropy 64-bit virtual address space
            IMAGE_DLLCHARACTERISTICS_DYNAMIC_BASE = 0x0040, //DLL can be relocated at load time
            IMAGE_DLLCHARACTERISTICS_FORCE_INTEGRITY = 0x0080, //Code integrity checks are enforced
            IMAGE_DLLCHARACTERISTICS_NX_COMPACT = 0x0100, //Image is NX compatible
            IMAGE_DLLCHARACTERISTICS_NO_ISOLATION = 0x0200, //Isolation aware, but do not isolate the image
            IMAGE_DLLCHARACTERISTICS_NO_SEH = 0x0400, //Does not use structure exception (SE) handling, No SE handles may be called in this image.
            IMAGE_DLLCHARACTERISTICS_NO_BIND = 0x0800, //Do not bind the image
            IMAGE_DLLCHARACTERISTICS_APPCONTAINER = 0x1000, //Image must execute in an AppContainer
            IMAGE_DLLCHARACTERISTICS_WDM_DRIVER = 0x2000, //A WDM driver
            IMAGE_DLLCHARACTERISTICS_GUARD_CF = 0x4000, //Image supports Control Flow Card
            IMAGE_DLLCHARACTERISTICS_TERMINAL_SERVER_AWARE = 0x8000 //Terminal Server Aware
        };

        public enum Magic : UInt16
        {
            ROMImage = 0x107,
            PE32 = 0x10B,
            PE64 = 0x20B
        }

        public struct _IMAGE_DATA_DIRECTORY
        {
            public UInt32 VirtualAddress;
            public UInt32 Size;
        } //typedef *PIMAGE_DATA_DIRECTORY

        [StructLayout(LayoutKind.Explicit)] //Not nessesary.
        public struct optional_header
        {

            /*Standard Fields*/

            // 0x107 - ROM image
            // 0x10B - PE32
            // 0X20B - PE32+
            [FieldOffset(0)] public Magic magic; //PE32+

            //Linker major version number
            [FieldOffset(2)] public UInt8 majorLinkerVersion; //14

            //Linker minor version number
            [FieldOffset(3)] public UInt8 minorLinkerVersion; //16 (Latest MSVC)

            //The size of the code (text) section, or the sum of all code sections if there are multiple.
            [FieldOffset(4)] public UInt32 sizeOfCode;

            //Size of the initialized data section, or the sum of all sections if there are multiple.
            [FieldOffset(8)] public UInt32 sizeOfInitialisedData;

            //The size of the uninitialized data section (BSS), or the sum of all such sections if there are multiple BSS sections. 
            [FieldOffset(12)] public UInt32 sizeOfUninitialisedData;

            //Entry point, Optional for DLL's, Must be 0 to be optional
            [FieldOffset(16)] public UInt32 addressOfEntryPoint; //Is zero

            //The address that is relative to the image base of the beginning-of-code section when it is loaded into memory
            [FieldOffset(20)] public UInt32 baseOfCode;



            /*Windows Specific Fields*/

            //The preffered base address, bust be a multiple of 64K, The defualt for DLL's is 0x1000'0000
            [FieldOffset(24)] public UInt64 imageBase;

            //The alignment (in bytes) of sections when they are loaded into memory. It must be greater than or equal to file alignment. The default is page size
            [FieldOffset(32)] public UInt32 sectionAlignment;

            //The alignment factor (in bytes) that is used to align the raw data of sections in the image file. The value should be a power of 2 between 512 and 64k. Default is 512.
            [FieldOffset(36)] public UInt32 fileAlignment;

            //THe major version number of required OS
            [FieldOffset(40)] public UInt16 majorOperatingSystemVersion;

            //The minor version number of the required OS
            [FieldOffset(42)] public UInt16 minorOperatingSystemVersion;

            //The major version number of the image
            [FieldOffset(44)] public UInt16 majorImageVersion;

            //The minor version number of the image
            [FieldOffset(46)] public UInt16 minorImageVersion;

            //The major version number of the subsystem
            [FieldOffset(48)] public UInt16 majorSubsystemVersion;

            //The Minor version number of the subsystem.
            [FieldOffset(50)] public UInt16 minorSubsystemVersion;

            //Reserved, must be 0
            [FieldOffset(52)] public UInt32 win32VersionValue;

            //The size (In bytes) of the image including all headers, as the image is loaded into memory.
            [FieldOffset(56)] public UInt32 sizeOfImage;

            //The combined size of an MS-DOS stub, PE header and sections header rounded up to a multiple of FileAlignment
            [FieldOffset(60)] public UInt32 sizeOfHeaders;

            //The image file checksum, The following are checked for validation at load time, Any DLL that is loaded into a critical windows process.
            [FieldOffset(64)] public UInt32 checkSum;

            //enum WindowsSubsystem
            [FieldOffset(68)] public UInt16 subSystem;

            //enum DLLCharacteristics
            [FieldOffset(70)] public UInt16 dllCharacteristics;

            //The size of the stack to reserve. Only SizeOfStackCommit os commited, the rest is made avaliable one page at a time until the reserve size is reached.
            [FieldOffset(72)] public UInt64 sizeOfStackReserve;

            //THe size of the stack to commit
            [FieldOffset(80)] public UInt64 sizeOfStackCommit;

            //The size of the local heap space to reserve, Only SizeOFheapCommit is commited, the rest is made avaliable on page at a time until the reserve size is reached,
            [FieldOffset(88)] public UInt64 sizeOfHeapReserve;

            //THe size of the local heap to commit
            [FieldOffset(96)] public UInt64 sizeOfHeapCommit;

            //Reserved, must be 0
            [FieldOffset(104)] public UInt32 loaderFlags;

            //The number of data directory entries in the remainder of the optional header. Each describes a location and size.
            [FieldOffset(108)] public UInt32 numberOfRvaAndSizes;



            /*Data Directories*/

            //The export table	.edata
            [FieldOffset(112)] public _IMAGE_DATA_DIRECTORY exportTable;

            //The import table	.idata
            [FieldOffset(120)] public _IMAGE_DATA_DIRECTORY importTable;

            //The resource table	.rsrc
            [FieldOffset(128)] public _IMAGE_DATA_DIRECTORY resourceTable;

            //The exception table	.pdata
            [FieldOffset(136)] public _IMAGE_DATA_DIRECTORY exceptionTable;

            //The certificate table	(Image Only)
            [FieldOffset(144)] public _IMAGE_DATA_DIRECTORY certificateTable;

            //The base relocation table	.reloc
            [FieldOffset(152)] public _IMAGE_DATA_DIRECTORY baseRelocationTable;

            //The debug table	.debug
            [FieldOffset(160)] public _IMAGE_DATA_DIRECTORY debug;

            //Reserved
            [FieldOffset(168)] public _IMAGE_DATA_DIRECTORY architecture;

            //The RVA of the value to be stored in the global pointer register. The size must be set to 0.
            [FieldOffset(176)] public _IMAGE_DATA_DIRECTORY globalPTR;

            //The thread local storage talble	.tls
            [FieldOffset(184)] public _IMAGE_DATA_DIRECTORY tlsTable;

            //The load config table	(Image Only)
            [FieldOffset(192)] public _IMAGE_DATA_DIRECTORY loadConfigTable;

            //The bound import table
            [FieldOffset(200)] public _IMAGE_DATA_DIRECTORY boundImport;

            //The import address table
            [FieldOffset(208)] public _IMAGE_DATA_DIRECTORY iat;

            //The Delay Import table	(Image only)
            [FieldOffset(216)] public _IMAGE_DATA_DIRECTORY delayImportDescriptor;

            //The CLR runtime header	.cormeta (Object Only)
            [FieldOffset(224)] public _IMAGE_DATA_DIRECTORY clrRuntimeHeader;

            //Must be 0
            [FieldOffset(232)] public _IMAGE_DATA_DIRECTORY reserved;
        };

        public struct NTHeaders
        {
            //Magic Number: PE\0\0 (0x50, 0x45, 0x00, 0x00)
            public UInt32 MagicNumber;

            public file_header FileHeader;

            public optional_header optnHeader;
        }

        public enum ImageDirectories
        {
            Export = 0,
            Import = 1,
            Resource = 2,
            Exception = 3,
            Security = 4,
            BaseReloc = 5,
            Debug = 6,
            //Copyright =   7,
            Architecture = 7,
            GlobalPtr = 8,
            Tls = 9,
            LoadConfig = 10,
            BoundImport = 11,
            IAT = 12,
            DelayImport = 13,
            ComDescriptor = 14,
        }

        [Flags]
        public enum SectionCharacteristics : UInt32
        { ///BitField (Weird custom section at IMG_SCN_ALIGN_BYTES) (DOCUMENTATION SO VISUAL STUDIO DOESN'T COLLAPSE RESERVED)
            //Reserved = 0x0000'0000
            //Reserved = 0x0000'0001
            //Reserved = 0x0000'0002
            //Reserved = 0x0000'0004
            IMAGE_SCN_TYPE_NO_PAD = 0x00000008,//The section should not be padded to the next boundry, Obselute and should be replaced with the IMAGE_SCN_ALIGN_1BYTES
                                               //Reserved = 0x0000'0010
            IMAGE_SCN_CNT_CODE = 0x00000020,    //This section contains executable code

            IMAGE_SCN_CNT_INITIALISED_DATA = 0x00000040,    //This section contains initialised data

            IMAGE_SCN_CNT_UNINITIALISED_DATA = 0x00000080,  //This section contains uninitialised data

            IMAGE_SCN_LNK_OTHER = 0x00000100,   //Reserved for future use

            IMAGE_SCN_LNK_INFO = 0x00000200,    //This sections contains comment or other information. 
                                                //Reserved = 0x0000'0400
            IMAGE_SCN_LNK_REMOVE = 0x00000800,  //The section will not become part of the image

            IMAGE_SCN_LNK_COMDAT = 0x00001000, //The section contains .COMDAT data.
                                               //Unused = 0x0000'2000, 
                                               //Unused = 0x0000'4000
            IMAGE_SCN_GPREL = 0x00008000,       //The section contains data references through the global pointer (GP)
                                                //Unused = 0x0001'0000
            IMAGE_SCN_MEM_PURGABLE = 0x00020000,    //Reserved for future use

            IMAGE_SCN_MEM_16BIT = 0x00020000,       //Reserved for future use

            IMAGE_SCN_MEM_LOCKED = 0x00040000,      //Reserved for future use

            IMAGE_SCN_MEM_PRELOAD = 0x00080000, //Reserved for future use

            IMAGE_SCN_ALIGN_1BYTES = 0x00100000,        //Align data on a 1-byte boundry. Valid only for object files.

            IMAGE_SCN_ALIGN_2BYTES = 0x00200000,        //Align data on a 2-byte boundry. Valid only for object files.

            IMAGE_SCN_ALIGN_4BYTES = 0x00300000,        //Align data on a 4-byte boundry. Valid only for object files.

            IMAGE_SCN_ALIGN_8BYTES = 0x00400000,        //Align data on a 8-byte boundry. Valid only for object files.

            IMAGE_SCN_ALIGN_16BYTES = 0x00500000,       //Align data on a 16-byte boundry. Valid only for object files.

            IMAGE_SCN_ALIGN_32YTES = 0x00600000,        //Align data on a 32-byte boundry. Valid only for object files.

            IMAGE_SCN_ALIGN_64YTES = 0x00700000,        //Align data on a 64-byte boundry. Valid only for object files.

            IMAGE_SCN_ALIGN_128YTES = 0x00800000,       //Align data on a 128-byte boundry. Valid only for object files.

            IMAGE_SCN_ALIGN_256BYTES = 0x00900000,      //Align data on a 256-byte boundry. Valid only for object files.

            IMAGE_SCN_ALIGN_512BYTES = 0x00A00000,      //Align data on a 512-byte boundry. Valid only for object files.

            IMAGE_SCN_ALIGN_1024BYTES = 0x00B00000, //Align data on a 1024-byte boundry. Valid only for object files.

            IMAGE_SCN_ALIGN_2048BYTES = 0x00C00000, //Align data on a 2048-byte boundry. Valid only for object files.

            IMAGE_SCN_ALIGN_4096BYTES = 0x00D00000, //Align data on a 4096-byte boundry. Valid only for object files.

            IMAGE_SCN_ALIGN_8192BYTES = 0x00E00000, //Align data on a 8192-byte boundry. Valid only for object files.

            IMAGE_SCN_LNK_NRELOC_OVFL = 0x01000000,//This section contains extended relocations

            IMAGE_SCN_MEM_DISCARDABLE = 0x02000000,//This section can be discareded as needed

            IMAGE_SCN_MEM_NOT_CACHED = 0x04000000, //This section cannot be cached

            IMAGE_SCN_MEM_NOT_PAGED = 0x08000000,   //This section is not pageable

            IMAGE_SCN_MEM_SHARED = 0x10000000,      //This section can be shared in memory

            IMAGE_SCN_MEM_EXECUTE = 0x20000000, //This section can be executed as code

            IMAGE_SCN_MEM_READ = 0x40000000,        //This section can be read

            IMAGE_SCN_MEM_WRITE = 0x80000000,       //this section can be written to
        };

        [StructLayout(LayoutKind.Explicit)]
        public struct section_table
        {
            //An 8 byte null padded UTF-8 encoded string, exactly 8 chars long, no null terminator.
            [FieldOffset(0)] unsafe fixed byte name[8]; //char[8]



            //Size of the section when loaded into memory, should be set to 0 for object files.
            [FieldOffset(8)] public UInt32 virtualSize;
            [FieldOffset(8)] public UInt32 PhysicalAddress; //Union


            //Adress of the firt byte before relocation is applied, for simplicity, compilers should set this to 0.
            [FieldOffset(12)] public UInt32 virtualAddress;

            //The size of the section. 
            [FieldOffset(16)] public UInt32 sizeOfRawData;

            //The file pointer to the first page of the section. For object files, the value should be aligned on a 4-byte boundry for best performance.
            //When a section contains only uninitialized data, this field should be 0.
            [FieldOffset(20)] public UInt32 pointerToRawData;

            //The file pointer to the beginning of the reloc entries for the section. This is set to EXE images if there are no relocs
            [FieldOffset(24)] public UInt32 pointerToRelocations;

            //Deprecated
            [FieldOffset(28)] public UInt32 pointerToLinenumbers;

            //The number of relocation entries for the section. Set to 0 for executable images.
            [FieldOffset(30)] public UInt16 numberOfRelocations;

            //Deprecated
            [FieldOffset(32)] public UInt16 numberOfLineNumbers;

            //enum SectionFlags
            [FieldOffset(36)] public SectionCharacteristics characteristics;

        }; //Typedef *PIMAGE_SECTION_HEADER, IMAGE_SECTION_HEADER

        public enum reloaction_type_x86 : UInt16
        {
            IMAGE_REL_AMD64_ABSOLUTE = 0x0000,  //The relocation is ignored
            IMAGE_REL_AMD64_ADDR64 = 0x0001,    //The 64-bit VA of the targed
            IMAGE_REL_AMD64_ADDR32 = 0x0002,    //The 32-bit VA of the target
            IMAGE_REL_AMD64_ADDR32NB = 0x0003,  //The 32-bit address without an image base (RVA)
            IMAGE_REL_AMD64_REL32 = 0x00004,    //The 32 bit relitive address from the byte following the relocation
            IMAGE_REL_AMD64_REL32_1 = 0x0005,   //The 32 bit address relitive to byte distance 1 from the relocation
            IMAGE_REL_AMD64_REL32_2 = 0x0006,   //The 32 bit address relitive to byte distance 2 from the relocation
            IMAGE_REL_AMD64_REL32_3 = 0x0007,   //The 32 bit address relitive to byte distance 3 from the relocation
            IMAGE_REL_AMD64_REL32_4 = 0x0008,   //The 32 bit address relitive to byte distance 4 from the relocation
            IMAGE_REL_AMD64_REL32_5 = 0x0009,   //The 32 bit address relitive to byte distance 5 from the relocation
            IMAGE_REL_AMD64_SECTION = 0x000A,   //The 16-bit section index of the section that contains the targed. This is used to support debugging information.
            IMAGE_REL_AMD64_SECREL = 0x000B,    //The 32-bit offset of the target from the beginning of the section, this is used to support debugging information.
            IMAGE_REL_AMD64_SECREL7 = 0x000C,   //A 7-bit unsigned offset from the base of the section that contains the target
            IMAGE_REL_AMD64_TOKEN = 0x000D,     //CLR tokens
            IMAGE_REL_AMD64_SREL32 = 0x000E,    //A 32-bit signed span-dependent value emmited into the object
            IMAGE_REL_AMD64_PAIR = 0x000F,      //A pair that must immeditley follow every span-dependent value
            IMAGE_REL_AMD64_SSPAN32 = 0x0010    //A 32-bit signed span-depentend value that is applied at link time
        };

        // Cannot be fucked doing ~25-30 enums for each architecture supported when windows only fucking runs on X86.
        // I'm not even sure why they fucking exist if windows only runs on I386 (32 bit x86) and x86. It's fucking retarded

        [StructLayout(LayoutKind.Explicit)]
        public struct relocations
        {
            //The address of the item to which the relocaiton is applied, this is the offset from the beginning of the section, plus the value of the sections RVA/Offset field.
            [FieldOffset(0)] UInt32 virtualAddress;
            [FieldOffset(0)] UInt32 relocCount; //Union

            //A zero based index into the symbol table. This symbol gives the address that is to be used for the relocation.
            [FieldOffset(4)] UInt32 symbolTableIndex;

            //Dependent on processor type. Windows only support x86, only one used here is 64 bit.
            [FieldOffset(8)] reloaction_type_x86 type;
        };

        #endregion PeFile

        public struct ExportInfo
        {
            public string name;
            public UInt32 RVA;
        }

        public struct IMAGE_EXPORT_DIRECTORY
        {
            public UInt32 characteristics;
            public UInt32 TimeDateStamp;
            public UInt16 MajorVersion;
            public UInt16 MinorVersion;
            public UInt32 Name;
            public UInt32 Base;
            public UInt32 NumberOfFunctions;
            public UInt32 NumberOfNames;
            public UInt32 AddressOfFunctions;
            public UInt32 AddressOfNames;
            public UInt32 AddressOfNameOrdinals;

        }

        #endregion Structs


        #region Methods

        // The offset is where the data is located in the file
        // The RVA (Relative Virtual Address) is where the data is located relative to the start of the file when loaded into memory.
        // The VA (Virtual Address) is the address of something in its own sandbox.
        // 
        // These can be calculated by:
        // VA = Offset + SectionHeader.VirtualAddress
        // RVA = Offset + (SectionHeader.VirtualAddress - SectionTable.PointerToRawData)
        // 

        public static unsafe UInt64 RVAtoOffset(UInt64 rva, NTHeaders* ntHeaders, byte* pBin)
        {

            MS_DOS_Stub* stub = (MS_DOS_Stub*)(pBin);

            for (int i = 0; i < ntHeaders->FileHeader.numberOfSections; i++)
            {
                section_table* secTable = (section_table*)(pBin + stub->e_lfanew + sizeof(NTHeaders) + sizeof(section_table) * i);

                if (secTable->virtualAddress <= rva && rva < secTable->virtualAddress + secTable->virtualSize)
                {
                    return (UInt64)(rva) + secTable->pointerToRawData - secTable->virtualAddress;
                }
            }

            throw new Exception("Erorr: Could not map RVA to Offset.");
        }

        public static unsafe NTHeaders* GetNtHeaders(byte* pBin)
        {
            // Bin is binary btw. Imo clearer than using peFile.

            MS_DOS_Stub* stub = (MS_DOS_Stub*)(pBin);

            if (stub->e_magic != IMAGE_DOS_SIGNATURE)
            {
                throw new FormatException("Error, Invalid file. DOS Signature is incorrect.");
            }

            NTHeaders* ntHeaders = (NTHeaders*)(pBin + stub->e_lfanew);

            if (ntHeaders->MagicNumber != IMAGE_NT_PEHEADER_SIGNATURE)
            {
                throw new FormatException("Error, Invalid file. PE File signature incorrect.");
            }

            if (ntHeaders->optnHeader.magic == Magic.PE32)
            {
                throw new FormatException("Error, Invalid file. 32 Bit DLL's are not supported.");
            }
            else if (ntHeaders->optnHeader.magic != Magic.PE64)
            {
                throw new FormatException("Error, Invalid file. Optional header signature is incorrect.");
            }

            return ntHeaders;
        }

        public static unsafe Dictionary<string, UInt32> dumpSymbolsFromMemory(byte* pBin)
        {
            Dictionary<string, UInt32> ExportTable = new Dictionary<string, UInt32>();

            NTHeaders* ntHeaders = GetNtHeaders(pBin);

            IMAGE_EXPORT_DIRECTORY* exportDir = (IMAGE_EXPORT_DIRECTORY*)(pBin +
                ntHeaders->optnHeader.exportTable.VirtualAddress);

            if (ntHeaders->optnHeader.numberOfRvaAndSizes <= 0)
            {
                throw new ArgumentException("Error, This file has no exports.");
            }

            for (UInt32 i = 0; i < exportDir->NumberOfNames; i++)
            {
                UInt32 rva = *(UInt32*)(pBin + exportDir->AddressOfFunctions + (i * sizeof(UInt32)));


                UInt32 namePointer = (UInt32)exportDir->AddressOfNames + (i * sizeof(UInt32));

                string methodName = Marshal.PtrToStringAnsi((IntPtr)
                (pBin + (*(UInt32*)(pBin + namePointer))));


                ExportTable.Add(methodName, rva);
            }
            return ExportTable;
        }

        public static unsafe Dictionary<string, UInt32> dumpSymbolsFromFile(byte* pBin)
        {
            Dictionary<string, UInt32> ExportTable = new Dictionary<string, UInt32>();

            NTHeaders* ntHeaders = GetNtHeaders(pBin);

            IMAGE_EXPORT_DIRECTORY* exportDir = (IMAGE_EXPORT_DIRECTORY*)(pBin +
            RVAtoOffset(ntHeaders->optnHeader.exportTable.VirtualAddress, ntHeaders, pBin));

            if (ntHeaders->optnHeader.numberOfRvaAndSizes <= 0)
            {
                throw new ArgumentException("Error, This file has no exports.");
            }

            for (UInt32 i = 0; i < exportDir->NumberOfNames; i++)
            {
                UInt32 offset = (UInt32)RVAtoOffset((*(UInt32*)(pBin + RVAtoOffset(exportDir->AddressOfFunctions, ntHeaders, pBin) + (i * sizeof(UInt32)))), ntHeaders, pBin);


                UInt32 nameOffset = (UInt32)RVAtoOffset((UInt32)exportDir->AddressOfNames, ntHeaders, pBin) + (i * sizeof(UInt32));

                string methodName = Marshal.PtrToStringAnsi((IntPtr)
                (pBin + RVAtoOffset((*(UInt32*)(pBin + nameOffset)), ntHeaders, pBin)));


                ExportTable.Add(methodName, offset);
            }

            return ExportTable;
        }

        #endregion Methods

    }

}

