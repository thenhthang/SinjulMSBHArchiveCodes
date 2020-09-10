namespace WebcamBlazorWasm.Pages
{
    public class WebCamOptions
    {
        public string VideoId { get; set; }
        public string CanvasId { get; set; }
        public string PhotoId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool VideoEnable { get; set; }
        public bool AudioEnable { get; set; }
        public string Filter { get; set; }
    }
}
