using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_WPF
{
    public class Original
    {
        public string filename { get; set; }
        public int length { get; set; }
        public string content_type { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string avg { get; set; }
    }

    public class Xlarge
    {
        public string filename { get; set; }
        public int length { get; set; }
        public string content_type { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Large
    {
        public string filename { get; set; }
        public int length { get; set; }
        public string content_type { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Medium
    {
        public string filename { get; set; }
        public int length { get; set; }
        public string content_type { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Small
    {
        public string filename { get; set; }
        public int length { get; set; }
        public string content_type { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Fs
    {
        public string filename { get; set; }
        public int length { get; set; }
        public string content_type { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Frame
    {
        public int x1 { get; set; }
        public int y1 { get; set; }
        public int x2 { get; set; }
        public int y2 { get; set; }
    }

    public class Face
    {
        public Frame frame { get; set; }
        public List<object> faces { get; set; }
    }

    public class Info
    {
        public Original original { get; set; }
        public Xlarge xlarge { get; set; }
        public Large large { get; set; }
        public Medium medium { get; set; }
        public Small small { get; set; }
        public Fs fs { get; set; }
        public Face face { get; set; }
    }

    public class UploadedImageProp
    {
        public string access_key { get; set; }
        public Info info { get; set; }
    }
}
