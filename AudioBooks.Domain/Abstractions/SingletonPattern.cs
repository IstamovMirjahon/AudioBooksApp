namespace AudioBooks.Domain.Abstractions
{
    public class SingletonPattern
    {
        private static SingletonPattern _instance;
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Code { get; set; }
        public DateTime CreateTime { get; set; }

        private SingletonPattern() { }

        public static SingletonPattern Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SingletonPattern();
                }
                return _instance;
            }
        }
    }
}
