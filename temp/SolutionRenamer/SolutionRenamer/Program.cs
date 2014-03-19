using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SolutionRenamer
{
    public class Program
    {
        private const string PlaceHolder = "MySpaProject";

        private static string _rootPath = "";
        
        private static string _projectName = "";

        static void Main(string[] args)
        {
            if (args != null && args.Length > 1)
            {
                _rootPath = args[0];
                _projectName = args[1];
            }
            else
            {
                Console.WriteLine("Root path and Project name must be supplied!");
                return;
            }

            Doit();
        }

        private static void Doit()
        {
            RenameDirRecursively(_rootPath);
            RenameAllFiles(_rootPath);
            ReplaceContent(_rootPath);
        }

        private static void RenameDirRecursively(string rootPath)
        {
            var dirs = Directory.GetDirectories(rootPath, "*.*", SearchOption.TopDirectoryOnly);
            foreach (var dir in dirs)
            {
                var newDir = dir;
                if (dir.Contains(PlaceHolder))
                {
                    newDir = dir.Replace(PlaceHolder, _projectName);
                    Directory.Move(dir, newDir);
                }

                RenameDirRecursively(newDir);
            }
        }

        private static void RenameAllFiles(string rootPath)
        {
            var files = Directory.GetFiles(rootPath, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (file.Contains(PlaceHolder))
                {
                    File.Move(file, file.Replace(PlaceHolder, _projectName));
                }
            }
        }

        private static void ReplaceContent(string rootPath)
        {
            var skipExtensions = new[]
                                 {
                                     ".exe", ".dll", ".bin", ".suo", ".png", ".pdb", ".obj"
                                 };

            var files = Directory.GetFiles(rootPath, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (skipExtensions.Contains(Path.GetExtension(file)))
                {
                    continue;
                }

                var fileSize = GetFileSize(file);
                if (fileSize < PlaceHolder.Length)
                {
                    continue;
                }

                var encoding = GetEncoding(file);

                var content = File.ReadAllText(file, encoding);
                var newContent = content.Replace(PlaceHolder, _projectName);
                if (newContent != content)
                {
                    File.WriteAllText(file, newContent, encoding);
                }
            }
        }

        private static long GetFileSize(string file)
        {
            return new FileInfo(file).Length;
        }

        private static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open)) file.Read(bom, 0, 4);

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.ASCII;
        }
    }
}
