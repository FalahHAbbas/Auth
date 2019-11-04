using System.Collections.Generic;

namespace Auth.Forms {
    public class SingleResponse<T> {
        public bool Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public class Responses<T> {
        public bool Status { get; set; }
        public string Message { get; set; }
        public List<T> Data { get; set; }
        public int PagesCount { get; set; }
        public int CurrentPage { get; set; }
        public string Type { get; private set; } = typeof(T).Name;
    }
}