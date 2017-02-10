using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Zip;

namespace Editor {
    public static class Archiver {
        private const string TarExtension = ".tar.gz";
        private const string ZipExtension = ".zip";

        public static void CreateTar(string outputTarFilename, string sourceDirectory) {
            Directory.SetCurrentDirectory(BuildSettings.Directory);
            outputTarFilename += TarExtension;
            using (var fs = new FileStream(outputTarFilename, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var gzipStream = new GZipOutputStream(fs))
            using (var tarArchive = TarArchive.CreateOutputTarArchive(gzipStream)) {
                AddDirectoryFilesToTar(tarArchive, sourceDirectory);
            }
            Directory.SetCurrentDirectory("..");
        }

        private static void AddDirectoryFilesToTar(TarArchive tarArchive, string sourceDirectory) {
            var directories = Directory.GetDirectories(sourceDirectory);
            foreach (var directory in directories)
                AddDirectoryFilesToTar(tarArchive, directory);

            var filenames = Directory.GetFiles(sourceDirectory);
            foreach (var filename in filenames) {
                var tarEntry = TarEntry.CreateEntryFromFile(filename);
                tarArchive.WriteEntry(tarEntry, true);
            }
        }

        public static void CreateZip(string outputZipFilename, string sourceDirectory) {
            Directory.SetCurrentDirectory(BuildSettings.Directory);
            outputZipFilename += ZipExtension;

            var fsOut = File.Create(outputZipFilename);
            var zipStream = new ZipOutputStream(fsOut);

            zipStream.SetLevel(3); //0-9, 9 being the highest level of compression

            CompressFolder(sourceDirectory, zipStream, 0);

            zipStream.IsStreamOwner = true;
            zipStream.Close();

            Directory.SetCurrentDirectory("..");
        }

        private static void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset) {
            var files = Directory.GetFiles(path);

            foreach (var filename in files) {
                var fi = new FileInfo(filename);

                var entryName = filename.Substring(folderOffset);
                entryName = ZipEntry.CleanName(entryName);
                var newEntry = new ZipEntry(entryName) {
                    DateTime = fi.LastWriteTime,
                    Size = fi.Length
                };

                zipStream.PutNextEntry(newEntry);

                var buffer = new byte[4096];
                using (var streamReader = File.OpenRead(filename)) {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }
            var folders = Directory.GetDirectories(path);
            foreach (var folder in folders) {
                CompressFolder(folder, zipStream, folderOffset);
            }
        }
    }
}
