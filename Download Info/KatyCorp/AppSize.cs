using System.IO;

namespace KatyCorp.Tools
{
    public static class AppSize
    {
        public static long DirRealSize(DirectoryInfo d)
        {
            long size = 0;

            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            string text;
            //FileStream file;
            StreamReader streamReader;
            //HashSet<Char> removeChars = new HashSet<Char> { '\0' };
            foreach (FileInfo fi in fis)
            {
                try
                {
                    streamReader = fi.OpenText();
                    //file = fi.OpenRead();
                    //size += file.ReadByts() == '\0' ? 0 : 1;

                    text = streamReader.ReadToEnd();
                    //size += text.Contains('\n') ? 1 : 0;
                    size += text.Replace("\0", string.Empty).Length;

                    /*while ((text = streamReader.ReadLine()) != null)
                    {
                        size += text.Replace("\0", string.Empty).Length;

                        StringBuilder sb = new StringBuilder(text.Length);
                        foreach (Char c in text)
                        {
                            if (!removeChars.Contains(c))
                                sb.Append(c);
                        }
                        size += sb.Length;
                    }*/

                    //file.Close();
                    streamReader.Close();
                }
                catch (IOException)
                {
                    continue;
                }
            }

            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
                size += DirRealSize(di);

            return size;
        }

        public static long DirSize(DirectoryInfo d)
        {
            long size = 0;

            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
                size += fi.Length;

            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
                size += DirSize(di);

            return size;
        }
    }
}
