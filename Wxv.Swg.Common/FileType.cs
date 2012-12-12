using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Wxv.Swg.Common.Exporters;
using Wxv.Swg.Common.Files;

namespace Wxv.Swg.Common
{
    [Flags]
    public enum FileView
    {
        None = 0,
        Binary = 1,
        Text = 2,
        String = 4,
        IFF = 8,
        Texture = 16,
        Media = 32
    }

    public delegate void FileExporterDelegate(IRepository repository, byte[] data, string targetFileName);

    public sealed class FileTypeExporter
    {
        public string Extension { get; internal set; }
        public string Name { get; internal set; }
        public FileExporterDelegate Converter { get; internal set; }

        public static IEnumerable<FileTypeExporter> EmptyList = new FileTypeExporter[] { };

        internal FileTypeExporter() { }
    }

    public class FileViewExporter
    {
        public FileView FileView { get; private set; }
        public IEnumerable<FileTypeExporter> Exporters { get; private set; }

        private FileViewExporter() 
        { 
            Exporters = FileTypeExporter.EmptyList; 
        }

        public static readonly IEnumerable<FileViewExporter> All = new List<FileViewExporter>
        {
            new FileViewExporter { 
                FileView = FileView.IFF, 
                Exporters = new [] 
                {
                    new FileTypeExporter
                    {
                        Extension = "txt",
                        Name = "Text",
                        Converter = (repository, data, targetFileName) => 
                        {
                            var iffFile = new IFFFileReader().Load(data);
                            using (var writer = File.CreateText(targetFileName))
                                iffFile.ToString(writer);
                        }
                    }
                }},
            new FileViewExporter { 
                FileView = FileView.String, 
                Exporters = new [] 
                {
                    new FileTypeExporter
                    {
                        Extension = "csv",
                        Name = "Comma Seperated Value",
                        Converter = (repository, data, targetFileName) => 
                        {
                            var stringExporter = new StringExporter(data);
                            stringExporter.Export(targetFileName);
                        }
                    }
                }
            }
        };

        public static IEnumerable<FileTypeExporter> FromFileView(FileView fileView)
        {
            var result = All.FirstOrDefault(fve => fve.FileView == fileView);
            return result != null ? result.Exporters : FileTypeExporter.EmptyList;
        }
    }

    public delegate void DebugToStringDelgate(byte[] data, TextWriter writer);

    public sealed class FileType
    {
        public string Extension { get; private set; }
        public string IFFRoot { get; private set; }
        public string Name { get; private set; }
        public FileView FileView { get; private set; }

        private IEnumerable<FileTypeExporter> FileTypeExporters { get; set; }
        public IEnumerable<FileTypeExporter> Exporters
        {
            get { return FileTypeExporters.Union(FileViewExporter.FromFileView(FileView)); }
        }

        public DebugToStringDelgate DebugToString { get; private set; }

        private FileType() 
        { 
            FileTypeExporters = FileTypeExporter.EmptyList; 
        }

