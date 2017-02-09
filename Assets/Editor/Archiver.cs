using System.IO;

using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

public static class Archiver {
	const string TAR_EXTENSION = ".tar.gz";
	const string ZIP_EXTENSION = ".zip";

	public static void CreateTar (string outputTarFilename, string sourceDirectory) {
		Directory.SetCurrentDirectory (BuildSettings.DIRECTORY);
		outputTarFilename += TAR_EXTENSION;
		using (FileStream fs = new FileStream (outputTarFilename, FileMode.Create, FileAccess.Write, FileShare.None))
		using (Stream gzipStream = new GZipOutputStream (fs))
		using (TarArchive tarArchive = TarArchive.CreateOutputTarArchive (gzipStream)) {
			AddDirectoryFilesToTar (tarArchive, sourceDirectory, true);
		}
		Directory.SetCurrentDirectory ("..");
	}

	private static void AddDirectoryFilesToTar (TarArchive tarArchive, string sourceDirectory, bool recurse) {
		if (recurse) {
			string[] directories = Directory.GetDirectories (sourceDirectory);
			foreach (string directory in directories)
				AddDirectoryFilesToTar (tarArchive, directory, recurse);
		}
		string[] filenames = Directory.GetFiles (sourceDirectory);
		foreach (string filename in filenames) {
			TarEntry tarEntry = TarEntry.CreateEntryFromFile (filename);
			tarArchive.WriteEntry (tarEntry, true);
		}
	}

	public static void CreateZip (string outputZipFilename, string sourceDirectory) {
		Directory.SetCurrentDirectory (BuildSettings.DIRECTORY);
		outputZipFilename += ZIP_EXTENSION;

		FileStream fsOut = File.Create (outputZipFilename);
		ZipOutputStream zipStream = new ZipOutputStream (fsOut);

		zipStream.SetLevel (3); //0-9, 9 being the highest level of compression

		CompressFolder (sourceDirectory, zipStream, 0);

		zipStream.IsStreamOwner = true;
		zipStream.Close ();

		Directory.SetCurrentDirectory ("..");
	}

	private static void CompressFolder (string path, ZipOutputStream zipStream, int folderOffset) {

		string[] files = Directory.GetFiles (path);

		foreach (string filename in files) {

			FileInfo fi = new FileInfo (filename);

			string entryName = filename.Substring (folderOffset);
			entryName = ZipEntry.CleanName (entryName);
			ZipEntry newEntry = new ZipEntry (entryName);
			newEntry.DateTime = fi.LastWriteTime;
			newEntry.Size = fi.Length;

			zipStream.PutNextEntry (newEntry);

			byte[] buffer = new byte[4096];
			using (FileStream streamReader = File.OpenRead (filename)) {
				StreamUtils.Copy (streamReader, zipStream, buffer);
			}
			zipStream.CloseEntry ();
		}
		string[] folders = Directory.GetDirectories (path);
		foreach (string folder in folders) {
			CompressFolder (folder, zipStream, folderOffset);
		}
	}
}