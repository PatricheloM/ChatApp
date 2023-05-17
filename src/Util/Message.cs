namespace ChatApp.Util
{
    public struct Message
    {
        public string Sender { get; set; }
        public MessageType MessageType { get; set; }
        public string Content { get; set; }
    }
}