        public static readonly IEnumerable<FileType> All = new List<FileType>
        {
            new FileType { Extension = "ans", IFFRoot = "CKAT", FileView = FileView.IFF,     Name="Animation" },
            new FileType { Extension = "apt", IFFRoot = "APT",  FileView = FileView.IFF,     Name="Appearance" },
            new FileType { Extension = "ash", IFFRoot = "ASHT", FileView = FileView.IFF,     Name="ASH File" },
            new FileType { Extension = "cdf", IFFRoot = "CLDF", FileView = FileView.IFF,     Name="Client Data" },
            new FileType { Extension = "cef", IFFRoot = "CLEF", FileView = FileView.IFF,     Name="Client Effect" },
            new FileType { Extension = "cmp", IFFRoot = "CMPA", FileView = FileView.IFF,     Name="CMP File" },
            new FileType { Extension = "dds", IFFRoot = null,   FileView = FileView.Texture, Name="DirectDraw Surface",
                FileTypeExporters = new [] {
                    new FileTypeExporter
                    {
                        Extension = "png",
                        Name = "PNG",
                        Converter = (repository, data, targetFileName) => new PngExporter(data).Export(targetFileName)
                    }
                }},
            new FileType { Extension = "eft", IFFRoot = "EFCT", FileView = FileView.IFF,     Name="EFT File" },
            new FileType { Extension = "flr", IFFRoot = "FLOR", FileView = FileView.IFF,     Name="FLR File" },
            new FileType { Extension = "iff", IFFRoot = null,   FileView = FileView.IFF,     Name="Interchange Format" },
            new FileType { Extension = "ilf", IFFRoot = "INLY", FileView = FileView.IFF,     Name="Interior Layout" },
            new FileType { Extension = "inc", IFFRoot = null,   FileView = FileView.Text,    Name="Includes" },
            new FileType { Extension = "lat", IFFRoot = "LATT", FileView = FileView.IFF,     Name="LAT File" },
            new FileType { Extension = "lay", IFFRoot = "SGRP", FileView = FileView.IFF,     Name="LAY File" },
            new FileType { Extension = "lmg", IFFRoot = "MLOD", FileView = FileView.IFF,     Name="Mesh Level of Detail" },
            new FileType { Extension = "lod", IFFRoot = "DTLA", FileView = FileView.IFF,     Name="Level of Detail" },
            new FileType { Extension = "lsb", IFFRoot = "LSAT", FileView = FileView.IFF,     Name="LSB File" },
            new FileType { Extension = "ltn", IFFRoot = "LEFX", FileView = FileView.IFF,     Name="LTN File" },
            new FileType { Extension = "mgn", IFFRoot = "SKMG", FileView = FileView.IFF,     Name="Dynamic Mesh",
                DebugToString = (data, writer) => new DynamicMeshFileReader().Load(data).ToString(writer)
            },
            new FileType { Extension = "mkr", IFFRoot = "MKAT", FileView = FileView.IFF,     Name="MKR File" },
            new FileType { Extension = "mp3", IFFRoot = null,   FileView = FileView.Media,   Name="Music" },
            new FileType { Extension = "msh", IFFRoot = "MESH", FileView = FileView.IFF,     Name="Static Mesh",
                FileTypeExporters = new [] {
                    new FileTypeExporter
                    {
                        Extension = "dae",
                        Name = "Collada",
                        Converter = (repository, data, targetFileName) => new ColladaMeshExporter(repository, data).Export(targetFileName)
                    }
                },
                DebugToString = (data, writer) => new MeshFileReader().Load(data).ToString(writer)
            },
            new FileType { Extension = "pal", IFFRoot = null,   FileView = FileView.Binary,  Name="Palette" },
            new FileType { Extension = "pob", IFFRoot = "PRTO", FileView = FileView.IFF,     Name="POB File" },
            new FileType { Extension = "prt", IFFRoot = "PEFT", FileView = FileView.IFF,     Name="Particle Effect" },
            new FileType { Extension = "psh", IFFRoot = "PSHP", FileView = FileView.IFF,     Name="PSH File" },
            new FileType { Extension = "pst", IFFRoot = "PBSC", FileView = FileView.IFF,     Name="PST File" },
            new FileType { Extension = "sat", IFFRoot = "SMAT", FileView = FileView.IFF,     Name="SAT File" },
            new FileType { Extension = "sfk", IFFRoot = null,   FileView = FileView.Binary,  Name="SFK File" },
            new FileType { Extension = "sfp", IFFRoot = "FOOT", FileView = FileView.IFF,     Name="SFP File" },
            new FileType { Extension = "sht", IFFRoot = "CSHD", FileView = FileView.IFF,     Name="Shader" },
            new FileType { Extension = "skt", IFFRoot = "SLOD", FileView = FileView.IFF,     Name="Skeleton",
                FileTypeExporters = new [] {
                    new FileTypeExporter
                    {
                        Extension = "dae",
                        Name = "Collada",
                        Converter = (repository, data, targetFileName) => new ColladaSkeletonExporter(repository, data).Export(targetFileName)
                    }
                },
                DebugToString = (data, writer) => new SkeletonFileReader().Load(data).ToString(writer)
            },
            new FileType { Extension = "snd", IFFRoot = "SD3D", FileView = FileView.IFF,     Name="Sound 3D" },
            new FileType { Extension = "spr", IFFRoot = "QUAD", FileView = FileView.IFF,     Name="SPR File" },
            new FileType { Extension = "ssa", IFFRoot = "APPR", FileView = FileView.IFF,     Name="SSA File" },
            new FileType { Extension = "stf", IFFRoot = null,   FileView = FileView.String,  Name="Strings" },
            new FileType { Extension = "swh", IFFRoot = "SWSH", FileView = FileView.IFF,     Name="SWH File" },
            new FileType { Extension = "tga", IFFRoot = null,   FileView = FileView.Binary,  Name="TGA Image" },
            new FileType { Extension = "trn", IFFRoot = "PTAT", FileView = FileView.IFF,     Name="Terrain" },
            new FileType { Extension = "trt", IFFRoot = "BTRT", FileView = FileView.IFF,     Name="TRT File" },
            new FileType { Extension = "txt", IFFRoot = null,   FileView = FileView.Text,    Name="Text" },
            new FileType { Extension = "ui",  IFFRoot = null,   FileView = FileView.Text,    Name="User Interface" },
            new FileType { Extension = "vsh", IFFRoot = null,   FileView = FileView.Text,    Name="Vertex Program" },
            new FileType { Extension = "wav", IFFRoot = null,   FileView = FileView.Media,   Name="Sound" },
            new FileType { Extension = "ws",  IFFRoot = "WSNP", FileView = FileView.IFF,     Name="World Snapshot" },
        };

        public static readonly FileType Undefined = new FileType
        {
            Extension = "",
            IFFRoot = "",
            Name = "Undefined"
        };

        public static FileType FromFileName(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("path");

            var extension = Path.GetExtension(path);
            var result = All.FirstOrDefault(ft => string.Equals("." + ft.Extension, extension, StringComparison.InvariantCultureIgnoreCase));
            return result ?? FileType.Undefined;
        }
    }
}
